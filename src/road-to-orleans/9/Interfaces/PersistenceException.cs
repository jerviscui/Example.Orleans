using Orleans;
using System;

namespace Interfaces;

[GenerateSerializer]
[Alias("Grains.PersistenceException")]
public class PersistenceException : Exception
{
    public PersistenceException()
    {
    }

    public PersistenceException(string message) : base(message)
    {
    }

    public PersistenceException(string message, Exception inner) : base(message, inner)
    {
    }
}
