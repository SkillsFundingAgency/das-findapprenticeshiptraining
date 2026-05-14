## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# Find Apprenticeship Training

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/SkillsFundingAgency.das-findapprenticeshiptraining?branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2181&branchName=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-findapprenticeshiptraining&metric=alert_status)](https://sonarcloud.io/dashboard?id=SkillsFundingAgency_das-findapprenticeshiptraining)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

## About

[Find Apprenticeship Training Service](https://findapprenticeshiptraining.apprenticeships.education.gov.uk/). The service is for finding training courses and training providers that can deliver the standard you have searched for. 

## 🚀 Installation

### Pre-requisites

- A clone of this repository
- .NET 10 SDK
- Visual Studio or another supported IDE
- Azurite (or Azure Storage Emulator)

### Dependencies

- `SFA.DAS.FAT.MockServer` for local API responses
- Configuration loaded from local storage table data

### Configuration

Create a table named `Configuration` in your local storage emulator and add:

PartitionKey: LOCAL

RowKey: SFA.DAS.FindApprenticeshipTraining.Web_1.0

Data:
```
{
  "FindApprenticeshipTrainingApi": {
    "Key": "test",
    "BaseUrl": "http://localhost:5003/",
    "PingUrl": "http://localhost:5003/"
  },
  "FindApprenticeshipTrainingWeb": {
    "RedisConnectionString": "",
    "DataProtectionKeysDatabase": "",
    "ZendeskSectionId": "1",
    "ZendeskSnippetKey": "test",
    "ZendeskCoBrowsingSnippetKey": "test"
  }
}
```

In `SFA.DAS.FAT.Web`, create `appSettings.Development.json` (if it does not already exist) with:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;",
  "ConfigNames": "SFA.DAS.FindApprenticeshipTraining.Web",
  "Environment": "LOCAL",
  "EnvironmentName": "LOCAL",
  "Version": "1.0",
  "APPLICATIONINSIGHTS_CONNECTION_STRING": "",
  "AllowedHosts": "*",
  "cdn": {
    "url": "https://das-at-frnt-end.azureedge.net"
  }
}
```

Ensure `FindApprenticeshipTrainingApi:BaseUrl` points to the mock server URL.

### Run locally

1. Run `SFA.DAS.FAT.MockServer` (starts on `http://localhost:5003`)
2. Start `SFA.DAS.FAT.Web` (site runs on `https://localhost:5004`)

## Technologies

- .NET 10
- ASP.NET Core
- Redis (configuration dependent)


## Useful URLs


### Courses
https://localhost:5004/courses/24 -> Available for new starts in future date

https://localhost:5004/courses/101 -> Course no longer available

https://localhost:5004/courses/333 -> Regulated Course

https://localhost:5004/courses/102 -> No providers at location

### Providers
https://localhost:5004/courses/102/providers -> No providers at location

### Course Provider Details
https://localhost:5004/courses/102/providers/10000 -> No provider available for course

https://localhost:5004/courses/12313/providers/100002?location=Coventry -> No provider at location

https://localhost:5004/courses/12313/providers/100002?location=Camden -> Provider at location
