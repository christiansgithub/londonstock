# London Stock API 

 
 

## Overview 

Start using `dotnet run` on the the LondonStock.Api project or running from an IDE 

 
 

Swagger docs reachable at swagger/index.html (if browser doesn't auto-launch) 

 
 

It uses Entity Framework with an in-memory SQLite database for ease of running but for production use a database such as Azure SQL Database could be suitable, especially given the many scaling options available. Using Entity Framework as an ORM gives some flexibility in being able to change the database backing, at least at an early stage. 

 
 

It uses a very simple data model of one table to avoid overcomplicating anything for the initial MVP. 

 

The tests are making use of WebApplicationFactory for more functional style tests and as the database is currently SQLite it also uses an in-memory database for these tests. 

 
 

### Assumptions 

There are various functional requirements that need to be clarified for any production release such as 

1. Calculation of the current stock price. For this exercise it uses the price of the latest trade (creation date). 

2. Is the price provided the overall price of the trade or for each share? For this exercise it assumes it is the price per share. 

3. Can the price or number of trades ever be zero? For this exercise it assumes both must be more than zero. 

 
 

In terms of scale, a few assumptions can be made about how many trades the system could expect to receive by looking at [some average daily London Stock Exchange trade data](https://www.statista.com/statistics/325326/uk-lse-average-daily-trades/), which points to roughly an average of 500,000 trades a day in July 2023, although there were as many as 1,000,000 in early 2021, so this could be used as a base. 

 
 

Given 1,000,000 trades a day with the exchange being open for 8.5 hours a day, an estimate is 2000 a minute or 33 a second, so the application would need to be able to handle at least this number of write requests, plus any expected number of reads. 

 
 

For fun I did set it up to use a regular disk-based SQLite database and load tested it locally with around 200 writes per second and read 500 requests per second (simultaneously for 700 requests per second total), which it handled without issue, so even with a basic database the writes and reads at this rate seem no issue. That said, the data model is quite simple and there is little logic and little data, so this could be non-representative. 

 
 

Running the API within a container in something like Azure Container Apps would allow for scaling of the app to however many instances are needed. Scaling based on the endpoints used would be an issue as currently both the write and read endpoints are in the same app. These could be split if needed, to allow more fine-grained scaling. 

 

Azure functions could be used in place of something like a container app, although maintaining a lot of apps also has a downside in respect to maintenance. 

 

The database could become a bottleneck, but there are various things such as sharding or caching that could help with this. 

 
 

## Enhancements 

There are various non-functional requirements that could be added such as logging any useful information such as any errors to somewhere it can be read and queried. Adding Open Telemetry support to enable ingestion of logs, traces, and metrics would be a good start. This would enable fault finding but also allow for monitoring of system performance and health. 

 
 

There is also currently no authentication or authorization enabled, so this would be required before going to production to avoid allowing any unauthorised requests. 

 
 

An alternative solution to writing trades to the database could be some kind of queue, which could allow for better scaling of writes. If the REST API published a message to some message broker (e.g. Azure Service Bus), it could then enable a quicker response from the REST API to the calling client. It could then also allow for things such as batching of writes or allow other services to subscribe to the events. While this does increase flexibility, it also brings more complexity to the system. 

 
 

While this service is a REST API, if the service is for internal use only and there’s no requirement for a REST endpoint, then the POST endpoint could potentially be replaced directly with raising some kind of trade event. 

 
 

There also seems room for caching reads, as it seems likely that there will be many reads to get the latest price of a stock. There are cloud services that could potentially handle this, although caching brings complexity such as ensuring consistency of responses and cache invalidation when a new trade for a particular stock is received. A better understanding of how the service is used would be required to know whether a caching strategy would be beneficial. 

 

 

 