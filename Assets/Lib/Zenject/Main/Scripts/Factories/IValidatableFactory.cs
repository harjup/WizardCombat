using System;
using System.Collections.Generic;

namespace ModestTree.Zenject
{
    public interface IValidatableFactory
    {
        Type ConstructedType
        {
            get;
        }

        Type[] ProvidedTypes
        {
            get;
        }
    }
}
