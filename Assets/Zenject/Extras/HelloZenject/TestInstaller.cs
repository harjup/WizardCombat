using ModestTree.Zenject;
using UnityEngine;
using System.Collections;

public class TestInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IDependencyRoot>().ToSingle<DependencyRootStandard>();

        Container.Bind<IInstaller>().ToSingle<StandardUnityInstaller>();

        Container.Bind<ITickable>().ToSingle<TestRunner>();
        Container.Bind<IInitializable>().ToSingle<TestRunner>();
    }
}

public class TestRunner : ITickable, IInitializable
{
    public void Initialize()
    {
        Debug.Log("Hello World");
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Exiting!");
            Application.Quit();
        }
    }
}