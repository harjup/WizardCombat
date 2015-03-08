using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugGuiHooks : MonoBehaviour
{
    private Text _vector3Text;
    private Text _debugText;
    private Text _boxAmountText;
    private Text _timeElapsed;

    void Start()
    {
        _vector3Text = transform.FindChild("Vector3Text").GetComponent<Text>();
        _debugText = transform.FindChild("DebugText").GetComponent<Text>();
        _boxAmountText = transform.FindChild("BoxAmountText").GetComponent<Text>();
        _timeElapsed = transform.FindChild("TimeText").GetComponent<Text>();
    }

    public Vector3 PositionDebug
    {
        set { _vector3Text.text = value.ToString(); }
    }

    public string TextDebug
    {
        set { _debugText.text = value; }
    }

    public string BoxAmount
    {
        set
        {
            if (_boxAmountText == null)
            {
                _boxAmountText = transform.FindChild("BoxAmountText").GetComponent<Text>();
            }

            _boxAmountText.text = value;
        }
    }


    public string TimeElapsed
    {
        set
        {
            if (_timeElapsed == null)
            {
                _timeElapsed = transform.FindChild("TimeText").GetComponent<Text>();
            }

            _timeElapsed.text = value;
        }
    }
}
