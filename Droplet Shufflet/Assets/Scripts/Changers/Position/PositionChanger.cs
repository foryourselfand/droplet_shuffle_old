using UnityEngine;

[RequireComponent(typeof(PositionBehavior))]
public class PositionChanger : Changer
{
    private Transform _transform;
    private PositionBehavior _behavior;
    private Vector3 _target;

    protected override void DefineChangingComponent()
    {
        _transform = GetComponent<Transform>();
        _behavior = GetComponent<PositionBehavior>();
    }

    #region Changer

    protected override bool CheckForCondition()
    {
        return Vector3.SqrMagnitude(_transform.localPosition - _target) > Vector3.kEpsilon;
    }

    protected override void Change(float t)
    {
        _transform.localPosition = _behavior.GetCurrentBehavior(_transform.localPosition, _target, t, Speed);
    }

    protected override void ActionOnEnd()
    {
        _transform.localPosition = _target;
        _behavior.SpecialAction();
    }

    #endregion

    public void SetTarget(Vector3 target)
    {
        StartChanging();
        _target = _transform.localPosition + target;
    }
}