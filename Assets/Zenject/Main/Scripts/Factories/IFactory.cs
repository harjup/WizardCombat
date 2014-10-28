using System;
using System.Collections.Generic;
using System.Reflection;

namespace ModestTree.Zenject
{
    // The difference between a factory and a provider:
    // Factories create new instances, providers might return an existing instance
    public interface IFactory<T>
    {
        // Note that we lose some type safety here when passing the arguments
        // We are trading compile time checks for some flexibility
        T Create(params object[] constructorArgs);

        IEnumerable<ZenjectResolveException> Validate(params Type[] extraType);
    }
}

