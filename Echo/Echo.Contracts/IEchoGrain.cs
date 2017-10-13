using Echo.Contracts.Messages;
using Orleans;
using System.Threading.Tasks;

namespace Echo.Contracts
{
	public interface IEchoGrain
		: IGrainWithGuidKey
	{
		Task SpeakAsync(EchoSpeakMessage message);
	}
}
