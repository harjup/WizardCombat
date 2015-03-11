using UnityEngine;
using System.Collections;
using ModestTree.Zenject;

public class GuiManager : IInitializable
{
    private DebugGuiHooks _guiHooks;
    
    public GuiManager(DebugGuiHooks guiHooks)
    {
        _guiHooks = guiHooks;
        _guiHooks.ResolveDependencies();
    }

    public void Initialize()
    {
        ShowMain();
    }

    public void ShowMain()
    {
        _guiHooks.ToggleMainText();
    }

    public void ShowResults()
    {
        _guiHooks.ToggleResultsText();
    }

    // TODO: Expose properties on guiHooks and do the databinding
    public DebugGuiHooks GuiHooks { get { return _guiHooks; } }
}
