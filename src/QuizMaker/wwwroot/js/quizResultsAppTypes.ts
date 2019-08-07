export class QuizQuestionResultsRow {
    name: string;
    count: number;
}

export class QuizQuestionResults {
    questionId: string;
    questionTitle: string;

    answers: Array<QuizQuestionResultsRow>;

    constructor() {
        this.answers = [];
    }
}

export class QuizResults {
    quizId: string;
    quizTitle: string;

    results: Array<QuizQuestionResults>;

    constructor() {
        this.results = [];
    }
}
