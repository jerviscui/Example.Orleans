using System.Threading.Tasks;
using Interfaces;
using Orleans;

namespace SiloHost2
{
    public class InterGrain : Grain, IInterGrain
    {
        /// <inheritdoc />
        public async Task<string> SayInternal(string name)
        {
            await Task.Delay(5);

            return $"Internal {name}!";
        }
    }
}
