using UnityEngine;

public class DefaultSmooth : _PositionBehavior
{
    private Vector3 _velocity = Vector3.zero;

    public override Vector3 GetCurrentBehavior(Vector3 current, Vector3 target, float t, float speed)
    {
        return Vector3.SmoothDamp(current, target, ref _velocity, t * speed * 25F);
    }

    public override void SpecialAction()
    {
        _velocity = Vector3.zero;
    }
}