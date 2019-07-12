declare var signalR: typeof import("@aspnet/signalr");
import { Quiz, QuizQuestion } from "./quiz";

function addMessage(msg: any) {
    console.log(msg);
}

let protocol = new signalR.JsonHubProtocol();
let hubRoute = "QuizHub";
let connection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Information)
    .withUrl(hubRoute)
    .withHubProtocol(protocol)
    .build();

let quizMandatoryQuestions = [];
let quiz: Quiz;

function createHiddenElement(name: string, value: string): HTMLInputElement {
    let hidden = document.createElement("input") as HTMLInputElement;
    hidden.type = "hidden";
    hidden.name = name;
    hidden.value = value;
    return hidden;
}

function formSubmitCheck() {
    let quizSubmitError = document.getElementById("quizSubmitError");
    quizSubmitError.innerHTML = "";

    if (document.cookie.indexOf(".AspNet.Consent=yes") == -1) {
        quizSubmitError.innerHTML = "Quiz requires that you \"Accept\" privacy consent";
        return false;
    }

    for (let i = 0; i < quizMandatoryQuestions.length; i++) {
        let q = quizMandatoryQuestions[i];
        console.log(q);
        let mandatoryInputElement = document.forms[0].elements[q] as HTMLInputElement;
        let value = mandatoryInputElement.value;

        if (value.length == 0) {
            quizSubmitError.innerHTML = "Please fill the quiz before submitting.";
            return false;
        }
    }
    return true;
}

function createRadioButton(name: string, value: string, text: string): HTMLDivElement {
    let id = `${name}-${value}`;
    let div = document.createElement("div") as HTMLDivElement;
    div.className = "radio";
    let radioButton = document.createElement("input") as HTMLInputElement;
    radioButton.type = "radio";
    radioButton.id = id;
    radioButton.name = name;
    radioButton.value = value;

    let label = document.createElement("label") as HTMLLabelElement;
    label.htmlFor = id;
    label.innerText = text;
    div.appendChild(radioButton);
    div.appendChild(label);
    return div;
}

connection.on('Connected', function (msg: any) {
    let data = "Date received: " + new Date().toLocaleTimeString();
    addMessage(data);
    addMessage(msg);

    let quizForm = document.getElementById("quizForm");
    quizForm.innerHTML = "";

    let question = new QuizQuestion();
    question.questionId = "questionId-333";
    question.questionTitle = "What's your favorite animal?";

    quiz = new Quiz();
    quiz.quizId = "111";
    quiz.quizTitle = "Animal survey";
    quiz.questions.push(question);

    quizForm.appendChild(createHiddenElement("quizId", "111"));
    quizForm.appendChild(createHiddenElement("userId", "222"));
    quizForm.appendChild(createRadioButton("questionId-333", "1", "Text 1 is longer"));
    quizForm.appendChild(createRadioButton("questionId-333", "2", "Text 2"));
    quizForm.appendChild(createRadioButton("questionId-333", "3", "Text 3"));
    quizForm.appendChild(createRadioButton("questionId-333", "4", "Text 4"));
    quizMandatoryQuestions.push("questionId-333");

    let quizSubmit = document.getElementById("quizSubmit");
    quizSubmit.style.display = "";
});

connection.onclose(function (e: any) {
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
    .catch(function (err: any) {
        console.log("SignalR error");
        console.log(err);

        addMessage(err);
    });
