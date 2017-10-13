using Echo.Contracts;
using Echo.Contracts.Messages;
using Orleans;
using Orleans.Runtime.Configuration;
using System;
using System.Threading.Tasks;

namespace Echo.Client
{
	public static class Program
	{
		private const string TerminationMessage = "STOP";

		public static async Task Main(string[] args)
		{
			await Console.Out.WriteLineAsync("Press enter to create client...");
			await Console.In.ReadLineAsync();

			await Console.Out.WriteLineAsync("Connecting client...");

			var configuration = ClientConfiguration.LocalhostSilo();
			configuration.TraceFilePattern = null;
			configuration.TraceToConsole = false;

			var client = new ClientBuilder().UseConfiguration(configuration).Build();
			await client.Connect();

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
