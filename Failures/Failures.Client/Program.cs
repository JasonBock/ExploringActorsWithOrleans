using Failures.Contracts;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Failures.Client
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
			await Console.Out.WriteLineAsync("Enter your message in the following format: {x},{y}.");
			await Console.Out.WriteLineAsync("Example: 3,5");
			await Console.Out.WriteLineAsync("Enter STOP to shutdown the client.");

			var grainId = Process.GetCurrentProcess().Id.ToString();

			while (true)
			{
				var content = await Console.In.ReadLineAsync();

				if (content == Program.TerminationMessage)
				{
					break;
				}

				var (success, (x, y)) = await Program.TryCreateMessageAsync(content);

				if (success)
				{
					var calculatorGrain = client.GetGrain<ICalculatorGrain>(grainId);
					var result = await calculatorGrain.AddAsync(x, y);
					await Console.Out.WriteLineAsync(
						$"Result of {x} + {y} is {result}");
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
						.AddApplicationPartsFromReferences(typeof(ICalculatorGrain).Assembly)
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

		private static async Task<(bool, (int, int))> TryCreateMessageAsync(string content)
		{
			var parts = content.Split(',');

			if (parts.Length != 2)
			{
				await Console.Out.WriteLineAsync($"Invalid input: {content}");
			}
			else
			{
				if (!int.TryParse(parts[0], out var x))
				{
					await Console.Out.WriteLineAsync($"Invalid value: {parts[0]}");
				}
				else if (!int.TryParse(parts[1], out var y))
				{
					await Console.Out.WriteLineAsync($"Invalid value: {parts[1]}");
				}
				else
				{
					return (true, (x, y));
				}
			}

			return (false, (default, default));
		}
	}
}
