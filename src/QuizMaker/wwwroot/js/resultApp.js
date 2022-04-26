System.register(["./resultTypes.js"], function (exports_1, context_1) {
    "use strict";
    var resultTypes_js_1, protocol, hubRoute, connection, quizId, results, labels, data;
    var __moduleName = context_1 && context_1.id;
    function getQuestionTitle(results) {
        var title = "Results";
        if (results.results.length > 0) {
            title = results.results[0].questionTitle;
        }
        return title;
    }
    function renderQuizResults(results, forceDraw) {
        data.labels = [];
        data.datasets[0].data = [];
        var resultQuestion = new resultTypes_js_1.ResultQuestionViewModel();
        if (results.results.length > 0) {
            resultQuestion = results.results[0];
            for (var i = 0; i < resultQuestion.answers.length; i++) {
                var answer = resultQuestion.answers[i];
                data.labels.push(answer.name);
                data.datasets[0].data.push(answer.count);
            }
        }
        var responsesElement = document.getElementById("responses");
        responsesElement.innerHTML = "".concat(results.responses, " \uD83D\uDCDD");
        console.log(labels);
        console.log("forceDraw: ".concat(forceDraw));
        console.log(data);
        if (forceDraw) {
            if (window.resultChart !== undefined) {
                // Release prior chart.js charts
                window.resultChart.destroy();
            }
            var canvas = document.getElementById('quizChart');
            var containerElement = document.getElementById("containerElement");
            canvas.width = document.documentElement.clientWidth * 0.9;
            canvas.height = document.documentElement.clientHeight * 0.9;
            var context = canvas.getContext('2d');
            context.clearRect(0, 0, canvas.width, canvas.height);
            Chart.defaults.font.size = 16;
            var config = {
                type: 'bar',
                data: data,
                options: {
                    responsive: false,
                    plugins: {
                        title: {
                            text: getQuestionTitle(results),
                            display: true,
                            fullSize: true,
                            color: '#000000',
                            font: {
                                size: 24
                            }
                        },
                        legend: {
                            display: false
                        }
                    },
                    scales: {
                        x: {},
                        y: {
                            suggestedMin: 0,
                            suggestedMax: 5,
                            ticks: {
                                stepSize: 1
                            }
                        }
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
        usersElement.innerHTML = "".concat(connection.counter, " \uD83D\uDC65");
    }
    return {
        setters: [
            function (resultTypes_js_1_1) {
                resultTypes_js_1 = resultTypes_js_1_1;
            }
        ],
        execute: function () {
            protocol = new signalR.JsonHubProtocol();
            hubRoute = "/QuizResultsHub";
            connection = new signalR.HubConnectionBuilder()
                .configureLogging(signalR.LogLevel.Information)
                .withUrl(hubRoute)
                .withHubProtocol(protocol)
                .build();
            quizId = document.location.href.split('/')[document.location.href.split('/').length - 1];
            results = new resultTypes_js_1.ResultViewModel();
            results.quizId = quizId;
            results.responses = 0;
            labels = new Array();
            data = {
                labels: [],
                datasets: [{
                        backgroundColor: "blue",
                        borderColor: "blue",
                        data: []
                    }]
            };
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
                var forceDraw = false;
                if (results !== undefined &&
                    r !== undefined &&
                    results.quizId !== r.quizId) {
                    forceDraw = true;
                    console.log("Quiz change so forcing re-draw");
                }
                results = r;
                renderQuizResults(results, forceDraw);
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
        }
    };
});
//# sourceMappingURL=resultApp.js.map