export class QuizQuestionOption {
    optionId: string;
    optionText: string;
}

export class QuizQuestion {
    questionId: string;
    questionTitle: string;

    options: Array<QuizQuestionOption>;

    constructor() {
        this.options = [];
    }
}

export class Quiz {
    quizId: string;
    quizTitle: string;

    questions: Array<QuizQuestion>;

    constructor() {
        this.questions = [];
    }
}

export class QuizQuestionResponse {
    quiestionId: string;
    optionId: string;
}

export class QuizResponse {
    quizId: string;
    userId: string;

    responses: Array<QuizQuestionResponse>;

    constructor() {
        this.responses = [];
    }
}
