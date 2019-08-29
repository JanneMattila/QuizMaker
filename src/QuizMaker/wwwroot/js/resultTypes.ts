export class ResultQuestionAnswerViewModel {
    name: string;
    count: number;
}

export class ResultQuestionViewModel {
    questionId: string;
    questionTitle: string;

    answers: Array<ResultQuestionAnswerViewModel>;

    constructor() {
        this.answers = [];
    }
}

export class ResultViewModel {
    quizId: string;
    quizTitle: string;

    responses: number;

    results: Array<ResultQuestionViewModel>;

    constructor() {
        this.results = [];
    }
}
