﻿using System.Net;
using System.Threading.Tasks;
using Echo.Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Echo.Host
{
	public static class Program
	{
		public static Task Main(string[] args) => 
			new HostBuilder()
				 .UseOrleans(builder =>
				 {
					 builder.UseLocalhostClustering()
						.Configure<ClusterOptions>(options =>
						{
							options.ClusterId = "dev";
							options.ServiceId = "Echo.Host";
						})
						.Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
						.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(EchoGrain).Assembly).WithReferences())
						.AddMemoryGrainStorage(name: "ArchiveStorage");
				 })
				 .ConfigureServices(services =>
				 {
					 services.Configure<ConsoleLifetimeOptions>(options =>
					 {
						 options.SuppressStatusMessages = true;
					 });
				 })
				 .ConfigureLogging(builder =>
				 {
					 builder.AddConsole();
				 })
				 .RunConsoleAsync();
	}
}