# das-findapprenticeshiptraining


[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/SkillsFundingAgency.das-findapprenticeshiptraining?branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2181&branchName=master)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-findapprenticeshiptraining&metric=alert_status)](https://sonarcloud.io/dashboard?id=SkillsFundingAgency_das-findapprenticeshiptraining)

## Requirements

DotNet Core 3.1 and any supported IDE for DEV running.

Azure Storage Emulator

## About

[Find Apprenticeship Training Service](https://findapprenticeshiptraining.apprenticeships.education.gov.uk/). The service is for finding training courses and training providers that can deliver the standard you have searched for. 

## Local running

You must have the Azure Storage emulator running, and in that a table created called `Configuration` in that table add the following:

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

In the SFA.DAS.FAT.Web project, if it does not exist already, add appSettings.Development.json file with following content:
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
  "Version": "1.0",
  "APPLICATIONINSIGHTS_CONNECTION_STRING": "",
  "AllowedHosts": "*",
  "cdn": {
    "url": "https://das-at-frnt-end.azureedge.net"
  }
}
```


The important part of the configuration is making sure that your BaseUrl is pointed to the MockServer url

You are able to run the website by doing the following:
* Run the console app ```SFA.DAS.FAT.MockServer``` - this will create a mock server on http://localhost:5003
* Start the web solution ```SFA.DAS.FAT.Web```


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
