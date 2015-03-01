using UnityEngine;
using System.Collections;

public class MainMenuHooks : MonoBehaviour {

    public void OnStartButtonClick()
    {
        Application.LoadLevel("WizZenjectTestBed");
    }
}
