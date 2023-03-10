The purpose of this repo is to help you run example code from this article: https://makolyte.com/csharp-how-to-make-concurrent-requests-with-httpclient/

## Client and web API

There are two apps:
1. Client-side console app. This sends concurrent requests to the web API.
2. Web API. This has one endpoint (GET /RandomNumber) and listens on https://localhost:12345/. 

The service stub runs on https://localhost:12345/.

## To run

1. Start the ASP.NET web API project in /WebApi/. 
2. Run the console app in /client/

You'll see it start sending concurrent requests. The max concurrency is set to 4.

By default, there are two types of responses:
1. A random number.
2. An error response 25% of the time. This is test the client's resiliency!

You'll see a random request fail and trip the circuit. After that, the remaining requests will fail. 

## To see timeouts

The simplest way to induce timeouts on the client-side is to put a delay in the Web API. For example: 
`await Task.Delay(-1, HttpContext.RequestAborted)`

Another way is to use toxiproxy. See here: https://makolyte.com/how-to-use-toxiproxy-to-verify-your-code-can-handle-timeouts-and-unavailable-endpoints/
