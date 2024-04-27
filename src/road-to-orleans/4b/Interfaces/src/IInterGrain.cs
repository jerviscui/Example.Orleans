using System.Threading.Tasks;
using Orleans;

namespace Interfaces
{
    public interface IInterGrain : IGrainWithIntegerKey
    {
        Task<string> SayInternal(string name);
    }
}
