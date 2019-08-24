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
    var OptionViewModel = /** @class */ (function () {
        function OptionViewModel() {
        }
        return OptionViewModel;
    }());
    exports.OptionViewModel = OptionViewModel;
    var ParametersViewModel = /** @class */ (function () {
        function ParametersViewModel() {
        }
        return ParametersViewModel;
    }());
    exports.ParametersViewModel = ParametersViewModel;
    var QuestionViewModel = /** @class */ (function () {
        function QuestionViewModel() {
            this.options = [];
            this.parameters = new ParametersViewModel();
        }
        return QuestionViewModel;
    }());
    exports.QuestionViewModel = QuestionViewModel;
    var QuizViewModel = /** @class */ (function () {
        function QuizViewModel() {
            this.questions = [];
        }
        return QuizViewModel;
    }());
    exports.QuizViewModel = QuizViewModel;
    var ResponseQuestionViewModel = /** @class */ (function () {
        function ResponseQuestionViewModel() {
            this.options = [];
        }
        return ResponseQuestionViewModel;
    }());
    exports.ResponseQuestionViewModel = ResponseQuestionViewModel;
    var ResponseViewModel = /** @class */ (function () {
        function ResponseViewModel() {
            this.responses = [];
        }
        return ResponseViewModel;
    }());
    exports.ResponseViewModel = ResponseViewModel;
});
//# sourceMappingURL=quizTypes.js.map