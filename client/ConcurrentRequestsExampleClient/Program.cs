using ConcurrentRequestsExampleClient;
using System.Threading.Tasks;
using System;

var randoService = new RandomNumberService(url: "https://localhost:12345", maxConcurrentRequests: 4,
    timeoutSeconds: 5);

for (int i = 0; i < 15; i++)
{
    Task.Run(async () =>
    {
        Console.WriteLine($"Requesting random number ");
        Console.WriteLine(await randoService.GetRandomNumber());
    });
}

Console.ReadLine();