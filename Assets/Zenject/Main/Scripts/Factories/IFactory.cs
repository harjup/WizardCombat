using System;

namespace ModestTree.Zenject
{
    public interface IFactory<T>
    {
        T Create();
    }

    public interface IFactory<TParam1, T>
    {
        T Create(TParam1 param);
    }

    public interface IFactory<TParam1, TParam2, T>
    {
        T Create(TParam1 param1, TParam2 param2);
    }

    public interface IFactory<TParam1, TParam2, TParam3, T>
    {
        T Create(TParam1 param1, TParam2 param2, TParam3 param3);
    }

    public interface IFactory<TParam1, TParam2, TParam3, TParam4, T>
    {
        T Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4);
    }
}
