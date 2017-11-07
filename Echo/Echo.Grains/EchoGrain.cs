using System.Threading.Tasks;
using Echo.Contracts;
using Echo.Contracts.Messages;
using System;
using Orleans;

namespace Echo.Grains
{
	public sealed class EchoGrain
		: Grain, IEchoGrain
	{
		public async Task SpeakAsync(EchoSpeakMessage message)
		{
			message = message ?? throw new ArgumentNullException(nameof(message));

			for(var i = 0; i < message.Repeat; i++)
			{
				await Console.Out.WriteLineAsync(
					$"{i} - {message.Message}");
			}
		}
	}
}
