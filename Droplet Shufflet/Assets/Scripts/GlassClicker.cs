using UnityEngine;

public class GlassClicker : MonoBehaviour
{
    public PlayManager playManager;

    private void OnMouseDown()
    {
        playManager.ActionOnClick(gameObject);
    }
}