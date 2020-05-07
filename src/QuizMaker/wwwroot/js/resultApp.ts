declare var Chart: typeof import("chart.js");
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
results.responses = 0;

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
    renderQuizResults(results, false);
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

let labels = new Array<string>();
let values = new Array<number>();
let data = {
    labels: [],
    datasets: [{
        backgroundColor: "blue",
        borderColor: "blue",
        data: []
    }]
};

function renderQuizResults(results: ResultViewModel, forceDraw: boolean) {

    let resultsTitleElement = document.getElementById("resultsTitle") as HTMLElement;
    resultsTitleElement.innerText = getQuestionTitle(results);

    data.labels = [];
    data.datasets[0].data = [];

    let resultQuestion = new ResultQuestionViewModel();
    if (results.results.length > 0) {
        resultQuestion = results.results[0];
        for (let i = 0; i < resultQuestion.answers.length; i++) {
            const answer = resultQuestion.answers[i];
            data.labels.push(answer.name);
            data.datasets[0].data.push(answer.count);
        }
    }

    let responsesElement = document.getElementById("responses");
    responsesElement.innerHTML = `${results.responses} 📝`;

    console.log(labels);
    console.log(forceDraw);
    console.log(data);

    if (forceDraw) {
        let canvas = <HTMLCanvasElement>document.getElementById('quizChart');
        const containerElement = document.getElementById("containerElement") as HTMLDivElement;
        canvas.width = Math.min(containerElement.clientWidth, window.innerWidth * 0.8);
        canvas.height = Math.min(containerElement.clientHeight, window.innerHeight * 0.8);

        const context = canvas.getContext('2d');
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

        (<any>window).resultChart = new Chart.Chart(context, config);
    }
    else {
        (<any>window).resultChart.update();
    }
}

window.addEventListener('resize', () => {
    console.log("resize");
    renderQuizResults(results, true);
});

renderQuizResults(results, true);
