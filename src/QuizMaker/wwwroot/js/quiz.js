System.register(["./quizTypes.js"], function (exports_1, context_1) {
    "use strict";
    var quizTypes_js_1, userId, protocol, hubRoute, connection, answeredQuestions, quizMandatoryQuestions, quiz;
    var __moduleName = context_1 && context_1.id;
    function addMessage(msg) {
        console.log(msg);
    }
    function getUserId() {
        var id = "";
        var QuizUserId = "QuizUserId";
        var searchText = "".concat(QuizUserId, "=");
        var startIndex = document.cookie.indexOf(searchText);
        if (startIndex === -1) {
            try {
                var random = window.crypto.getRandomValues(new Uint32Array(4));
                id = random[0].toString(16) + "-" + random[1].toString(16) + "-" + random[2].toString(16) + "-" + random[3].toString(16);
            }
            catch (e) {
                console.log("Secure random number generation is not supported.");
                id = Math.floor(Math.random() * 10000000000).toString();
            }
            document.cookie = "".concat(QuizUserId, "=").concat(id, "; max-age=").concat(3600 * 12, "; secure; samesite=strict");
        }
        else {
            startIndex = startIndex + searchText.length;
            var endIndex = document.cookie.indexOf(";", startIndex);
            if (endIndex === -1) {
                id = document.cookie.substr(startIndex);
            }
            else {
                id = document.cookie.substring(startIndex, endIndex);
            }
        }
        return id;
    }
    function createHiddenElement(name, value) {
        var hidden = document.createElement("input");
        hidden.type = "hidden";
        hidden.name = name;
        hidden.value = value;
        return hidden;
    }
    function updateQuizTitle(title) {
        var titleElement = document.getElementById("homeLink");
        titleElement.innerHTML = title;
        return titleElement;
    }
    function createQuestionTitle(title) {
        var titleElement = document.createElement("h1");
        titleElement.innerText = title;
        return titleElement;
    }
    function createInput(type, name, value, text) {
        var id = "".concat(name, "-").concat(value);
        var div = document.createElement("div");
        div.className = "quiz-question-option";
        var radioButton = document.createElement("input");
        radioButton.type = type;
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
    function updateUserCount(connectionModel) {
        var usersElement = document.getElementById("users");
        usersElement.innerHTML = "".concat(connectionModel.counter, " \uD83D\uDC65");
    }
    return {
        setters: [
            function (quizTypes_js_1_1) {
                quizTypes_js_1 = quizTypes_js_1_1;
            }
        ],
        execute: function () {
            userId = getUserId();
            protocol = new signalR.JsonHubProtocol();
            hubRoute = "/QuizHub";
            connection = new signalR.HubConnectionBuilder()
                .configureLogging(signalR.LogLevel.Information)
                .withUrl(hubRoute, { accessTokenFactory: function () { return userId; } })
                .withAutomaticReconnect()
                .withHubProtocol(protocol)
                .build();
            answeredQuestions = Array();
            quizMandatoryQuestions = Array();
            window.formSubmitCheck = function () {
                var quizSubmitError = document.getElementById("quizSubmitError");
                quizSubmitError.innerHTML = "";
                for (var i = 0; i < quizMandatoryQuestions.length; i++) {
                    var q = quizMandatoryQuestions[i];
                    var mandatoryInputElement = document.forms[0].elements[q];
                    var value = mandatoryInputElement.value;
                    if (value.length === 0) {
                        quizSubmitError.innerHTML = "Please fill the quiz before submitting.";
                        quizSubmitError.scrollIntoView();
                        return;
                    }
                }
                // Submit form
                var quizResponse = new quizTypes_js_1.ResponseViewModel();
                quizResponse.quizId = quiz.quizId;
                quizResponse.userId = userId;
                var allowMultipleResponses = false;
                for (var i = 0; i < quiz.questions.length; i++) {
                    var question = quiz.questions[i];
                    var inputElement = document.forms[0].elements[question.questionId];
                    var questionResponse = new quizTypes_js_1.ResponseQuestionViewModel();
                    questionResponse.questionId = question.questionId;
                    if (question.parameters.multiSelect) {
                        // Checkbox
                        var list = inputElement;
                        for (var j = 0; j < list.length; j++) {
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
                connection.invoke("QuizResponse", quizResponse)
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
            };
            connection.on('Connected', function (connectionModel) {
                updateUserCount(connectionModel);
            });
            connection.on('Disconnected', function (connectionModel) {
                updateUserCount(connectionModel);
            });
            connection.on('Quiz', function (quizReceived) {
                var data = "Quiz received: " + new Date().toLocaleTimeString();
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
                var quizForm = document.getElementById("quizForm");
                var quizSubmit = document.getElementById("quizSubmit");
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
                for (var i = 0; i < quiz.questions.length; i++) {
                    var question = quiz.questions[i];
                    quizForm.appendChild(createQuestionTitle(question.questionTitle));
                    var type = question.parameters.multiSelect ? "checkbox" : "radio";
                    if (!question.parameters.multiSelect) {
                        quizMandatoryQuestions.push(question.questionId);
                    }
                    var inputElements = new Array();
                    for (var j = 0; j < question.options.length; j++) {
                        var option = quiz.questions[i].options[j];
                        inputElements.push(createInput(type, question.questionId, option.optionId, option.optionText));
                    }
                    if (question.parameters.randomizeOrder) {
                        for (var j = 0; j < inputElements.length; j++) {
                            var k = Math.floor(Math.random() * inputElements.length);
                            var a = inputElements[j];
                            var b = inputElements[k];
                            inputElements[j] = b;
                            inputElements[k] = a;
                        }
                    }
                    for (var j = 0; j < inputElements.length; j++) {
                        quizForm.appendChild(inputElements[j]);
                    }
                }
            });
            connection.onclose(function (e) {
                if (e) {
                    addMessage("Connection closed with error: " + e);
                    setTimeout(function () {
                        console.log("Validating the connection");
                        console.log(connection.state);
                        if (connection.state !== signalR.HubConnectionState.Connected) {
                            console.log("Restarting the connection");
                            connection.start();
                        }
                    }, 1500);
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
        }
    };
});
//# sourceMappingURL=quiz.js.map