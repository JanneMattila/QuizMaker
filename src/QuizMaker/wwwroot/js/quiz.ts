declare const signalR: typeof import("@aspnet/signalr");
import { QuizViewModel, ResponseViewModel, ResponseQuestionViewModel, ConnectionViewModel } from "./quizTypes";

function addMessage(msg) {
    console.log(msg);
}

function getUserId() {
    let id = "";
    const QuizUserId = "QuizUserId";
    const searchText = `${QuizUserId}=`;
    let startIndex = document.cookie.indexOf(searchText);
    if (startIndex === -1) {
        try {
            const random = window.crypto.getRandomValues(new Uint32Array(4));
            id = random[0].toString(16) + "-" + random[1].toString(16) + "-" + random[2].toString(16) + "-" + random[3].toString(16);
        } catch (e) {
            console.log("Secure random number generation is not supported.");
            id = Math.floor(Math.random() * 10000000000).toString();
        }

        document.cookie = `${QuizUserId}=${id}; max-age=${3600 * 12}; secure; samesite=strict`;
    }
    else {
        startIndex = startIndex + searchText.length;
        const endIndex = document.cookie.indexOf(";", startIndex);
        if (endIndex === -1) {
            id = document.cookie.substr(startIndex);
        }
        else {
            id = document.cookie.substring(startIndex, endIndex);
        }
    }

    return id;
}

const userId = getUserId();

const protocol = new signalR.JsonHubProtocol();
const hubRoute = "/QuizHub";
const connection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Information)
    .withUrl(hubRoute, { accessTokenFactory: () => { return userId; } })
    .withHubProtocol(protocol)
    .build();

const answeredQuestions = Array<string>();
let quizMandatoryQuestions = Array<string>();
let quiz: QuizViewModel;

function createHiddenElement(name: string, value: string): HTMLInputElement {
    const hidden = document.createElement("input") as HTMLInputElement;
    hidden.type = "hidden";
    hidden.name = name;
    hidden.value = value;
    return hidden;
}

(window as any).formSubmitCheck = () => {
    const quizSubmitError = document.getElementById("quizSubmitError");
    quizSubmitError.innerHTML = "";

    for (let i = 0; i < quizMandatoryQuestions.length; i++) {
        const q = quizMandatoryQuestions[i];
        const mandatoryInputElement = document.forms[0].elements[q] as HTMLInputElement;
        const value = mandatoryInputElement.value;

        if (value.length === 0) {
            quizSubmitError.innerHTML = "Please fill the quiz before submitting.";
            quizSubmitError.scrollIntoView();
            return;
        }
    }

    // Submit form
    const quizResponse = new ResponseViewModel();
    quizResponse.quizId = quiz.quizId;
    quizResponse.userId = userId;
    let allowMultipleResponses = false;
    for (let i = 0; i < quiz.questions.length; i++) {
        const question = quiz.questions[i];
        const inputElement = document.forms[0].elements[question.questionId] as HTMLInputElement;
        const questionResponse = new ResponseQuestionViewModel();
        questionResponse.questionId = question.questionId;

        if (question.parameters.multiSelect) {
            // Checkbox
            const list = (inputElement as unknown) as HTMLInputElement[];
            for (let j = 0; j < list.length; j++) {
                if (list[j].checked) {
                    questionResponse.options.push(list[j].value);
                }
            }
        }
        else {
            // Radio
            questionResponse.options.push(inputElement.value);
        }

        if (question.parameters.allowMultipleResponses) {
            allowMultipleResponses = true;
        }

        quizResponse.responses.push(questionResponse);
    }

    connection.invoke<ResponseViewModel>("QuizResponse", quizResponse)
        .then(function () {
            console.log("QuizResponse submitted: " + quizResponse.quizId);
            if (!allowMultipleResponses) {
                answeredQuestions.push(quizResponse.quizId);
            }

            // Clear quiz
            quiz = undefined;
            quizMandatoryQuestions = [];
        })
        .catch(function (err) {
            console.log("QuizResponse submission error");
            console.log(err);

            addMessage(err);
        });
}

function updateQuizTitle(title: string): HTMLElement {
    const titleElement = document.getElementById("homeLink");
    titleElement.innerHTML = title;
    return titleElement;
}

function createQuestionTitle(title: string): HTMLElement {
    const titleElement = document.createElement("h1") as HTMLElement;
    titleElement.innerText = title;
    return titleElement;
}

function createInput(type: string, name: string, value: string, text: string): HTMLDivElement {
    const id = `${name}-${value}`;
    const div = document.createElement("div") as HTMLDivElement;
    div.className = "quiz-question-option";
    const radioButton = document.createElement("input") as HTMLInputElement;
    radioButton.type = type;
    radioButton.id = id;
    radioButton.name = name;
    radioButton.value = value;

    const label = document.createElement("label") as HTMLLabelElement;
    label.htmlFor = id;
    label.innerText = text;
    div.appendChild(radioButton);
    div.appendChild(label);
    return div;
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

connection.on('Quiz', function (quizReceived: QuizViewModel) {
    const data = "Quiz received: " + new Date().toLocaleTimeString();
    addMessage(data);
    addMessage(quizReceived);

    if (answeredQuestions.indexOf(quizReceived.quizId) !== -1) {
        console.log("This quiz has been answered already");
        quizReceived.quizId = "";
        quizReceived.quizTitle = "Quiz";
        quizReceived.questions = [];
        quizMandatoryQuestions = [];
    }
    else if (quiz !== undefined && quiz.quizId === quizReceived.quizId) {
        console.log("Do not reprocess already open quiz");
        return;
    }

    quiz = quizReceived;

    const quizForm = document.getElementById("quizForm");
    const quizSubmit = document.getElementById("quizSubmit");

    if (quiz.questions.length === 0) {
        quizSubmit.style.display = "none";
        quizForm.innerHTML = "Waiting for available quiz...";
    }
    else {
        quizSubmit.style.display = "";
        quizForm.innerHTML = "";
    }

    updateQuizTitle(quiz.quizTitle);
    quizForm.appendChild(createHiddenElement("quizId", quiz.quizId));
    quizMandatoryQuestions = [];

    for (let i = 0; i < quiz.questions.length; i++) {
        const question = quiz.questions[i];
        quizForm.appendChild(createQuestionTitle(question.questionTitle));

        const type = question.parameters.multiSelect ? "checkbox" : "radio";
        if (!question.parameters.multiSelect) {
            quizMandatoryQuestions.push(question.questionId);
        }

        const inputElements = new Array<HTMLDivElement>();
        for (let j = 0; j < question.options.length; j++) {
            const option = quiz.questions[i].options[j];
            inputElements.push(createInput(type, question.questionId, option.optionId, option.optionText));
        }

        if (question.parameters.randomizeOrder) {
            for (let j = 0; j < inputElements.length; j++) {
                const k = Math.floor(Math.random() * inputElements.length);
                const a = inputElements[j];
                const b = inputElements[k];
                inputElements[j] = b;
                inputElements[k] = a;
            }
        }

        for (let j = 0; j < inputElements.length; j++) {
            quizForm.appendChild(inputElements[j]);
        }
    }
});

connection.onclose(function (e) {
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
    .catch(function (err) {
        console.log("SignalR error");
        console.log(err);

        addMessage(err);
    });
