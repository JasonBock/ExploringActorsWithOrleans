using Failures.Contracts;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Failures.Grains
{
	public sealed class CalculatorGrain
		: Grain, ICalculatorGrain
	{
		private int callCount;

		public Task<int> AddAsync(int x, int y)
		{
			this.callCount++;
			Console.Out.WriteLine(
				$"Object {this.GetPrimaryKeyString()} has been called {this.callCount} time(s).");

			if (x % 2 == 0) { throw new ArgumentException("Argument cannot be even.", nameof(x)); }
			if (y % 2 == 0) { throw new ArgumentException("Argument cannot be even.", nameof(y)); }

			return Task.FromResult(x + y);
		}
	}
}
