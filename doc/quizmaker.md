# Quiz Maker

## Instructions

### Working with 'quizmaker'

#### How to create image locally

```batch
# Build container image
docker build -t quizmaker:latest .

# Run container using command
docker run --rm -p "20080:80" -p "20443:443" `
  -e SignalRConnectionString="<put signalr conn string here>" `
  -e StorageConnectionString="<put storage conn string here>" `
  -e AzureAD__ClientId="<put AzureAD ApplicationId here>" `
  quizmaker:latest
``` 
