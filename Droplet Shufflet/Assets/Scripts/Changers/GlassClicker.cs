using UnityEngine;

public class GlassClicker : MonoBehaviour
{
    public PlayManager playManager;

    private void OnMouseDown()
    {
        Debug.Log(transform.parent.gameObject.transform.childCount);
    }
}