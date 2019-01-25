using UnityEngine;

public abstract class _Changer : MonoBehaviour
{
    public float Speed;

    private bool _changing;

    private void Awake()
    {
        DefineChangingComponent();
    }

    private void Update()
    {
        if (!_changing) return;
        if (CheckForCondition())
            Change(Time.deltaTime);
        else
        {
            ActionOnEnd();
            _changing = false;
        }
    }

    protected abstract void DefineChangingComponent();

    protected abstract bool CheckForCondition();

    protected abstract void Change(float t);

    protected abstract void ActionOnEnd();

    public void StartChanging()
    {
        _changing = true;
    }

    public bool IsDone()
    {
        return _changing == false;
    }
}