using Echo.Contracts;
using Echo.Contracts.Messages;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using System;
using System.Threading.Tasks;

namespace Echo.Client
{
	public static class Program
	{
		private static readonly TimeSpan Retry = TimeSpan.FromSeconds(1);
		private const int RetryAttempts = 5;
		private const string TerminationMessage = "STOP";

		public static async Task Main(string[] args)
		{
			await Console.Out.WriteLineAsync("Client is connecting...");

			var client = await Program.StartClientWithRetries();

			await Console.Out.WriteLineAsync("Client has connected.");
			await Console.Out.WriteLineAsync("Enter your message in the following format: {Repeat},{Message}.");
			await Console.Out.WriteLineAsync("Example: 3,Hello");
			await Console.Out.WriteLineAsync("Enter STOP to shutdown the client.");

			var grainId = Guid.NewGuid();

			while (true)
			{
				var content = await Console.In.ReadLineAsync();

				if(content == Program.TerminationMessage)
				{
					break;
				}

				var (success, message) = await Program.TryCreateMessageAsync(content);

				if (success)
				{
					var echoGrain = client.GetGrain<IEchoGrain>(grainId);
					await echoGrain.SpeakAsync(message);
				}
			}
		}

		private static async Task<IClusterClient> StartClientWithRetries(
			int initializeAttemptsBeforeFailing = Program.RetryAttempts)
		{
			var attempt = 0;
			IClusterClient client;

			while (true)
			{
				try
				{
					var configuration = ClientConfiguration.LocalhostSilo();
					client = new ClientBuilder()
						.UseConfiguration(configuration)
						.AddApplicationPartsFromReferences(typeof(IEchoGrain).Assembly)
						.Build();

					await client.Connect();
					Console.WriteLine("Client successfully connect to silo host");
					break;
				}
				catch (SiloUnavailableException)
				{
					attempt++;

					await Console.Out.WriteLineAsync(
						$"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");

					if (attempt > initializeAttemptsBeforeFailing)
					{
						throw;
					}

					await Task.Delay(Program.Retry);
				}
			}

			return client;
		}

		private static async Task<(bool, EchoSpeakMessage)> TryCreateMessageAsync(string content)
		{
			var parts = content.Split(',');

			if (parts.Length != 2)
			{
				await Console.Out.WriteLineAsync($"Invalid input: {content}");
			}
			else
			{
				if (!uint.TryParse(parts[0], out var repeat))
				{
					await Console.Out.WriteLineAsync($"Invalid repeat value: {parts[0]}");
				}
				else
				{
					return (true, new EchoSpeakMessage { Message = parts[1], Repeat = repeat });
				}
			}

			return (false, null);
		}
	}
}
