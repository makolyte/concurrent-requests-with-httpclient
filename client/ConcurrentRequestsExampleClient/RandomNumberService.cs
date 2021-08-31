using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentRequestsExampleClient
{
    public class RandomNumberService
	{
		private readonly HttpClient HttpClient;
		private readonly string GetRandomNumberUrl;
		private SemaphoreSlim semaphore;
		private long circuitStatus;
		private const long CLOSED = 0;
		private const long TRIPPED = 1;
		public string UNAVAILABLE = "Unavailable";

		public RandomNumberService(string url, int maxConcurrentRequests, int timeoutSeconds)
		{
			GetRandomNumberUrl = $"{url}/RandomNumber/";

			HttpClient = new HttpClient();
			HttpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
			SetMaxConcurrency(url, maxConcurrentRequests);
			semaphore = new SemaphoreSlim(maxConcurrentRequests);

			circuitStatus = CLOSED;
		}

		private void SetMaxConcurrency(string url, int maxConcurrentRequests)
		{
			ServicePointManager.FindServicePoint(new Uri(url)).ConnectionLimit = maxConcurrentRequests;
		}

		public void CloseCircuit()
		{
			if (Interlocked.CompareExchange(ref circuitStatus, CLOSED, TRIPPED) == TRIPPED)
			{
				Console.WriteLine("Closed circuit");
			}
		}
		private void TripCircuit(string reason)
		{
			if (Interlocked.CompareExchange(ref circuitStatus, TRIPPED, CLOSED) == CLOSED)
			{
				Console.WriteLine($"Tripping circuit because: {reason}");
			}
		}
		private bool IsTripped()
		{
			return Interlocked.Read(ref circuitStatus) == TRIPPED;
		}
		public async Task<string> GetRandomNumber()
		{
			try
			{
				await semaphore.WaitAsync();

				if (IsTripped())
				{
					return UNAVAILABLE;
				}

				var response = await HttpClient.GetAsync(GetRandomNumberUrl);

				if (response.StatusCode != HttpStatusCode.OK)
				{
					TripCircuit(reason: $"Status not OK. Status={response.StatusCode}");
					return UNAVAILABLE;
				}

				return await response.Content.ReadAsStringAsync();
			}
			catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
			{
				//Note: If you want to simulate timeouts to see this error handling path, I suggest using toxiproxy.
				//Ref: https://makolyte.com/how-to-use-toxiproxy-to-verify-your-code-can-handle-timeouts-and-unavailable-endpoints/
				Console.WriteLine("Timed out");
				TripCircuit(reason: $"Timed out");
				return UNAVAILABLE;
			}
			catch(HttpRequestException ex) when (ex?.InnerException is SocketException sockEx && sockEx.SocketErrorCode == SocketError.ConnectionRefused)
            {
				//Note: The simplest way to see the error handling in action is to not run the service. 
				//That will result in this error happening, instead of timeout.
				Console.WriteLine("Connection failed.");
				TripCircuit(reason: $"Connection failed");
				return UNAVAILABLE;
            }
			finally
			{
				semaphore.Release();
			}
		}
	}
}
