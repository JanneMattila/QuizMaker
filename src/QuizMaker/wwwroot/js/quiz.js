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
    var QuizQuestionOption = /** @class */ (function () {
        function QuizQuestionOption() {
        }
        return QuizQuestionOption;
    }());
    exports.QuizQuestionOption = QuizQuestionOption;
    var QuizQuestion = /** @class */ (function () {
        function QuizQuestion() {
            this.options = [];
        }
        return QuizQuestion;
    }());
    exports.QuizQuestion = QuizQuestion;
    var Quiz = /** @class */ (function () {
        function Quiz() {
            this.questions = [];
        }
        return Quiz;
    }());
    exports.Quiz = Quiz;
    var QuizQuestionResponse = /** @class */ (function () {
        function QuizQuestionResponse() {
        }
        return QuizQuestionResponse;
    }());
    exports.QuizQuestionResponse = QuizQuestionResponse;
    var QuizResponse = /** @class */ (function () {
        function QuizResponse() {
            this.responses = [];
        }
        return QuizResponse;
    }());
    exports.QuizResponse = QuizResponse;
});
//# sourceMappingURL=quiz.js.map