using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Echo.Client
{
	public static class Program
	{
		public static Task Main() =>
			new HostBuilder()
				.ConfigureServices(services =>
				{
					services.AddSingleton<ClusterClientHostedService>();
					services.AddSingleton<IHostedService>(_ => _.GetService<ClusterClientHostedService>());
					services.AddSingleton(_ => _.GetService<ClusterClientHostedService>().Client);

					services.AddHostedService<EchoClientHostedService>();

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
