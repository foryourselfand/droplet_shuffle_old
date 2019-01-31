using UnityEngine;

public class SpriteOpacity : OpacityChanger
{
    private SpriteRenderer _spriteRenderer;

    protected override void DefineChangingComponent()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void SetOpacityRef(float current)
    {
        var tmp = _spriteRenderer.color;
        tmp.a = current;
        _spriteRenderer.color = tmp;
    }
}