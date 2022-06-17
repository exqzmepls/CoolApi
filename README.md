# To start

1. Clone Repository
2. Add `appsettings.json` in *CoolApi* folder with following content

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ConnectionStrings": {
    "DebugDb": `your-connection-string`
  },
  "AllowedHosts": "*",
  "JWT": {
    "Secret": `your-secret-key`
  }
}

3. Open sln file and run project
