using UnityEngine;
using System.Collections;
using Assets.Scripts.Managers;
using UnityEngine.UI;

public class ControlLabel : MonoBehaviour
{
    public enum Action
    {
        Undefined,
        Forward,
        Back,
        Left,
        Right,
        Dash
    }

    public Action ActionType;

    private Image panelImage;
    private Color defaultImageColor;

    private Text textImage;
    private Color defaultTextColor;

    public void Start()
    {
        panelImage = GetComponent<Image>();
        defaultImageColor = panelImage.color;

        textImage = GetComponentInChildren<Text>();
        defaultTextColor = textImage.color;
    }

    //TODO: Just want to make sure this works, have something else manage this. There should be no state management in here!
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Pressed();
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            Default();
        }
    }

    public void Default()
    {
        panelImage.color = defaultImageColor;
        //textImage.color = defaultTextColor;
    }

    public void Pressed()
    {
        panelImage.color = panelImage.color.SetAlpha(200);
        //textImage.color = new Color(0, 0, 0, 0);
    }
}