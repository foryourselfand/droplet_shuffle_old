using System;
using System.Net.NetworkInformation;
using UnityEngine;

public class PositionChanger : MonoBehaviour
{
    public float Speed;

    private Transform _transformLink;
    private Vector3 _targetVector;

    private bool _changing;

    private void Awake()
    {
        _transformLink = GetComponent<Transform>();
    }

    private void Update()
    {
        if (!_changing) return;
        if (Vector3.SqrMagnitude(_transformLink.position - _targetVector) > Vector3.kEpsilonNormalSqrt)
        {
            _transformLink.position =
                Vector3.MoveTowards(_transformLink.position, _targetVector, Time.deltaTime * Speed);
        }
        else
        {
            _transformLink.position = _targetVector;
            _changing = false;
        }
    }

    public void SetTarget(Vector2 targetVector)
    {
        _changing = true;
        _targetVector = targetVector;
    }
}