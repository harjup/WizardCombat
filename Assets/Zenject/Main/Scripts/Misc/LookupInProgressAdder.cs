using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModestTree.Zenject
{
    // This class should ONLY be used the following way:
    //
    //  using (_container.PushLookup(currentType))
    //  {
    //      new PropertiesInjector()
    //      container.Resolve()
    //      ...  etc.
    //  }
    //
    // It is very useful to track the object graph when debugging a DI related problem
    // so the way we track this is by pushing and popping from LookupsInProgress
    // using C# using() pattern
    internal class LookupInProgressAdder : IDisposable
    {
        Type _concreteType;

        public LookupInProgressAdder(DiContainer container, Type concreteType)
        {
            if (DiContainer.LookupsInProgress.Contains(concreteType))
            {
                throw new ZenjectResolveException(
                    "Circular dependency detected! \nObject graph:\n" + DiContainer.GetCurrentObjectGraph());
            }

            DiContainer.LookupsInProgress.Push(concreteType);

            _concreteType = concreteType;
        }

        public void Dispose()
        {
            Assert.That(DiContainer.LookupsInProgress.Peek() == _concreteType);
            DiContainer.LookupsInProgress.Pop();
        }
    }
}

