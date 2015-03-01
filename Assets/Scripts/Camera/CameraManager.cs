using ModestTree.Zenject;
using UnityEngine;
using System.Collections;

public class CameraManager {

    public CameraManager(
        [Inject(MyGameInstaller.Cameras.Main)] 
        Camera main)
    {
        Main = main;
    }

    public Camera Main { get; set; }
}
