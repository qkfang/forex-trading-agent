-- Seed mock data for ResearchArticles, Customers, and CustomerPortfolios
-- Run against the FxDatabase after migrations have been applied

-- Clear existing data (order matters for FK constraints)
DELETE FROM CustomerPortfolios;
DELETE FROM Customers;
DELETE FROM ResearchArticles;

-- Reset identity seeds
DBCC CHECKIDENT ('Customers', RESEED, 0);
DBCC CHECKIDENT ('ResearchArticles', RESEED, 0);
DBCC CHECKIDENT ('CustomerPortfolios', RESEED, 0);

-------------------------------------------------------
-- Customers (10 records)
-------------------------------------------------------
INSERT INTO Customers (Name, Email, Phone, Company, CreatedAt) VALUES
('Alice Johnson',   'alice.johnson@example.com',   '+1-555-0101', 'Global Trading Co',      '2025-09-15T08:30:00'),
('Bob Smith',       'bob.smith@example.com',       '+1-555-0102', 'Pacific Investments',     '2025-10-02T14:20:00'),
('Carlos Rivera',   'carlos.rivera@example.com',   '+44-20-7946-0201', 'Euro Capital Ltd',   '2025-10-18T09:00:00'),
('Diana Chen',      'diana.chen@example.com',      '+852-3456-7890', 'Asia Macro Fund',      '2025-11-05T11:45:00'),
('Erik Lindberg',   'erik.lindberg@example.com',   '+46-8-123-4567', 'Nordic FX Partners',   '2025-11-20T07:15:00'),
('Fatima Al-Rashid','fatima.alrashid@example.com', '+971-4-555-6789', 'Gulf Finance Group',  '2025-12-01T10:30:00'),
('Grace Park',      'grace.park@example.com',      '+82-2-555-3456', 'Seoul Wealth Mgmt',    '2026-01-10T13:00:00'),
('Hugo Martinez',   'hugo.martinez@example.com',   '+52-55-5555-7890', 'LatAm FX Advisors', '2026-01-25T16:45:00'),
('Isla Thompson',   'isla.thompson@example.com',   '+61-2-5555-1234', 'Oceania Partners',   '2026-02-14T08:00:00'),
('James Okafor',    'james.okafor@example.com',    '+234-1-555-9012', 'Frontier Markets Inc','2026-03-01T12:30:00');

-------------------------------------------------------
-- ResearchArticles (15 records)
-------------------------------------------------------
INSERT INTO ResearchArticles (Title, Summary, Content, Category, Author, PublishedDate, Status, Tags, Sentiment) VALUES
('USD Strength Outlook Q2 2026',
 'Analysis of USD momentum heading into Q2 driven by Fed policy expectations.',
 'The US dollar has shown resilience amid shifting rate expectations. Key drivers include persistent inflation data and labor market tightness. We expect the DXY to trade in the 104-107 range through Q2.',
 'Macro Analysis', 'Sarah Mitchell', '2026-03-28T09:00:00', 'Published', 'USD,Fed,interest-rates,macro', 'Bullish'),

('EUR/USD Technical Levels to Watch',
 'Key support and resistance zones for EUR/USD in the near term.',
 'EUR/USD is consolidating near the 1.0820 level after failing to break above 1.0900. The 200-day moving average at 1.0780 provides critical support. A break below opens the door to 1.0700.',
 'Technical Analysis', 'Marco Rossi', '2026-03-30T10:30:00', 'Published', 'EUR/USD,technical,support,resistance', 'Bearish'),

('GBP Impact from UK Fiscal Policy',
 'How upcoming UK budget decisions may influence the British pound.',
 'The UK government faces challenging fiscal choices that could weigh on sterling. Potential tax increases and spending cuts may slow growth, while the Bank of England remains cautious on rate cuts.',
 'Macro Analysis', 'Emily Carter', '2026-03-25T08:15:00', 'Published', 'GBP,UK,fiscal-policy,BoE', 'Bearish'),

('JPY Carry Trade Dynamics in 2026',
 'Examining whether the yen carry trade remains attractive given BOJ policy shifts.',
 'With the BOJ gradually normalizing policy, the traditional yen carry trade faces headwinds. However, the wide rate differential with the US continues to support short-yen positions for now.',
 'Strategy', 'Kenji Tanaka', '2026-04-01T07:00:00', 'Published', 'JPY,carry-trade,BOJ,rates', 'Neutral'),

