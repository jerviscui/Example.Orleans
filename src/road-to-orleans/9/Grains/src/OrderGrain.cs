using Interfaces;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Grains;

public class OrderGrain : Grain, IOrderGrain, IIncomingGrainCallFilter
{
    public OrderGrain()
    {
    }

    #region IIncomingGrainCallFilter implementations

    public async Task Invoke(IIncomingGrainCallContext context)
    {
        Console.WriteLine($"{context.MethodName} before calling");
        await context.Invoke();
        Console.WriteLine($"{context.MethodName} after called");
    }

    #endregion

    #region IOrderGrain implementations

    public async Task CreateAsync(OrderCreateInput order, GrainCancellationToken? token = null)
    {
        await Task.Delay(100);

        Console.WriteLine($"User.Name: {User.GetName()}");
    }

    #endregion

}
