System.register(["./resultTypes.js"], function (exports_1, context_1) {
    "use strict";
    var resultTypes_js_1, protocol, hubRoute, connection, quizId, results;
    var __moduleName = context_1 && context_1.id;
    function getQuestionTitle(results) {
        var title = "Results";
        if (results.results.length > 0) {
            title = results.results[0].questionTitle;
        }
        return title;
    }
    function renderQuizResults(results) {
        var resultsTitleElement = document.getElementById("resultsTitle");
        resultsTitleElement.innerText = getQuestionTitle(results);
        var resultQuestion = new resultTypes_js_1.ResultQuestionViewModel();
        if (results.results.length > 0) {
            resultQuestion = results.results[0];
        }
        var responsesElement = document.getElementById("responses");
        responsesElement.innerHTML = results.responses + " \uD83D\uDCDD";
        var svg = d3.select("svg");
        svg.selectAll("*").remove();
        var containerElement = document.getElementById("containerElement");
        svg.attr("width", Math.min(containerElement.clientWidth, window.innerWidth * 0.8));
        svg.attr("height", Math.min(containerElement.clientHeight, window.innerHeight * 0.8));
        var margin = {
            top: 20,
            right: 20,
            bottom: 30,
            left: 50
        };
        var width = +svg.attr("width") - margin.left - margin.right;
        var height = +svg.attr("height") - margin.top - margin.bottom;
        var g = svg.append("g").attr("transform", "translate(" + margin.left + "," + margin.top + ")");
        var x = d3.scaleBand()
            .rangeRound([0, width])
            .padding(0.1);
        var y = d3.scaleLinear()
            .rangeRound([height, 0]);
        x.domain(resultQuestion.answers.map(function (d) {
            return d.name;
        }));
        y.domain([0, d3.max(resultQuestion.answers, function (d) {
                return Number(d.count);
            })]);
        g.append("g")
            .attr("transform", "translate(0," + height + ")")
            .style("font-size", "16px")
            .call(d3.axisBottom(x));
        g.append("g")
            .call(d3.axisLeft(y))
            .style("font-size", "16px")
            .append("text")
            .attr("fill", "#000")
            .attr("transform", "rotate(-90)")
            .attr("y", 6)
            .attr("dy", "0.71em")
            .attr("text-anchor", "end")
            .text("Count");
        g.selectAll(".quiz-results-bar")
            .data(resultQuestion.answers)
            .enter().append("rect")
            .attr("class", "quiz-results-bar")
            .attr("x", function (d) {
            return x(d.name);
        })
            .attr("y", function (d) {
            return y(Number(d.count));
        })
            .attr("width", x.bandwidth())
            .attr("height", function (d) {
            return height - y(Number(d.count));
        });
    }
    function updateUserCount(connection) {
        var usersElement = document.getElementById("users");
        usersElement.innerHTML = connection.counter + " \uD83D\uDC65";
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
                renderQuizResults(results);
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
                renderQuizResults(results);
            });
            renderQuizResults(results);
        }
    };
});
//# sourceMappingURL=resultApp2.js.map