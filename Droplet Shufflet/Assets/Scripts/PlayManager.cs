using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public GameObject GlassParent;
    public GameObject BlueBoy, YellowBoy;

    private GameObject[] _glasses;


    private void Awake()
    {
        var childCount = GlassParent.transform.childCount;
        _glasses = new GameObject[childCount];

        for (var i = 0; i < childCount; i++)
        {
            _glasses[i] = GlassParent.transform.GetChild(i).gameObject;
        }
    }

    private void Start()
    {
        var tempNumber = Random.Range(0, 2);
        BlueBoy.transform.parent = _glasses[tempNumber].transform;
        BlueBoy.transform.localPosition = Vector3.zero;


        tempNumber = Random.Range(0, 2);
        YellowBoy.transform.parent = _glasses[2 + tempNumber].transform;
        YellowBoy.transform.localPosition = Vector3.zero;
    }
}