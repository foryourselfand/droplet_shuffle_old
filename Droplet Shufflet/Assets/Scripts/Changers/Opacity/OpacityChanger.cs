using UnityEngine;

public class OpacityChanger : Changer
{
    private SpriteRenderer _spriteRenderer;
    private float _target;

    protected override void DefineChangingComponent()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override bool CheckForCondition()
    {
        return Mathf.Abs(_spriteRenderer.color.a - _target) > 0.01F;
    }

    protected override void Change(float t)
    {
        var tmp = _spriteRenderer.color;
        tmp.a = Mathf.MoveTowards(tmp.a, _target, t * Speed);
        _spriteRenderer.color = tmp;
    }

    protected override void ActionOnEnd()
    {
        var tmp = _spriteRenderer.color;
        tmp.a = _target;
        _spriteRenderer.color = tmp;
    }

    public void SetTarget(float target)
    {
        StartChanging();
        _target = target;
    }

    public void SetCurrent(float current)
    {
        var tmp = _spriteRenderer.color;
        tmp.a = current;
        _spriteRenderer.color = tmp;
    }

    public void SetCurrentAndTarget(float current, float target)
    {
        SetCurrent(target);
        SetTarget(target);
    }
}