('AUD/NZD Cross Rate Opportunity',
 'Relative value opportunity in the Antipodean cross as fundamentals diverge.',
 'Australian economic data has outperformed New Zealand recently, creating a tactical long AUD/NZD opportunity. We target 1.1050 with a stop at 1.0850.',
 'Trade Ideas', 'Liam O''Brien', '2026-04-02T11:00:00', 'Published', 'AUD/NZD,relative-value,Antipodean', 'Bullish'),

('CHF Safe Haven Flows Monitor',
 'Tracking safe haven demand for the Swiss franc amid geopolitical uncertainty.',
 'Geopolitical tensions have periodically boosted CHF demand. We monitor EUR/CHF positioning and SNB intervention signals as key indicators for franc direction.',
 'Macro Analysis', 'Sarah Mitchell', '2026-04-03T09:45:00', 'Published', 'CHF,safe-haven,SNB,geopolitics', 'Neutral'),

('Emerging Market FX Weekly Review',
 'Weekly performance review of major EM currencies against the dollar.',
 'EM currencies had a mixed week. BRL and MXN outperformed on commodity strength, while TRY and ZAR lagged due to domestic policy concerns. Overall EM sentiment remains cautiously optimistic.',
 'Weekly Review', 'Hugo Santos', '2026-04-04T06:30:00', 'Published', 'EM,BRL,MXN,TRY,ZAR,weekly', 'Neutral'),

('CAD and Oil Price Correlation Update',
 'Reassessing the CAD-crude oil relationship as energy markets evolve.',
 'The historical correlation between CAD and WTI oil prices has weakened in 2026. Diversification of Canada''s economy and shifting trade patterns suggest a more nuanced approach to trading USD/CAD.',
 'Correlation Study', 'Emily Carter', '2026-03-20T14:00:00', 'Published', 'CAD,oil,correlation,USD/CAD', 'Neutral'),

('Central Bank Watch: April 2026',
 'Preview of upcoming central bank meetings and expected policy decisions.',
 'April features decisions from the ECB, BOC, and RBA. Markets price a 60% chance of an ECB cut, while the BOC and RBA are expected to hold. We outline scenario analysis for each.',
 'Central Banks', 'Marco Rossi', '2026-04-01T08:00:00', 'Published', 'central-banks,ECB,BOC,RBA,rates', 'Neutral'),

('USD/CNH: Navigating Trade Policy Risks',
 'How evolving trade policies affect the offshore yuan outlook.',
 'Trade policy uncertainty continues to create volatility in USD/CNH. We analyze tariff scenarios and PBOC management of the daily fix as key drivers for the pair.',
 'Geopolitics', 'Kenji Tanaka', '2026-03-27T10:00:00', 'Published', 'CNH,trade-policy,PBOC,tariffs', 'Bearish'),

('Volatility Smile Analysis: Major Pairs',
 'Options market signals for G10 currency pairs.',
 'Risk reversals across major pairs show elevated demand for USD calls vs EUR and GBP puts, signaling hedging activity against further dollar strength. Vol surfaces suggest limited downside risk to USD.',
 'Options & Volatility', 'Sarah Mitchell', '2026-04-03T12:00:00', 'Published', 'volatility,options,risk-reversal,G10', 'Bullish'),

('NOK/SEK Divergence Play',
 'Scandinavian currencies offer a mean-reversion opportunity.',
 'NOK has underperformed SEK by 3% YTD despite similar macro fundamentals. Oil price stabilization and Norges Bank rhetoric support a tactical long NOK/SEK position targeting 0.9850.',
 'Trade Ideas', 'Erik Svensson', '2026-03-22T09:30:00', 'Draft', 'NOK,SEK,Scandinavia,mean-reversion', 'Bullish'),

('Digital Currency Impact on FX Markets',
 'How CBDCs and stablecoin adoption are reshaping foreign exchange flows.',
 'Central bank digital currencies are progressing from pilot to implementation in several economies. We examine how this may affect cross-border payment flows and traditional FX market structure.',
 'Thematic Research', 'Liam O''Brien', '2026-03-18T15:00:00', 'Published', 'CBDC,digital-currency,fintech,structure', 'Neutral'),

