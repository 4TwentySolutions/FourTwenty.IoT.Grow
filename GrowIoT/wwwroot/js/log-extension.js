
window.scrollLogsToBottom = () => {
    var elem = document.getElementsByClassName('real-time-output')[0];
    elem.scrollTop = elem.scrollHeight;
};