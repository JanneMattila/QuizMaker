declare var d3: typeof import("d3");
declare var signalR: typeof import("@aspnet/signalr");
import { ConnectionViewModel } from "./quizTypes";
import { ResultViewModel, ResultQuestionViewModel, ResultQuestionAnswerViewModel } from "./resultTypes";

let protocol = new signalR.JsonHubProtocol();
let hubRoute = "/QuizResultsHub";
let connection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Information)
    .withUrl(hubRoute)
    .withHubProtocol(protocol)
    .build();

let quizId = document.location.href.split('/')[document.location.href.split('/').length - 1];

let results = new ResultViewModel();
results.quizId = quizId;

function updateUserCount(connection: ConnectionViewModel) {
    let usersElement = document.getElementById("users");
    usersElement.innerHTML = `${connection.counter} 👥`;
}

connection.on('Connected', function (connection: ConnectionViewModel) {
    updateUserCount(connection);
});

connection.on('Disconnected', function (connection: ConnectionViewModel) {
    updateUserCount(connection);
});

connection.on('Results', function (r: ResultViewModel) {
    let data = "Results received: " + new Date().toLocaleTimeString();
    console.log(data);
    console.log(r);

    results = r;
    renderQuizResults(results);
});

connection.onclose(function (e: any) {
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
            .catch(function (err: any) {
                console.log("GetResults submission error");
                console.log(err);
            });
    })
    .catch(function (err: any) {
        console.log("SignalR error");
        console.log(err);

        console.log(err);
    });

function getQuestionTitle(results: ResultViewModel) {
    let title = "Results";
    if (results.results.length > 0) {
        title = results.results[0].questionTitle;
    }
    return title;
}

function renderQuizResults(results: ResultViewModel) {

    let resultsTitleElement = document.getElementById("resultsTitle") as HTMLElement;
    resultsTitleElement.innerText = getQuestionTitle(results);

    let resultQuestion = new ResultQuestionViewModel();
    if (results.results.length > 0) {
        resultQuestion = results.results[0];
    }

    let responsesElement = document.getElementById("responses");
    responsesElement.innerHTML = `${results.responses} 📝`;

    let svg = d3.select("svg");
    svg.selectAll("*").remove();
    let containerElement = document.getElementById("containerElement") as HTMLDivElement;
    svg.attr("width", Math.min(containerElement.clientWidth, window.innerWidth * 0.8));
    svg.attr("height", Math.min(containerElement.clientHeight, window.innerHeight * 0.8));
    let margin = {
        top: 20,
        right: 20,
        bottom: 30,
        left: 50
    };
    let width = +svg.attr("width") - margin.left - margin.right;
    let height = +svg.attr("height") - margin.top - margin.bottom;
    let g = svg.append("g").attr("transform", "translate(" + margin.left + "," + margin.top + ")");
    let x = d3.scaleBand()
        .rangeRound([0, width])
        .padding(0.1);

    let y = d3.scaleLinear()
        .rangeRound([height, 0]);

    x.domain(resultQuestion.answers.map(function (d: ResultQuestionAnswerViewModel) {
        return d.name;
    }));
    y.domain([0, d3.max(resultQuestion.answers, function (d: ResultQuestionAnswerViewModel) {
        return Number(d.count);
    })]);

    g.append("g")
        .attr("transform", "translate(0," + height + ")")
        .call(d3.axisBottom(x))

    g.append("g")
        .call(d3.axisLeft(y))
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
        .attr("x", function (d: any) {
            return x(d.name);
        })
        .attr("y", function (d: any) {
            return y(Number(d.count));
        })
        .attr("width", x.bandwidth())
        .attr("height", function (d: any) {
            return height - y(Number(d.count));
        });
}

window.addEventListener('resize', () => {
    console.log("resize");
    renderQuizResults(results);
});

renderQuizResults(results);
