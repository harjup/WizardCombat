using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine.UI;

public class DebugGuiHooks : MonoBehaviour
{
    private Text _vector3Text;
    private Text _debugText;
    private Text _boxAmountText;
    private Text _timeElapsed;
    private Text _timeElapsedResult;

    private GameObject _mainGui;
    private GameObject _resultsGui;

    public delegate void ButtonPressed();

    public event ButtonPressed AgainButtonPressed;


    private bool _initialized = false;
    public void ResolveDependencies()
    {
        if (!_initialized)
        {
            var textComponents = GetComponentsInChildren<Text>();
            _vector3Text = textComponents.FirstOrDefault(t => t.name == "Vector3Text");
            _debugText = textComponents.FirstOrDefault(t => t.name == "DebugText");
            _boxAmountText = textComponents.FirstOrDefault(t => t.name == "BoxAmountText");

            _timeElapsed = textComponents.FirstOrDefault(t => t.name == "TimeText");
            _timeElapsedResult = textComponents.FirstOrDefault(t => t.name == "TimeResultText");

            _mainGui = transform.FindChild("MainGui").gameObject;
            _resultsGui = transform.FindChild("ResultsGui").gameObject;
        }

        _initialized = true;
    }

    public void ToggleMainText()
    {
        _mainGui.SetActive(true);
        _resultsGui.SetActive(false);
    }

    public void ToggleResultsText()
    {
        _mainGui.SetActive(false);
        _resultsGui.SetActive(true);
    }
    
    public Vector3 PositionDebug
    {
        set
        {
            _vector3Text.text = value.ToString();
        }
    }

    public string TextDebug
    {
        set
        {
            _debugText.text = value;
        }
    }

    public string BoxAmount
    {
        set
        {
            _boxAmountText.text = value;
        }
    }


    public string TimeElapsed
    {
        set
        {
            _timeElapsed.text = value;
            _timeElapsedResult.text = value;
        }
    }

    public void OnButtonPressed()
    {
        var handler = AgainButtonPressed;
        if (handler != null) handler();
    }
}
