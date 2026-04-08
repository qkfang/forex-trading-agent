using FxWebApi.Data;
using FxWebApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace FxWebApi.Services
{
    public class AccountService
    {
        private readonly FxRateService _fxRateService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<AccountService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly List<Account> _accounts;
        private readonly Dictionary<int, List<Position>> _positions;
        private readonly Dictionary<int, List<AccountTransaction>> _transactions;
        private readonly List<LeadNotification> _leads = new();
        private readonly List<TradeNotification> _tradeNotifications = new();
        private readonly object _lock = new();

        public AccountService(FxRateService fxRateService, IHttpClientFactory httpClientFactory,
            IConfiguration config, ILogger<AccountService> logger, IServiceScopeFactory scopeFactory)
        {
            _fxRateService = fxRateService;
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
            _scopeFactory = scopeFactory;

            _accounts = new List<Account>();
            _positions = new Dictionary<int, List<Position>>();
            _transactions = new Dictionary<int, List<AccountTransaction>>();

            LoadCustomerDataFromDatabase();
        }

        private void LoadCustomerDataFromDatabase()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<FxDbContext>();

                var customers = dbContext.Customers
                    .Include(c => c.Portfolios)
                    .ToList();

                if (customers.Count == 0)
                {
                    _logger.LogWarning("No customers found in database");
                    return;
                }

                foreach (var customer in customers)
                {
                    var account = new Account
                    {
                        Id = customer.Id,
                        AccountNumber = $"FX{10000 + customer.Id}",
                        CustomerName = customer.Name,
                        Email = customer.Email,
                        Country = customer.Company,
                        AccountType = "Standard",
                        Status = "Active",
                        Balance = 50000m,
                        Leverage = 100m,
                        CreatedAt = customer.CreatedAt
                    };
                    _accounts.Add(account);

                    // Map open portfolios to positions
                    var positions = customer.Portfolios
                        .Where(p => p.Status == "Open")
                        .Select(p => new Position
                        {
                            PositionId = $"POS{p.Id}",
                            AccountId = customer.Id,
                            CurrencyPair = p.CurrencyPair,
                            Type = p.Direction,
                            Lots = p.Amount / 100000m,
                            OpenRate = p.EntryRate,
                            CurrentRate = p.EntryRate,
                            PnL = 0m,
                            Margin = Math.Round((p.Amount / 100000m * p.EntryRate * 100000m) / account.Leverage, 2),
                            OpenTime = p.OpenedAt
                        })
                        .ToList();
                    _positions[customer.Id] = positions;

                    // Map customer histories to account transactions
                    var histories = dbContext.CustomerHistories
                        .Where(h => h.CustomerId == customer.Id)
                        .OrderBy(h => h.OpenedAt)
                        .ToList();

                    var txList = new List<AccountTransaction>
                    {
                        new AccountTransaction
                        {
                            TransactionId = $"T{customer.Id}00",
                            AccountId = customer.Id,
                            Type = "Deposit",
                            CurrencyPair = "-",
                            Lots = 0,
                            Rate = 0,
                            PnL = 0,
                            BalanceAfter = 50000m,
                            Timestamp = customer.CreatedAt
                        }
                    };

                    decimal balance = 50000m;
                    foreach (var h in histories)
                    {
                        txList.Add(new AccountTransaction
                        {
                            TransactionId = $"T{h.Id}",
                            AccountId = customer.Id,
                            Type = h.Direction,
                            CurrencyPair = h.CurrencyPair,
                            Lots = h.Amount / 100000m,
                            Rate = h.EntryRate,
                            PnL = 0,
                            BalanceAfter = balance,
                            Timestamp = h.OpenedAt
                        });

                        balance += h.PnL;
                        txList.Add(new AccountTransaction
                        {
                            TransactionId = $"TC{h.Id}",
                            AccountId = customer.Id,
                            Type = $"Close {h.Direction}",
                            CurrencyPair = h.CurrencyPair,
                            Lots = h.Amount / 100000m,
                            Rate = h.ExitRate,
                            PnL = h.PnL,
                            BalanceAfter = balance,
                            Timestamp = h.ClosedAt
                        });
                    }

                    account.Balance = balance;
                    _transactions[customer.Id] = txList;
                }

                _logger.LogInformation("Loaded {Count} customer accounts from database", _accounts.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load customer data from database");
            }
        }

        private decimal CalculatePnL(Position pos, decimal currentRate)
        {
            var units = pos.Lots * 100000m;
            return pos.Type == "Buy"
                ? (currentRate - pos.OpenRate) * units
                : (pos.OpenRate - currentRate) * units;
        }

        private decimal CalculateMargin(decimal lots, decimal rate, decimal leverage)
        {
            return Math.Round((lots * 100000m * rate) / leverage, 2);
        }

        private List<Position> GetPositions(int accountId)
        {
            if (!_positions.ContainsKey(accountId))
                _positions[accountId] = new List<Position>();
            return _positions[accountId];
        }

        private List<AccountTransaction> GetTransactions(int accountId)
        {
            if (!_transactions.ContainsKey(accountId))
                _transactions[accountId] = new List<AccountTransaction>();
            return _transactions[accountId];
        }

        public List<AccountSummary> GetAllAccounts()
        {
            lock (_lock)
            {
                var currentRate = _fxRateService.GetCurrentRate().Rate;
                var summaries = new List<AccountSummary>();

                foreach (var account in _accounts)
                {
                    var positions = GetPositions(account.Id);
                    var openPnL = positions.Sum(p => CalculatePnL(p, currentRate));

                    summaries.Add(new AccountSummary
                    {
                        Id = account.Id,
                        AccountNumber = account.AccountNumber,
                        CustomerName = account.CustomerName,
                        AccountType = account.AccountType,
                        Status = account.Status,
                        Country = account.Country,
                        Balance = account.Balance,
                        Equity = Math.Round(account.Balance + openPnL, 2),
                        OpenPnL = Math.Round(openPnL, 2),
                        OpenPositionsCount = positions.Count,
                        Leverage = account.Leverage,
                        CreatedAt = account.CreatedAt
                    });
                }

                return summaries;
            }
        }

        public BalanceSheet? GetBalanceSheet(int accountId)
        {
            lock (_lock)
            {
                var account = _accounts.FirstOrDefault(a => a.Id == accountId);
                if (account == null) return null;

                var currentRate = _fxRateService.GetCurrentRate().Rate;
                var positions = GetPositions(accountId);

                foreach (var pos in positions)
                {
                    pos.CurrentRate = currentRate;
                    pos.PnL = Math.Round(CalculatePnL(pos, currentRate), 2);
                }

                var openPnL = positions.Sum(p => p.PnL);
                var margin = positions.Sum(p => p.Margin);
                var equity = account.Balance + openPnL;
                var freeMargin = equity - margin;
                var marginLevel = margin > 0 ? (equity / margin) * 100 : 0;

                return new BalanceSheet
                {
                    AccountId = account.Id,
                    AccountNumber = account.AccountNumber,
                    CustomerName = account.CustomerName,
                    AccountType = account.AccountType,
                    Status = account.Status,
                    Balance = Math.Round(account.Balance, 2),
                    Equity = Math.Round(equity, 2),
                    OpenPnL = Math.Round(openPnL, 2),
                    Margin = Math.Round(margin, 2),
                    FreeMargin = Math.Round(freeMargin, 2),
                    MarginLevel = margin > 0 ? Math.Round(marginLevel, 2) : 0,
                    OpenPositions = positions.ToList(),
                    RecentTransactions = GetTransactions(accountId)
                        .OrderByDescending(t => t.Timestamp)
                        .Take(10)
                        .ToList()
                };
            }
        }

        public FxTransactionResult ExecuteTrade(int accountId, string type, decimal lots)
        {
            lock (_lock)
            {
                var account = _accounts.FirstOrDefault(a => a.Id == accountId);
                if (account == null)
                    return new FxTransactionResult { Success = false, Message = "Account not found" };

                if (lots <= 0)
                    return new FxTransactionResult { Success = false, Message = "Invalid lots amount" };

                var currentRate = _fxRateService.GetCurrentRate().Rate;
                var margin = CalculateMargin(lots, currentRate, account.Leverage);
                var acctPositions = GetPositions(accountId);
                var openPnL = acctPositions.Sum(p => CalculatePnL(p, currentRate));
                var equity = account.Balance + openPnL;
                var usedMargin = acctPositions.Sum(p => p.Margin);
                var freeMargin = equity - usedMargin;

                if (margin > freeMargin)
                    return new FxTransactionResult { Success = false, Message = "Insufficient margin" };

                var position = new Position
                {
                    PositionId = $"POS{DateTime.UtcNow.Ticks}",
                    AccountId = accountId,
                    CurrencyPair = "AUD/USD",
                    Type = type,
                    Lots = lots,
                    OpenRate = currentRate,
                    CurrentRate = currentRate,
                    PnL = 0m,
                    Margin = Math.Round(margin, 2),
                    OpenTime = DateTime.UtcNow
                };

                GetPositions(accountId).Add(position);

                GetTransactions(accountId).Add(new AccountTransaction
                {
                    TransactionId = $"T{DateTime.UtcNow.Ticks}",
                    AccountId = accountId,
                    Type = type,
                    CurrencyPair = "AUD/USD",
                    Lots = lots,
                    Rate = currentRate,
                    PnL = 0,
                    BalanceAfter = account.Balance,
                    Timestamp = DateTime.UtcNow
                });

                var result = new FxTransactionResult
                {
                    Success = true,
                    Message = $"{type} {lots} lots AUD/USD at {currentRate:F4} executed successfully",
                    Transaction = new FxTransaction
                    {
                        Type = type,
                        CurrencyPair = "AUD/USD",
                        Amount = lots,
                        Rate = currentRate
                    }
                };

                return result;
            }
        }

        public FxTransactionResult ClosePosition(int accountId, string positionId)
        {
            lock (_lock)
            {
                var account = _accounts.FirstOrDefault(a => a.Id == accountId);
                if (account == null)
                    return new FxTransactionResult { Success = false, Message = "Account not found" };

                var closePositions = GetPositions(accountId);
                var position = closePositions.FirstOrDefault(p => p.PositionId == positionId);
                if (position == null)
                    return new FxTransactionResult { Success = false, Message = "Position not found" };

                var currentRate = _fxRateService.GetCurrentRate().Rate;
                var pnl = Math.Round(CalculatePnL(position, currentRate), 2);

                closePositions.Remove(position);
                account.Balance += pnl;

                var closeType = position.Type == "Buy" ? "Close Buy" : "Close Sell";
                GetTransactions(accountId).Add(new AccountTransaction
                {
                    TransactionId = $"T{DateTime.UtcNow.Ticks}",
                    AccountId = accountId,
                    Type = closeType,
                    CurrencyPair = "AUD/USD",
                    Lots = position.Lots,
                    Rate = currentRate,
                    PnL = pnl,
                    BalanceAfter = Math.Round(account.Balance, 2),
                    Timestamp = DateTime.UtcNow
                });

                return new FxTransactionResult
                {
                    Success = true,
                    Message = $"Position closed. P&L: {(pnl >= 0 ? "+" : "")}{pnl:F2}",
                    Transaction = new FxTransaction
                    {
                        Type = closeType,
                        CurrencyPair = "AUD/USD",
                        Amount = position.Lots,
                        Rate = currentRate
                    }
                };
            }
        }

        public void AddLead(LeadNotification lead)
        {
            lock (_lock)
            {
                _leads.Add(lead);
            }
        }

        public List<LeadNotification> GetLeads()
        {
            lock (_lock)
            {
                return _leads.OrderByDescending(l => l.ReceivedAt).ToList();
            }
        }

        public void AddTradeNotification(TradeNotification notification)
        {
            lock (_lock)
            {
                _tradeNotifications.Add(notification);

                // Find a matching account by customer name, or use the first account
                var account = _accounts.FirstOrDefault(a =>
                    a.CustomerName.Equals(notification.CustomerName, StringComparison.OrdinalIgnoreCase))
                    ?? _accounts.FirstOrDefault();

                if (account != null)
                {
                    var rate = notification.Rate > 0 ? notification.Rate : _fxRateService.GetCurrentRate().Rate;
                    var margin = CalculateMargin(notification.Lots, rate, account.Leverage);

                    // Add position
                    GetPositions(account.Id).Add(new Position
                    {
                        PositionId = $"POS{DateTime.UtcNow.Ticks}",
                        AccountId = account.Id,
                        CurrencyPair = notification.CurrencyPair,
                        Type = notification.Direction,
                        Lots = notification.Lots,
                        OpenRate = rate,
                        CurrentRate = rate,
                        PnL = 0m,
                        Margin = Math.Round(margin, 2),
                        OpenTime = DateTime.UtcNow
                    });

                    // Add transaction
                    GetTransactions(account.Id).Add(new AccountTransaction
                    {
                        TransactionId = notification.TransactionId,
                        AccountId = account.Id,
                        Type = notification.Direction,
                        CurrencyPair = notification.CurrencyPair,
                        Lots = notification.Lots,
                        Rate = rate,
                        PnL = 0,
                        BalanceAfter = account.Balance,
                        Timestamp = DateTime.UtcNow
                    });

                    _logger.LogInformation("Trade notification applied to account {AccountId} ({Customer}): {Direction} {Lots} lots {Pair} @ {Rate}",
                        account.Id, account.CustomerName, notification.Direction, notification.Lots, notification.CurrencyPair, rate);
                }
            }
        }

        public List<TradeNotification> GetTradeNotifications()
        {
            lock (_lock)
            {
                return _tradeNotifications.OrderByDescending(t => t.ReceivedAt).ToList();
            }
        }

    }
}
