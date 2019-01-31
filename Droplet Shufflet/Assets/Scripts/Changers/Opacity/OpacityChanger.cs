using UnityEngine;

public abstract class OpacityChanger : _Changer
{
    private float _current;
    private float _target;

    public float Opacity
    {
        private get { return _current; }
        set
        {
            _current = value;
            SetOpacityRef(value);
        }
    }

    protected abstract void SetOpacityRef(float current);

    #region CHANGER

    protected abstract override void DefineChangingComponent();

    protected override bool CheckForCondition()
    {
        return Mathf.Abs(Opacity - _target) > 0.01F;
    }

    protected override void Change(float t)
    {
        Opacity = Mathf.MoveTowards(Opacity, _target, t * Speed);
    }

    protected override void ActionOnEnd()
    {
        Opacity = _target;
    }

    #endregion

    public void SetCurrent(float current)
    {
        Opacity = current;
    }

    public void SetTarget(float target)
    {
        _target = target;
        StartChanging();
    }

    public void SetCurrentAndTarget(float current, float target)
    {
        SetCurrent(current);
        SetTarget(target);
    }
}