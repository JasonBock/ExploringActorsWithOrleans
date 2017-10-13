namespace Echo.Contracts.Messages
{
	public sealed class EchoSpeakMessage
	{
		public string Message { get; set; }
		public uint Repeat { get; set; }
	}
}
