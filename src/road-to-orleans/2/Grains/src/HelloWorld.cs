using System.Threading.Tasks;
using Interfaces;
using Orleans;

namespace Grains
{
    public class HelloWorld : Grain, IHelloWorld
    {
        public Task<string> SayHello(string name)
        {
            return Task.FromResult($"Hello {name}!");
        }
    }
}
