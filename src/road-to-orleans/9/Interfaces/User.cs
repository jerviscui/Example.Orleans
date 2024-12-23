using Orleans.Runtime;

namespace Interfaces;

public static class User
{

    #region Constants & Statics

    public const string NameKey = "User.NameKey";

    public static string GetName()
    {
        var name = RequestContext.Get(NameKey) as string;

        return name ?? string.Empty;
    }

    #endregion

}
