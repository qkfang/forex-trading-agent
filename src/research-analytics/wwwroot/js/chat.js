(function () {
    var panel = document.getElementById('chat-panel');
    var toggle = document.getElementById('chat-toggle');
    var closeBtn = document.getElementById('chat-close');
    var inputEl = document.getElementById('chat-input');
    var sendBtn = document.getElementById('chat-send');
    var messages = document.getElementById('chat-messages');
    var history = [];

    function getInputValue() {
        // fluent-text-field exposes .value on the custom element
        return (inputEl.value || '').trim();
    }
    function setInputValue(v) {
        inputEl.value = v;
    }

    toggle.addEventListener('click', function () {
        var open = panel.style.display === 'none' || panel.style.display === '';
        panel.style.display = open ? 'flex' : 'none';
        if (open) inputEl.focus();
    });

    closeBtn.addEventListener('click', function () {
        panel.style.display = 'none';
    });

    inputEl.addEventListener('keydown', function (e) {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            sendMessage();
        }
    });

    sendBtn.addEventListener('click', sendMessage);

    function appendMessage(role, text) {
        var div = document.createElement('div');
        div.className = 'chat-msg ' + role;
        var bubble = document.createElement('div');
        bubble.className = 'chat-bubble';
        bubble.textContent = text;
        div.appendChild(bubble);
        messages.appendChild(div);
        messages.scrollTop = messages.scrollHeight;
        return bubble;
    }

    function sendMessage() {
        var text = getInputValue();
        if (!text) return;

        appendMessage('user', text);
        setInputValue('');
        inputEl.setAttribute('disabled', '');
        sendBtn.setAttribute('disabled', '');

        var thinking = appendMessage('assistant', '…');

        fetch('/api/chat', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ message: text, history: history })
        })
        .then(function (res) { return res.json(); })
        .then(function (data) {
            thinking.textContent = data.reply || 'No response.';
            history.push({ role: 'user', content: text });
            history.push({ role: 'assistant', content: data.reply || '' });
            if (history.length > 40) history = history.slice(history.length - 40);
        })
        .catch(function () {
            thinking.textContent = 'Sorry, something went wrong. Please try again.';
        })
        .finally(function () {
            inputEl.removeAttribute('disabled');
            sendBtn.removeAttribute('disabled');
            inputEl.focus();
        });
    }
})();
