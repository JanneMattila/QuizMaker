export class OptionViewModel {
    optionId: string;
    optionText: string;
}

export class ParametersViewModel {
    allowMultipleResponses: boolean;
    multiSelect: boolean;
    randomizeOrder: boolean;
}

export class QuestionViewModel {
    questionId: string;
    questionTitle: string;

    options: Array<OptionViewModel>;
    parameters: ParametersViewModel;

    constructor() {
        this.options = [];
        this.parameters = new ParametersViewModel();
    }
}

export class QuizViewModel {
    quizId: string;
    quizTitle: string;

    questions: Array<QuestionViewModel>;

    constructor() {
        this.questions = [];
    }
}

export class ConnectionViewModel {
    counter: number;
}

export class ResponseQuestionViewModel {
    questionId: string;
    options: Array<string>;

    constructor() {
        this.options = [];
    }
}

export class ResponseViewModel {
    quizId: string;
    userId: string;

    responses: Array<ResponseQuestionViewModel>;

    constructor() {
        this.responses = [];
    }
}
