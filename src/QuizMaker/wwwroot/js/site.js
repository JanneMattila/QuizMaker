function addMessage(msg) {
    console.log(msg);
}
var protocol = new signalR.JsonHubProtocol();
var hubRoute = "QuizHub";
var connection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Information)
    .withUrl(hubRoute)
    .withHubProtocol(protocol)
    .build();
connection.on('Connected', function (msg) {
    var data = "Date received: " + new Date().toLocaleTimeString();
    addMessage(data);
    addMessage(msg);
});
connection.onclose(function (e) {
    if (e) {
        addMessage("Connection closed with error: " + e);
    }
    else {
        addMessage("Disconnected");
    }
});
connection.start()
    .then(function () {
    console.log("SignalR connected");
})
    .catch(function (err) {
    console.log("SignalR error");
    console.log(err);
    addMessage(err);
});
//# sourceMappingURL=site.js.map