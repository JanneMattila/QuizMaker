declare const d3: typeof import("d3");
declare const signalR: typeof import("@aspnet/signalr");
import { ConnectionViewModel } from "./quizTypes";
import { ResultViewModel, ResultQuestionViewModel, ResultQuestionAnswerViewModel } from "./resultTypes";

const protocol = new signalR.JsonHubProtocol();
const hubRoute = "/QuizResultsHub";
const connection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Information)
    .withUrl(hubRoute)
    .withHubProtocol(protocol)
    .build();

const quizId = document.location.href.split('/')[document.location.href.split('/').length - 1];

let results = new ResultViewModel();
results.quizId = quizId;
results.responses = 0;

function getQuestionTitle(results: ResultViewModel) {
    let title = "Results";
    if (results.results.length > 0) {
        title = results.results[0].questionTitle;
    }
    return title;
}

function renderQuizResults(results: ResultViewModel) {

    const resultsTitleElement = document.getElementById("resultsTitle") as HTMLElement;
    resultsTitleElement.innerText = getQuestionTitle(results);

    let resultQuestion = new ResultQuestionViewModel();
    if (results.results.length > 0) {
        resultQuestion = results.results[0];
    }

    const responsesElement = document.getElementById("responses");
    responsesElement.innerHTML = `${results.responses} 📝`;

    const svg = d3.select("svg");
    svg.selectAll("*").remove();
    const containerElement = document.getElementById("containerElement") as HTMLDivElement;
    svg.attr("width", Math.min(containerElement.clientWidth, window.innerWidth * 0.8));
    svg.attr("height", Math.min(containerElement.clientHeight, window.innerHeight * 0.8));
    const margin = {
        top: 20,
        right: 20,
        bottom: 30,
        left: 50
    };
    const width = +svg.attr("width") - margin.left - margin.right;
    const height = +svg.attr("height") - margin.top - margin.bottom;
    const g = svg.append("g").attr("transform", "translate(" + margin.left + "," + margin.top + ")");
    const x = d3.scaleBand()
        .rangeRound([0, width])
        .padding(0.1);

    const y = d3.scaleLinear()
        .rangeRound([height, 0]);

    x.domain(resultQuestion.answers.map(function (d: ResultQuestionAnswerViewModel) {
        return d.name;
    }));
    y.domain([0, d3.max(resultQuestion.answers, function (d: ResultQuestionAnswerViewModel) {
        return Number(d.count);
    })]);

    g.append("g")
        .attr("transform", "translate(0," + height + ")")
        .style("font-size", "16px")
        .call(d3.axisBottom(x))

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

function updateUserCount(connection: ConnectionViewModel) {
    const usersElement = document.getElementById("users");
    usersElement.innerHTML = `${connection.counter} 👥`;
}

connection.on('Connected', function (connection: ConnectionViewModel) {
    updateUserCount(connection);
});

connection.on('Disconnected', function (connection: ConnectionViewModel) {
    updateUserCount(connection);
});

connection.on('Results', function (r: ResultViewModel) {
    const data = "Results received: " + new Date().toLocaleTimeString();
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

        connection.invoke<string>("GetResults", quizId)
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

window.addEventListener('resize', () => {
    console.log("resize");
    renderQuizResults(results);
});

renderQuizResults(results);
