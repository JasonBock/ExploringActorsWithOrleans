using Echo.Contracts;
using Echo.Contracts.Messages;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Echo.Grains
{
	public sealed class EchoGrain
		: Grain, IEchoGrain
	{
		public async Task SpeakAsync(EchoSpeakMessage message)
		{
			if (message == null) { throw new ArgumentNullException(nameof(message)); }

			for (var i = 0; i < message.Repeat; i++)
			{
				await Console.Out.WriteLineAsync(
					$"{i} - {message.Message}");
			}
		}
	}
}
