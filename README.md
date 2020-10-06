# QuizMaker

## Build Status

[![Build Status](https://jannemattila.visualstudio.com/jannemattila/_apis/build/status/JanneMattila.QuizMaker?branchName=master)](https://jannemattila.visualstudio.com/jannemattila/_build/latest?definitionId=40&branchName=master)
[![Actions Status](https://github.com/JanneMattila/QuizMaker/workflows/ASP.NET%20Core%20CI/badge.svg)](https://github.com/JanneMattila/QuizMaker/tree/master/.github/workflows)
[![Actions Status](https://github.com/JanneMattila/QuizMaker/workflows/Docker%20Image%20CI/badge.svg)](https://github.com/JanneMattila/QuizMaker/tree/master/.github/workflows)
![Docker Pulls](https://img.shields.io/docker/pulls/jannemattila/quizmaker?style=plastic)
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

### Develop in cloud

#### Use Visual Studio Online

You can clone this repository into [Visual Studio Online](https://online.visualstudio.com/environments/new?name=quizmaker&repo=JanneMattila/QuizMaker)
to start developing this directly in your browser.

**Note**: For now you need execute following commands to get compilation working correctly:

```bash
sudo apt-get install dotnet-sdk-3.0

cd src/QuizMaker; npm install
```

### Develop locally

First clone this to your local machine:
```bash
git clone https://github.com/JanneMattila/QuizMaker.git
```

#### Use Visual Studio

Open `QuizMaker.sln` in Visual Studio. If you don't have 
Visual Studio you can get [Visual Studio Community 2019](https://visualstudio.microsoft.com/free-developer-offers/) for free.

#### Use Visual Studio Code

Open cloned folder in Visual Studio Code. If you don't have 
Visual Studio Code you can get it [here](https://visualstudio.microsoft.com/free-developer-offers/).
