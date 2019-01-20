using UnityEngine;

public class SpecialSmoothBehavior : _PositionBehavior
{
    private bool _fromSlowToFast;

    private static float _lerpTime;

    public override Vector3 GetCurrentBehavior(Vector3 current, Vector3 target, float t, float speed)
    {
        if (_fromSlowToFast)
            _lerpTime += t / speed;
        else
            _lerpTime = t * speed;

        return Vector2.Lerp(current, target, _lerpTime);
    }

    public override void SpecialAction()
    {
        _lerpTime = 0;
        _fromSlowToFast = !_fromSlowToFast;
    }
}