using System;
using ModestTree.Zenject;
using UnityEngine;
using System.Collections;

public class MainMenuInstaller : MonoInstaller
{

    public GameObject MainMenuHooksPrefab;

    public override void InstallBindings()
    {
        Container.Bind<IInstaller>().ToSingle<StandardUnityInstaller>();

        Container.Bind<MainMenuHooks>().ToSingleFromPrefab<MainMenuHooks>(MainMenuHooksPrefab);
    }
}
