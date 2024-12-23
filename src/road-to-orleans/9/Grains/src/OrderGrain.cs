using Interfaces;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains;

public class OrderGrain : Grain, IOrderGrain
{
    public OrderGrain()
    {
    }

    #region IOrderGrain implementations

    public async Task CreateAsync(OrderCreateInput order, GrainCancellationToken? token = null)
    {
        await Task.Delay(100);

        Console.WriteLine(User.GetName());
    }

    #endregion

}
