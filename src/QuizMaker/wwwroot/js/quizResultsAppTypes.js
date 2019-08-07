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
    var QuizQuestionResultsRow = /** @class */ (function () {
        function QuizQuestionResultsRow() {
        }
        return QuizQuestionResultsRow;
    }());
    exports.QuizQuestionResultsRow = QuizQuestionResultsRow;
    var QuizQuestionResults = /** @class */ (function () {
        function QuizQuestionResults() {
            this.answers = [];
        }
        return QuizQuestionResults;
    }());
    exports.QuizQuestionResults = QuizQuestionResults;
    var QuizResults = /** @class */ (function () {
        function QuizResults() {
            this.results = [];
        }
        return QuizResults;
    }());
    exports.QuizResults = QuizResults;
});
//# sourceMappingURL=quizResultsAppTypes.js.map