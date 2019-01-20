using UnityEngine;

public abstract class _PositionBehavior : MonoBehaviour
{
    public abstract Vector3 GetCurrentBehavior(Vector3 current, Vector3 target, float t, float speed);

    public virtual void SpecialAction()
    {
    }
}