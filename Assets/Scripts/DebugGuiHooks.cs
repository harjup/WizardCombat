using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugGuiHooks : MonoBehaviour
{
    private Text _vector3Text;
    private Text _debugText;

    void Start()
    {
        _vector3Text = transform.FindChild("Vector3Text").GetComponent<Text>();
        _debugText = transform.FindChild("DebugText").GetComponent<Text>();
    }

    public Vector3 PositionDebug
    {
        set { _vector3Text.text = value.ToString(); }
    }

    public string TextDebug
    {
        set { _debugText.text = value; }
    }

}
