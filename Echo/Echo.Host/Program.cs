using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using System;
using System.Threading.Tasks;

namespace Echo.Host
{
	public static class Program
	{
		private const string SiloName = "EchoSilo";

		public static async Task Main(string[] args)
		{
			await Console.Out.WriteLineAsync($"{Program.SiloName} is starting...");

			var siloConfiguration = ClusterConfiguration.LocalhostPrimarySilo();
			siloConfiguration.Defaults.TraceFilePattern = null;
			siloConfiguration.Defaults.TraceToConsole = false;

			var silo = new SiloHost(Program.SiloName, siloConfiguration);
			silo.InitializeOrleansSilo();
			silo.StartOrleansSilo();

			await Console.Out.WriteLineAsync($"{Program.SiloName} is available.");
			await Console.Out.WriteLineAsync($"Press Enter to shutdown {Program.SiloName}...");
			await Console.In.ReadLineAsync();

			silo.ShutdownOrleansSilo();
		}
	}
}
