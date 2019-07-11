declare var signalR: typeof import("@aspnet/signalr");

function addMessage(msg:any) {
    console.log(msg);
}

let protocol = new signalR.JsonHubProtocol();
let hubRoute = "QuizHub";
let connection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Information)
    .withUrl(hubRoute)
    .withHubProtocol(protocol)
    .build();

connection.on('Connected', function (msg:any) {
    let data = "Date received: " + new Date().toLocaleTimeString();
    addMessage(data);
    addMessage(msg);
});

connection.onclose(function (e:any) {
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
    .catch(function (err:any) {
        console.log("SignalR error");
        console.log(err);

        addMessage(err);
    });