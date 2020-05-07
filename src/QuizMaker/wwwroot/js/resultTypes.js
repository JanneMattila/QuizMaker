System.register([], function (exports_1, context_1) {
    "use strict";
    var ResultQuestionAnswerViewModel, ResultQuestionViewModel, ResultViewModel;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [],
        execute: function () {
            ResultQuestionAnswerViewModel = /** @class */ (function () {
                function ResultQuestionAnswerViewModel() {
                }
                return ResultQuestionAnswerViewModel;
            }());
            exports_1("ResultQuestionAnswerViewModel", ResultQuestionAnswerViewModel);
            ResultQuestionViewModel = /** @class */ (function () {
                function ResultQuestionViewModel() {
                    this.answers = [];
                }
                return ResultQuestionViewModel;
            }());
            exports_1("ResultQuestionViewModel", ResultQuestionViewModel);
            ResultViewModel = /** @class */ (function () {
                function ResultViewModel() {
                    this.results = [];
                }
                return ResultViewModel;
            }());
            exports_1("ResultViewModel", ResultViewModel);
        }
    };
});
//# sourceMappingURL=resultTypes.js.map