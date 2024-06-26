using Orleans;
using System.Threading.Tasks;

namespace Interfaces;

[Alias("Interfaces.IHelloWorld")]
public interface IHelloWorld : IGrainWithIntegerKey
{

    #region Methods

    [Alias("SayHello")]
    Task<string> SayHelloAsync(string name, GrainCancellationToken? token = null);

    #endregion

}
