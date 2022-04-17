# QuizMaker

## Build Status

![Build Status](https://dev.azure.com/jannemattila/jannemattila/_apis/build/status/JanneMattila.QuizMaker?branchName=master&stageName=Build)
[![Docker Pulls](https://img.shields.io/docker/pulls/jannemattila/quizmaker?style=plastic)](https://hub.docker.com/r/jannemattila/quizsim)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Introduction

Enhance your presentations or talks with interactive quizzes. QuizMaker solves just that!

You can:
* Show simple Quiz user interface for your audience
* Have simple management views for managing the quizzes
  * Activate quiz, delete responses etc.
* Show result in graphical view which is updated real-time when responses flow in

## Getting started

**Note**: You need *either* [Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator) 
or [Azure Storage Account](https://portal.azure.com/#create/Microsoft.StorageAccount-ARM) 
for data storage during development.

### Develop locally

First clone this to your local machine:

```bash
git clone https://github.com/JanneMattila/QuizMaker.git
```

#### Use Visual Studio

Open `QuizMaker.sln` in Visual Studio. If you don't have 
Visual Studio you can get [Visual Studio Community](https://visualstudio.microsoft.com/free-developer-offers/) for free.

#### Use Visual Studio Code

Open cloned folder in Visual Studio Code. If you don't have 
Visual Studio Code you can get it [here](https://visualstudio.microsoft.com/free-developer-offers/).
