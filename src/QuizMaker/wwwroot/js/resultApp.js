define(["require", "exports", "./resultTypes"], function (require, exports, resultTypes_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var protocol = new signalR.JsonHubProtocol();
    var hubRoute = "/QuizResultsHub";
    var connection = new signalR.HubConnectionBuilder()
        .configureLogging(signalR.LogLevel.Information)
        .withUrl(hubRoute)
        .withHubProtocol(protocol)
        .build();
    var quizId = document.location.href.split('/')[document.location.href.split('/').length - 1];
    var results = new resultTypes_1.ResultViewModel();
    results.quizId = quizId;
    results.responses = 0;
    var labels = new Array();
    var data = {
        labels: [],
        datasets: [{
                backgroundColor: "blue",
                borderColor: "blue",
                data: []
            }]
    };
    function getQuestionTitle(results) {
        var title = "Results";
        if (results.results.length > 0) {
            title = results.results[0].questionTitle;
        }
        return title;
    }
    function renderQuizResults(results, forceDraw) {
        var resultsTitleElement = document.getElementById("resultsTitle");
        resultsTitleElement.innerText = getQuestionTitle(results);
        data.labels = [];
        data.datasets[0].data = [];
        var resultQuestion = new resultTypes_1.ResultQuestionViewModel();
        if (results.results.length > 0) {
            resultQuestion = results.results[0];
            for (var i = 0; i < resultQuestion.answers.length; i++) {
                var answer = resultQuestion.answers[i];
                data.labels.push(answer.name);
                data.datasets[0].data.push(answer.count);
            }
        }
        var responsesElement = document.getElementById("responses");
        responsesElement.innerHTML = results.responses + " \uD83D\uDCDD";
        console.log(labels);
        console.log(forceDraw);
        console.log(data);
        if (forceDraw) {
            var canvas = document.getElementById('quizChart');
            var containerElement = document.getElementById("containerElement");
            canvas.width = Math.min(containerElement.clientWidth, window.innerWidth * 0.8);
            canvas.height = Math.min(containerElement.clientHeight, window.innerHeight * 0.8);
            var context = canvas.getContext('2d');
            context.clearRect(0, 0, canvas.width, canvas.height);
            var config = {
                type: 'bar',
                data: data,
                options: {
                    responsive: false,
                    legend: {
                        display: false
                    },
                    scales: {
                        xAxes: [{
                                ticks: {
                                    fontSize: 16
                                }
                            }],
                        yAxes: [{
                                ticks: {
                                    fontSize: 16,
                                    suggestedMin: 0,
                                    suggestedMax: 5,
                                    stepSize: 1
                                }
                            }]
                    }
                }
            };
            window.resultChart = new Chart.Chart(context, config);
        }
        else {
            window.resultChart.update();
        }
    }
    function updateUserCount(connection) {
        var usersElement = document.getElementById("users");
        usersElement.innerHTML = connection.counter + " \uD83D\uDC65";
    }
    connection.on('Connected', function (connection) {
        updateUserCount(connection);
    });
    connection.on('Disconnected', function (connection) {
        updateUserCount(connection);
    });
    connection.on('Results', function (r) {
        var data = "Results received: " + new Date().toLocaleTimeString();
        console.log(data);
        console.log(r);
        results = r;
        renderQuizResults(results, false);
    });
    connection.onclose(function (e) {
        if (e) {
            console.log("Connection closed with error: " + e);
        }
        else {
            console.log("Disconnected");
        }
    });
    connection.start()
        .then(function () {
        console.log("SignalR connected");
        connection.invoke("GetResults", quizId)
            .then(function () {
            console.log("GetResults called");
        })
            .catch(function (err) {
            console.log("GetResults submission error");
            console.log(err);
        });
    })
        .catch(function (err) {
        console.log("SignalR error");
        console.log(err);
        console.log(err);
    });
    window.addEventListener('resize', function () {
        console.log("resize");
        renderQuizResults(results, true);
    });
    renderQuizResults(results, true);
});
//# sourceMappingURL=resultApp.js.map