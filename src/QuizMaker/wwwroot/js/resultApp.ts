declare const Chart: typeof import("chart.js");
declare const signalR: typeof import("@aspnet/signalr");
import { ConnectionViewModel } from "./quizTypes.js";
import { ResultViewModel, ResultQuestionViewModel } from "./resultTypes.js";

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

const labels = new Array<string>();
const data = {
    labels: [],
    datasets: [{
        backgroundColor: "blue",
        borderColor: "blue",
        data: []
    }]
};

function getQuestionTitle(results: ResultViewModel) {
    let title = "Results";
    if (results.results.length > 0) {
        title = results.results[0].questionTitle;
    }
    return title;
}

function renderQuizResults(results: ResultViewModel, forceDraw: boolean) {

    const resultsTitleElement = document.getElementById("resultsTitle") as HTMLElement;
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

    const responsesElement = document.getElementById("responses");
    responsesElement.innerHTML = `${results.responses} 📝`;

    console.log(labels);
    console.log(forceDraw);
    console.log(data);

    if (forceDraw) {
        const canvas = document.getElementById('quizChart') as HTMLCanvasElement;
        const containerElement = document.getElementById("containerElement") as HTMLDivElement;
        canvas.width = Math.min(containerElement.clientWidth, window.innerWidth * 0.8);
        canvas.height = Math.min(containerElement.clientHeight, window.innerHeight * 0.8);

        const context = canvas.getContext('2d');
        context.clearRect(0, 0, canvas.width, canvas.height);

        const config = {
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
    renderQuizResults(results, true);
});

renderQuizResults(results, true);
