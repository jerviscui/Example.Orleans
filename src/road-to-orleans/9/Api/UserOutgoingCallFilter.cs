using Interfaces;

namespace Api;

public class UserOutgoingCallFilter : IOutgoingGrainCallFilter
{
    public UserOutgoingCallFilter()
    {
    }

    #region IOutgoingGrainCallFilter implementations

    public Task Invoke(IOutgoingGrainCallContext context)
    {
        // set user
        RequestContext.Set(User.NameKey, DateTime.Now.ToString("O"));

        return context.Invoke();
    }

    #endregion

}
