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
    var QuizResultsRow = /** @class */ (function () {
        function QuizResultsRow() {
        }
        return QuizResultsRow;
    }());
    exports.QuizResultsRow = QuizResultsRow;
    var QuizResults = /** @class */ (function () {
        function QuizResults() {
            this.values = [];
        }
        return QuizResults;
    }());
    exports.QuizResults = QuizResults;
});
//# sourceMappingURL=quizResultsAppTypes.js.map