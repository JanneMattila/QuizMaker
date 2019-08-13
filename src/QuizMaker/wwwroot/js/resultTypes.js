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
    var ResultQuestionAnswerViewModel = /** @class */ (function () {
        function ResultQuestionAnswerViewModel() {
        }
        return ResultQuestionAnswerViewModel;
    }());
    exports.ResultQuestionAnswerViewModel = ResultQuestionAnswerViewModel;
    var ResultQuestionViewModel = /** @class */ (function () {
        function ResultQuestionViewModel() {
            this.answers = [];
        }
        return ResultQuestionViewModel;
    }());
    exports.ResultQuestionViewModel = ResultQuestionViewModel;
    var ResultViewModel = /** @class */ (function () {
        function ResultViewModel() {
            this.results = [];
        }
        return ResultViewModel;
    }());
    exports.ResultViewModel = ResultViewModel;
});
//# sourceMappingURL=resultTypes.js.map