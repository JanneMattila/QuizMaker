System.register([], function (exports_1, context_1) {
    "use strict";
    var OptionViewModel, ParametersViewModel, QuestionViewModel, QuizViewModel, ConnectionViewModel, ResponseQuestionViewModel, ResponseViewModel;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [],
        execute: function () {
            OptionViewModel = /** @class */ (function () {
                function OptionViewModel() {
                }
                return OptionViewModel;
            }());
            exports_1("OptionViewModel", OptionViewModel);
            ParametersViewModel = /** @class */ (function () {
                function ParametersViewModel() {
                }
                return ParametersViewModel;
            }());
            exports_1("ParametersViewModel", ParametersViewModel);
            QuestionViewModel = /** @class */ (function () {
                function QuestionViewModel() {
                    this.options = [];
                    this.parameters = new ParametersViewModel();
                }
                return QuestionViewModel;
            }());
            exports_1("QuestionViewModel", QuestionViewModel);
            QuizViewModel = /** @class */ (function () {
                function QuizViewModel() {
                    this.questions = [];
                }
                return QuizViewModel;
            }());
            exports_1("QuizViewModel", QuizViewModel);
            ConnectionViewModel = /** @class */ (function () {
                function ConnectionViewModel() {
                }
                return ConnectionViewModel;
            }());
            exports_1("ConnectionViewModel", ConnectionViewModel);
            ResponseQuestionViewModel = /** @class */ (function () {
                function ResponseQuestionViewModel() {
                    this.options = [];
                }
                return ResponseQuestionViewModel;
            }());
            exports_1("ResponseQuestionViewModel", ResponseQuestionViewModel);
            ResponseViewModel = /** @class */ (function () {
                function ResponseViewModel() {
                    this.responses = [];
                }
                return ResponseViewModel;
            }());
            exports_1("ResponseViewModel", ResponseViewModel);
        }
    };
});
//# sourceMappingURL=quizTypes.js.map