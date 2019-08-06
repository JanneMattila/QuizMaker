declare var d3: typeof import("d3");
declare var signalR: typeof import("@aspnet/signalr");
import { QuizResults } from "./quizResultsAppTypes";

let protocol = new signalR.JsonHubProtocol();
let hubRoute = "/QuizResultsHub";
let connection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Information)
    .withUrl(hubRoute)
    .withHubProtocol(protocol)
    .build();

connection.on('Results', function (msg: QuizResults) {
    let data = "Date received: " + new Date().toLocaleTimeString();
    console.log(data);
    console.log(msg);
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
    })
    .catch(function (err: any) {
        console.log("SignalR error");
        console.log(err);

        console.log(err);
    });

let quizId = document.location.href.split('/')[document.location.href.split('/').length - 1];

let results = new QuizResults();
results.quizId = quizId;
results.quizTitle = "Example";
results.values = [
    { name: "Text 1", count: 16 },
    { name: "Text 3", count: 12 },
    { name: "Text 2", count: 5 },
    { name: "Text 4", count: 2 }
];

function renderQuizResults(results: QuizResults) {
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

    x.domain(results.values.map(function (d) {
        return d.name;
    }));
    y.domain([0, d3.max(results.values, function (d: any) {
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
        .data(results.values)
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
