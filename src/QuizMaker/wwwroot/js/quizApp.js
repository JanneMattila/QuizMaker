(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    function addMessage(msg) {
        console.log(msg);
    }
    var protocol = new signalR.JsonHubProtocol();
    var hubRoute = "QuizHub";
    var connection = new signalR.HubConnectionBuilder()
        .configureLogging(signalR.LogLevel.Information)
        .withUrl(hubRoute)
        .withHubProtocol(protocol)
        .build();
    var quizMandatoryQuestions = [];
    var quiz;
    function createHiddenElement(name, value) {
        var hidden = document.createElement("input");
        hidden.type = "hidden";
        hidden.name = name;
        hidden.value = value;
        return hidden;
    }
    function formSubmitCheck() {
        var quizSubmitError = document.getElementById("quizSubmitError");
        quizSubmitError.innerHTML = "";
        if (document.cookie.indexOf(".AspNet.Consent=yes") == -1) {
            quizSubmitError.innerHTML = "Quiz requires that you \"Accept\" privacy consent";
            return false;
        }
        for (var i = 0; i < quizMandatoryQuestions.length; i++) {
            var q = quizMandatoryQuestions[i];
            console.log(q);
            var mandatoryInputElement = document.forms[0].elements[q];
            var value = mandatoryInputElement.value;
            if (value.length == 0) {
                quizSubmitError.innerHTML = "Please fill the quiz before submitting.";
                return false;
            }
        }
        return true;
    }
    function createRadioButton(name, value, text) {
        var id = name + "-" + value;
        var div = document.createElement("div");
        div.className = "radio";
        var radioButton = document.createElement("input");
        radioButton.type = "radio";
        radioButton.id = id;
        radioButton.name = name;
        radioButton.value = value;
        var label = document.createElement("label");
        label.htmlFor = id;
        label.innerText = text;
        div.appendChild(radioButton);
        div.appendChild(label);
        return div;
    }
    connection.on('Connected', function (msg) {
        var data = "Date received: " + new Date().toLocaleTimeString();
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
    connection.on('Disconnected', function (msg) {
        var data = "Disconnected: " + new Date().toLocaleTimeString();
        addMessage(data);
    });
    connection.on('Quiz', function (quizReceived) {
        var data = "Quiz received: " + new Date().toLocaleTimeString();
        addMessage(data);
        addMessage(quizReceived);
        if (quiz != null && quiz.quizId == quizReceived.quizId) {
            // Do not reprocess question
            return;
        }
        quiz = quizReceived;
        var quizForm = document.getElementById("quizForm");
        quizForm.innerHTML = "";
        quizForm.appendChild(createHiddenElement("quizId", quiz.quizId));
        for (var i = 0; i < quiz.questions.length; i++) {
            var question = quiz.questions[i];
            quizMandatoryQuestions.push(question.questionId);
            for (var j = 0; j < question.options.length; j++) {
                var option = quiz.questions[i].options[j];
                quizForm.appendChild(createRadioButton(question.questionId, option.optionId, option.optionText));
            }
        }
        var quizSubmit = document.getElementById("quizSubmit");
        quizSubmit.style.display = "";
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
});
//# sourceMappingURL=quizApp.js.map