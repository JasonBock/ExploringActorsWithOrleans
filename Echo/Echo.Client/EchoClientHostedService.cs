using System;
using System.Threading;
using System.Threading.Tasks;
using Echo.Contracts;
using Echo.Contracts.Messages;
using Microsoft.Extensions.Hosting;
using Orleans;

namespace Echo.Client
{
	public sealed class EchoClientHostedService 
		: IHostedService
	{
		private const string TerminationMessage = "STOP";
		private readonly IClusterClient client;

		public EchoClientHostedService(IClusterClient client) => 
			this.client = client;

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await Console.Out.WriteLineAsync("Client has connected.");
			await Console.Out.WriteLineAsync("Enter your message in the following format: {Repeat},{Message}.");
			await Console.Out.WriteLineAsync("Example: 3,Hello");
			await Console.Out.WriteLineAsync("Enter STOP to shutdown the client.");

			var grainId = Guid.NewGuid();

			while (true)
			{
				var content = await Console.In.ReadLineAsync();

				if (content == EchoClientHostedService.TerminationMessage)
				{
					break;
				}

				var (success, message) = await EchoClientHostedService.TryCreateMessageAsync(content);

				if (success)
				{
					var echoGrain = this.client.GetGrain<IEchoGrain>(grainId);
					await echoGrain.SpeakAsync(message);
				}
			}
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

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
