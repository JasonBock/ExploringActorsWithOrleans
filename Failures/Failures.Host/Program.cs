using Failures.Grains;
using Orleans.Hosting;
using Orleans.Runtime.Configuration;
using System;
using System.Threading.Tasks;

namespace Failures.Host
{
	public static class Program
	{
		public static async Task Main(string[] args)
		{
			await Console.Out.WriteLineAsync($"Orleans silo is starting...");

			var configuration = ClusterConfiguration.LocalhostPrimarySilo();
			var builder = new SiloHostBuilder()
				.UseConfiguration(configuration)
				.AddApplicationPartsFromReferences(typeof(CalculatorGrain).Assembly);

			var host = builder.Build();
			await host.StartAsync();

			await Console.Out.WriteLineAsync($"Orleans silo is available.");
			await Console.Out.WriteLineAsync($"Press Enter to terminate...");
			await Console.In.ReadLineAsync();

			await host.StopAsync();
			await Console.Out.WriteLineAsync("Orleans silo is terminated.");
		}
	}
}
