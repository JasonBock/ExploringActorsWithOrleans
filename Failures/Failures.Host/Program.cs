using Failures.Grains;
using Orleans;
using Orleans.ApplicationParts;
using Orleans.Hosting;
using Orleans.Runtime.Configuration;
using Orleans.Serialization;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Failures.Host
{
	public static class Program
	{
		public static async Task Main(string[] args)
		{
			await Console.Out.WriteLineAsync($"Orleans silo is starting...");

			var configuration = ClusterConfiguration.LocalhostPrimarySilo();
			//configuration.Globals.FallbackSerializationProvider = typeof(ILBasedSerializer).GetTypeInfo();

			var builder = new SiloHostBuilder()
				.UseConfiguration(configuration)
				.ConfigureApplicationParts(manager => manager.AddApplicationPart(typeof(CalculatorGrain).Assembly));

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