('GBP/JPY Breakout Watch',
 'GBP/JPY approaching a multi-month range breakout level.',
 'GBP/JPY has been compressing in a 188.00-192.50 range for eight weeks. Momentum indicators suggest a breakout is imminent. A close above 192.50 targets 196.00; below 188.00 opens 184.50.',
 'Technical Analysis', 'Marco Rossi', '2026-04-04T07:45:00', 'Published', 'GBP/JPY,technical,breakout,range', 'Bullish'),

('Quarterly FX Forecast Update',
 'Updated forecasts for major and select EM currency pairs through Q4 2026.',
 'We revise our EUR/USD forecast lower to 1.0600 by year-end from 1.0900 previously. GBP/USD is adjusted to 1.2400. USD/JPY target raised to 158. Full forecast table included.',
 'Forecast', 'Emily Carter', '2026-04-05T06:00:00', 'Draft', 'forecast,EUR/USD,GBP/USD,USD/JPY,outlook', 'Bearish');

-------------------------------------------------------
-- CustomerPortfolios (20 records)
-------------------------------------------------------
-- Note: Fabric SQL RESEED 0 produces IDs starting at 0, so CustomerId references are 0-based
INSERT INTO CustomerPortfolios (CustomerId, CurrencyPair, Direction, Amount, EntryRate, OpenedAt, Status) VALUES
(0, 'EUR/USD', 'Buy',  100000.0000, 1.082300, '2026-03-10T09:15:00', 'Open'),
(0, 'GBP/USD', 'Sell',  75000.0000, 1.264500, '2026-03-15T14:30:00', 'Open'),
(1, 'USD/JPY', 'Buy',  200000.0000, 151.450000, '2026-02-20T08:00:00', 'Closed'),
(1, 'AUD/USD', 'Buy',   50000.0000, 0.653200, '2026-03-22T10:45:00', 'Open'),
(2, 'EUR/GBP', 'Sell', 150000.0000, 0.856100, '2026-03-01T11:00:00', 'Open'),
(2, 'USD/CHF', 'Buy',  120000.0000, 0.882400, '2026-03-18T07:30:00', 'Closed'),
(3, 'USD/CNH', 'Sell', 300000.0000, 7.245600, '2026-02-28T04:00:00', 'Open'),
(3, 'AUD/NZD', 'Buy',   80000.0000, 1.087500, '2026-04-02T06:15:00', 'Open'),
(4, 'EUR/SEK', 'Sell', 250000.0000, 11.235000, '2026-03-12T08:45:00', 'Closed'),
(4, 'NOK/SEK', 'Buy',  100000.0000, 0.972000, '2026-03-25T09:00:00', 'Open'),
(5, 'USD/AED', 'Buy',  500000.0000, 3.672900, '2026-01-15T10:00:00', 'Closed'),
(5, 'EUR/USD', 'Sell', 200000.0000, 1.078600, '2026-04-01T13:20:00', 'Open'),
(6, 'USD/KRW', 'Sell', 150000.0000, 1345.500000, '2026-03-05T02:30:00', 'Open'),
(6, 'GBP/JPY', 'Buy',  90000.0000, 190.780000, '2026-04-03T05:00:00', 'Open'),
(7, 'USD/MXN', 'Sell', 175000.0000, 17.125000, '2026-02-10T16:00:00', 'Closed'),
(7, 'EUR/USD', 'Buy',  100000.0000, 1.085100, '2026-04-04T09:30:00', 'Open'),
(8, 'AUD/USD', 'Sell',  60000.0000, 0.657800, '2026-03-28T01:00:00', 'Open'),
(8, 'NZD/USD', 'Buy',   45000.0000, 0.598400, '2026-04-01T22:45:00', 'Open'),
(9, 'USD/ZAR', 'Buy', 200000.0000, 18.450000, '2026-03-08T12:00:00', 'Closed'),
(9, 'GBP/USD', 'Buy', 130000.0000, 1.258900, '2026-04-02T14:00:00', 'Open');
