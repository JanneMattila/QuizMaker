export class OptionViewModel {
    optionId: string;
    optionText: string;
}

export class QuestionViewModel {
    questionId: string;
    questionTitle: string;

    options: Array<OptionViewModel>;

    constructor() {
        this.options = [];
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
