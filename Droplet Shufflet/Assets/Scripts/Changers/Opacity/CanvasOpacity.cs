using UnityEngine;

public class CanvasOpacity : OpacityChanger
{
    private CanvasGroup _canvasGroup;
    
    protected override void DefineChangingComponent()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    protected override void SetOpacityRef(float current)
    {
        _canvasGroup.alpha = current;
    }
}