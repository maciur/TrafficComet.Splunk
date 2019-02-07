# TrafficComet.Splunk
Tool for saving information about (request/response) traffic from website written in .Net Core 2.1 in Splunk

## Simple Instalation 
#### Run command in Nuget Package Manager 
```csharp
Install-Package TrafficComet.Splunk.LogWriter 
``` 

#### Edit Startup file like below :
```csharp 
public class Startup
{
  	public IConfiguration Configuration { get; }
  
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddMvc();
		services.AddTrafficCometSplunkLogWriter(Configuration);
	}

	public void Configure(IApplicationBuilder app, IHostingEnvironment env)
	{
		app.UseTrafficComet();
		app.UseMvc();
	}
}
```

#### Add configuration to appsettings.js :
```json 
  "TrafficComet": {
    "Writers": {
      "Splunk": {
        "HostedServices": {
          "Executor": {
            "TasksAtOnce": 10
          }
        },
        "Collectors": {
          "Http": {
            "Url": "URL of Splunk Http Collector",
            "EndPoint": "services/collector",
            "HealthEndPoint": "services/collector/health",
            "Token": "Token from Splunk Http Collector",
            "Index": "Name of index where all logs will be stored",
            "RequestsIndexPrefix": "request",
            "ResponseIndexPrefix": "response"
          },
          "Folder": {
            "Path": "Where Traffic Coment will save json log files when Http Collector is down",
            "RootFolder": "root",
            "RequestsFolder": "requests",
            "ResponsesFolder": "responses"
          }
        }
      }
    },
    "Middleware": {
      "Root": {
        "ApplicationId": "WebApiTest"
      }
    }
  }
``` 
  
### Splunk Configuration
Create a new 3 indexes in Splunk: 
  - root index - index should have same name like value in Splunk.Collectors.Http.Index property from config file, 
  - second index for requests logs - {index-root-name}-{value from RequestsIndexPrefix from config file} 
  - third index for responses logs - {index-root-name}-{value from ResponseIndexPrefix from config file} 
