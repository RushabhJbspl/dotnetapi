### Appsetting File Configurations
1) Set SqlServerConnectionString
2) Replace <APIURL> to your API Url
3) Change Google Authentication ClientId and ClientSecret
4) Configure Redis and Replace <RedisHost> to redis server Host URL
5) Set <DefaultPair> for Front 
6) Replace <FrontURL> to Front App Url
7) If You have integarte Azure SignalR then Change AzureSignalR key to 'True' and set Azure SignalR ConnectionString.
   - Reference Link (https://docs.microsoft.com/en-us/azure/azure-signalr/signalr-quickstart-dotnet-core#create-an-azure-signalr-resource)



### Redis Configurations
=> Steps for Redis Server in Vertual Machine

1. Download redis setup for windows (https://github.com/dmajkic/redis/downloads)
2. After download, Find "redis-server.exe" and run this file
3. Then Open command prompt and type "npm i redis-commander -g" and press enter
4. after successfully install type "redis-commander" and press enter
5. now your redis server started