using Interfaces;
using Orleans;
using System;
using System.Threading.Tasks;

namespace SiloHost2;

public class StockGrain : Grain, IStockGrain
{

    #region IStockGrain implementations

    public Task CreateAsync(StockCreateInput stock, GrainCancellationToken? token = null)
    {
        throw new NotImplementedException();
    }

    #endregion

}
