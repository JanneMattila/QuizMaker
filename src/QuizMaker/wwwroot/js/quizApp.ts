declare var signalR: typeof import("@aspnet/signalr");
import { Quiz, QuizResponse, QuizQuestionResponse } from "./quizAppTypes";

function addMessage(msg: any) {
    console.log(msg);
}

function getUserId() {
    let id = "";
    const QuizUserId = "QuizUserId";
    const searchText = `${QuizUserId}=`;
    let startIndex = document.cookie.indexOf(searchText);
    if (startIndex == -1) {
        try {
            let random = window.crypto.getRandomValues(new Uint32Array(4));
            id = random[0].toString(16) + "-" + random[1].toString(16) + "-" + random[2].toString(16) + "-" + random[3].toString(16);
        } catch (e) {
            console.log("Secure random number generation is not supported.");
            id = Math.floor(Math.random() * 10000000000).toString();
        }

        document.cookie = `${QuizUserId}=${id}; max-age=${3600*12}; secure; samesite=strict`;
    }
    else {
        startIndex = startIndex + searchText.length;
        let endIndex = document.cookie.indexOf(";", startIndex);
        if (endIndex == -1) {
            id = document.cookie.substr(startIndex);
        }
        else {
            id = document.cookie.substring(startIndex, endIndex);
        }
    }

    return id;
}

let userId = getUserId();

let protocol = new signalR.JsonHubProtocol();
let hubRoute = "/QuizHub";
let connection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Information)
    .withUrl(hubRoute, { accessTokenFactory: () => { return userId; }})
    .withHubProtocol(protocol)
    .build();

let quizMandatoryQuestions = Array<string>();
let quiz: Quiz;

function createHiddenElement(name: string, value: string): HTMLInputElement {
    let hidden = document.createElement("input") as HTMLInputElement;
    hidden.type = "hidden";
    hidden.name = name;
    hidden.value = value;
    return hidden;
}

(<any>window).formSubmitCheck = () => {
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

    // Submit form
    let quizResponse = new QuizResponse();
    quizResponse.quizId = quiz.quizId;
    quizResponse.userId = userId;
    for (let i = 0; i < quiz.questions.length; i++) {
        let question = quiz.questions[i];
        let inputElement = document.forms[0].elements[question.questionId] as HTMLInputElement;
        let value = inputElement.value;

        let questionResponse = new QuizQuestionResponse();
        questionResponse.questionId = question.questionId;
        questionResponse.options.push(value);

        quizResponse.responses.push(questionResponse);
    }

    connection.invoke<QuizResponse>("QuizResponse", quizResponse)
        .then(function () {
            console.log("QuizResponse submitted");
        })
        .catch(function (err: any) {
            console.log("QuizResponse submission error");
            console.log(err);

            addMessage(err);
        });;
    return false;
}

function updateQuizTitle(title: string): HTMLElement {
    let titleElement = document.getElementById("homeLink");
    titleElement.innerHTML = title;
    return titleElement;
}

function createQuestionTitle(title: string): HTMLElement {
    let titleElement = document.createElement("h4") as HTMLElement;
    titleElement.innerText = title;
    return titleElement;
}

function createRadioButton(name: string, value: string, text: string): HTMLDivElement {
    let id = `${name}-${value}`;
    let div = document.createElement("div") as HTMLDivElement;
    div.className = "quiz-question-option";
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

    //let quizForm = document.getElementById("quizForm");
    //quizForm.innerHTML = "";

    //let question = new QuizQuestion();
    //question.questionId = "questionId-333";
    //question.questionTitle = "What's your favorite animal?";

    //for (let i = 1; i < 5; i++) {
    //    let option = new QuizQuestionOption();
    //    option.optionId = i.toString();
    //    option.optionText = `Text for option ${i}`;

    //    question.options.push(option);
    //}

    //quiz = new Quiz();
    //quiz.quizId = "111";
    //quiz.quizTitle = "Animal survey";
    //quiz.questions.push(question);

    //quizForm.appendChild(createHiddenElement("quizId", "111"));
    //quizForm.appendChild(createHiddenElement("userId", "222"));
    //quizForm.appendChild(createRadioButton("questionId-333", "1", "Text 1 is longer"));
    //quizForm.appendChild(createRadioButton("questionId-333", "2", "Text 2"));
    //quizForm.appendChild(createRadioButton("questionId-333", "3", "Text 3"));
    //quizForm.appendChild(createRadioButton("questionId-333", "4", "Text 4"));
    //quizMandatoryQuestions.push("questionId-333");

    //let quizSubmit = document.getElementById("quizSubmit");
    //quizSubmit.style.display = "";
});

connection.on('Disconnected', function (msg: any) {
    let data = "Disconnected: " + new Date().toLocaleTimeString();
    addMessage(data);
});

connection.on('Quiz', function (quizReceived: Quiz) {
    let data = "Quiz received: " + new Date().toLocaleTimeString();
    addMessage(data);
    addMessage(quizReceived);

    if (quiz != null && quiz.quizId == quizReceived.quizId) {
        // Do not reprocess same question
        return;
    }

    quiz = quizReceived;

    let quizForm = document.getElementById("quizForm");
    let quizSubmit = document.getElementById("quizSubmit");

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

    for (let i = 0; i < quiz.questions.length; i++) {
        let question = quiz.questions[i];
        quizForm.appendChild(createQuestionTitle(question.questionTitle));

        quizMandatoryQuestions.push(question.questionId);

        for (let j = 0; j < question.options.length; j++) {
            let option = quiz.questions[i].options[j];
            quizForm.appendChild(createRadioButton(question.questionId, option.optionId, option.optionText));
        }
    }
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
