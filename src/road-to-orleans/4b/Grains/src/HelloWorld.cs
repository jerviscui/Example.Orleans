using System;
using System.Threading.Tasks;
using Interfaces;
using Orleans;

namespace Grains
{
    public class HelloWorld : Grain, IHelloWorld
    {
        private readonly IGrainFactory _grainFactory;

        private readonly IClusterClient _client;

        public HelloWorld(IGrainFactory grainFactory, IClusterClient client)
        {
            _grainFactory = grainFactory;
            _client = client;
        }

        public async Task<string> SayHello(string name)
        {
            await Task.Delay(Random.Shared.Next(10, 50));

            var result = await _grainFactory.GetGrain<IInterGrain>(this.GetPrimaryKeyLong()).SayInternal(name);
            // or
            //var result = await _client.GetGrain<IInterGrain>(this.GetPrimaryKeyLong()).SayInternal(name);

            return $"Hello {name}!\n{result}";
        }
    }
}
