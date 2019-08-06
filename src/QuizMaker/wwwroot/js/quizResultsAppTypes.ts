export class QuizResultsRow {
    name: string;
    count: number;
}

export class QuizResults {
    quizId: string;
    quizTitle: string;
    values: Array<QuizResultsRow>;

    constructor() {
        this.values = [];
    }
}
