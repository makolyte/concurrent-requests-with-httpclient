The purpose of this repo is to help you run example code from this article: https://makolyte.com/csharp-how-to-make-concurrent-requests-with-httpclient/

There are two apps:
1. /client/ - this is a console app that executes concurrent requests to the service stub found in /service/. 
2. /server/ - this is an ASP.NET web API service stub with a single endpoint: GET /RandomNumber/. This runs as a console app, so you can see the requests coming in.

The service stub runs on https://localhost:12345/.

## To run (happy path)
1. Start the ASP.NET web API project in /server/. 
2. Run the console app in /client/

You should see it send 15 requests concurrently without problems.

## To see the error handling
The simplest way to induce errors is to not even run the service stub endpoint. When requests are sent, they'll fail with a "Connection refused" socket error.

1. Don't run the ASP.NET web API. Or stop it if it's already running.
2. Run the console app in /client/

You should see 15 requests concurrently. It throttles, so only 4 are actually sent concurrently. You'll see 1 request fail and trip the circuit. The 3 that were being sent at the same time will fail. The remaining 11 will all not even attempt to send since the circuit is tripped.

### To see timeouts
The simplest way to simulate timeouts is to use toxiproxy. See here: https://makolyte.com/how-to-use-toxiproxy-to-verify-your-code-can-handle-timeouts-and-unavailable-endpoints/

Alternatively, you could add an endpoint to the service stub that does:
`await Task.Delay(-1, HttpContext.RequestAborted)`
