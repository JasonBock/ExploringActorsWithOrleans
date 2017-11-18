using Orleans;
using System.Threading.Tasks;

namespace Failures.Contracts
{
	public interface ICalculatorGrain
		: IGrainWithStringKey
	{
		Task<int> AddAsync(int x, int y);
	}
}
