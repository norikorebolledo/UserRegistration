# User Registration API 

### Framework
1. C#/ASP.NET Core
2. Websockets API 
3. EntityFramework Core
4. FluentValidation

### Installation

#### Setting Up Database
Before running the project you should apply the database migration first
1. Change the connection string in the appsettings.json under UserRegistration.Api project
2. Open Package Console Manager
3. Make sure you have selected the UserRegisration.Data.Sql project
4. Run Update-Database in the console

#### Setting Up Mail
You can add also a mail settings to send email by configuring it on the appsettings.json
``` json  
"MailSettings": {
    "Mail": null,
    "DisplayName": null,
    "Password": null,
    "Host": null,
    "Port": null
  },
```

but by default it was set to devmode to by pass the sending of email, you can change it in SecuritySettings.DevMode in appsettings.json

``` json
  "SecuritySettings": {
    "DevMode": true
  }

```

### Testing
I have also added Testing UI in the root url of the site, as well as Unit Testing that uses InMemory Database