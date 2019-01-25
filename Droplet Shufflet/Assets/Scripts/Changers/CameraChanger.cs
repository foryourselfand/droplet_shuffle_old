using UnityEngine;

public class CameraChanger : _Changer
{
    private Camera _camera;
    private float _target;

    protected override void DefineChangingComponent()
    {
        _camera = GetComponent<Camera>();
    }

    protected override bool CheckForCondition()
    {
        return Mathf.Abs(_camera.orthographicSize - _target) > 0.01F;
    }

    protected override void Change(float t)
    {
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _target, t * Speed);
    }

    protected override void ActionOnEnd()
    {
        _camera.orthographicSize = _target;
    }

    public void SetTarget(float increasing)
    {
        StartChanging();
        _target = _camera.orthographicSize + increasing;
    }

    public void SetCurrent(float current)
    {
        _camera.orthographicSize = current;
    }
}