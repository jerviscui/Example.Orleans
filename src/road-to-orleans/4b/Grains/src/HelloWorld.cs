using System;
using System.Threading.Tasks;
using Interfaces;
using Orleans;

namespace Grains
{
    public class HelloWorld : Grain, IHelloWorld
    {
        public async Task<string> SayHello(string name)
        {
            await Task.Delay(Random.Shared.Next(10, 50));

            return $"Hello {name}!";
        }
    }
}
