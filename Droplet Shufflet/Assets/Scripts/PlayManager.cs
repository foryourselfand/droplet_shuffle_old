using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public GameObject GlassParent;
    public GameObject BoysParent;

    private GameObject[] _glasses;
    private GameObject[] _boys;


    private void Awake()
    {
        SaveInArray(GlassParent, ref _glasses);
        SaveInArray(BoysParent, ref _boys);
    }

    private void Start()
    {
        foreach (var boy in _boys)
        {
            while (true)
            {
                var tempNumber = Random.Range(0, _glasses.Length);

                if (_glasses[tempNumber].transform.childCount != 0) continue;
                boy.transform.parent = _glasses[tempNumber].transform;
                boy.transform.localPosition = Vector3.zero;
                break;
            }
        }

        foreach (var glass in _glasses)
        {
            glass.GetComponent<PositionChanger>().SetTarget(new Vector2(glass.transform.position.x, 1));
        }
    }

    private void SaveInArray(GameObject parent, ref GameObject[] objectsArray)
    {
        var childCount = parent.transform.childCount;
        objectsArray = new GameObject[childCount];

        for (var i = 0; i < childCount; i++)
        {
            objectsArray[i] = parent.transform.GetChild(i).gameObject;
        }
    }
}