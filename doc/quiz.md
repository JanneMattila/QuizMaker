# Quiz

## Quiz data structure

Here is example quiz format:

```json
{
  "quizId": "123-abc-123",
  "quizTitle": "Animal survey",
  "questions": [
    {
      "questionId": "123-abc-123-1",
      "questionTitle": "What's your favorite animal?",
      "options": [
        {
          "optionId": "123-abc-123-1-1",
          "optionText": "Cat"
        },
        {
          "optionId": "123-abc-123-1-2",
          "optionText": "Dog"
        }
      ],
      "parameters": {
        "multiSelect": false
      }
    }
  ]
}
```

## Quiz  response data structure

Here is example quiz response format:

```json
{
  "userId": "123",
  "quizId": "123-abc-123",
  "responses": [
    {
      "questionId": "123-abc-123-1",
      "optionId": "123-abc-123-1-2"
    }
  ]
}
```
