using Orleans;
using System;

namespace Interfaces;

[GenerateSerializer]
[Alias("Interfaces.MyTransactionException")]
public class MyTransactionException : Exception
{
    public MyTransactionException()
    {
    }

    public MyTransactionException(string message) : base(message)
    {
    }

    public MyTransactionException(string message, Exception inner) : base(message, inner)
    {
    }
}
