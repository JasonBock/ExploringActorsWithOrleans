using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace Echo.Client
{
	// Lifted from https://github.com/dotnet/orleans/blob/master/Samples/3.0/HelloWorld/src/OrleansClient/ClusterClientHostedService.cs
	public sealed class ClusterClientHostedService 
		: IHostedService
	{
		private readonly ILogger<ClusterClientHostedService> logger;

		public ClusterClientHostedService(ILogger<ClusterClientHostedService> logger, ILoggerProvider loggerProvider)
		{
			this.logger = logger;
			this.Client = new ClientBuilder()
				 .UseLocalhostClustering()
				 .ConfigureLogging(builder => builder.AddProvider(loggerProvider))
				 .Build();
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			var attempt = 0;
			var maxAttempts = 100;
			var delay = TimeSpan.FromSeconds(1);
			
			return this.Client.Connect(async error =>
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return false;
				}

				if (++attempt < maxAttempts)
				{
					this.logger.LogWarning(error,
						 "Failed to connect to Orleans cluster on attempt {@Attempt} of {@MaxAttempts}.",
						 attempt, maxAttempts);

					try
					{
						await Task.Delay(delay, cancellationToken);
					}
					catch (OperationCanceledException)
					{
						return false;
					}

					return true;
				}
				else
				{
					this.logger.LogError(error,
						 "Failed to connect to Orleans cluster on attempt {@Attempt} of {@MaxAttempts}.",
						 attempt, maxAttempts);

					return false;
				}
			});
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			try
			{
				await this.Client.Close();
			}
			catch (OrleansException error)
			{
				this.logger.LogWarning(error, "Error while gracefully disconnecting from Orleans cluster. Will ignore and continue to shutdown.");
			}
		}

		public IClusterClient Client { get; }
	}
}