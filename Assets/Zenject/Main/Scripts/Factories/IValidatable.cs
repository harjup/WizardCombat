using System;
using System.Collections.Generic;

namespace ModestTree.Zenject
{
    public interface IValidatable
    {
        IEnumerable<ZenjectResolveException> Validate();
    }
}
