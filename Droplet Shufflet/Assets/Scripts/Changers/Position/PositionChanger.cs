using UnityEngine;

[RequireComponent(typeof(_PositionBehavior))]
public class PositionChanger : _Changer
{
    private Transform _transform;
    private _PositionBehavior _behavior;
    private Vector3 _target;
    private float _startSpeed;

    protected override void DefineChangingComponent()
    {
        _transform = GetComponent<Transform>();
        _behavior = GetComponent<_PositionBehavior>();
        _startSpeed = Speed;
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
        _behavior.BehaviorActionOnEnd();
        Speed = _startSpeed;
    }

    #endregion

    public void SetTarget(Vector3 target)
    {
        _target = _transform.localPosition + target;
        StartChanging();
    }

    public void SetTarget(Vector3 target, float speedAdd)
    {
        _target = _transform.localPosition + target;
        Speed += speedAdd;
        StartChanging();
    }
}