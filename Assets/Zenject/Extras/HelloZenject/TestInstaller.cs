﻿using ModestTree.Zenject;
using UnityEngine;
using System.Collections;

public class TestInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        _container.Bind<IDependencyRoot>().ToSingle<DependencyRootStandard>();

        _container.Bind<IInstaller>().ToSingle<StandardUnityInstaller>();

        _container.Bind<ITickable>().ToSingle<TestRunner>();
        _container.Bind<IInitializable>().ToSingle<TestRunner>();
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