using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugGuiHooks : MonoBehaviour
{
    private Text _vector3Text;

    void Start()
    {
        _vector3Text = GetComponentInChildren<Text>();
    }

    public Vector3 PositionDebug
    {
        set { _vector3Text.text = value.ToString(); }
    }

}
