using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public GameObject ShadowParent;
    public GameObject BoysParent;

    private GameObject[] _shadows;
    private GameObject[] _boys;

    private void Awake()
    {
        SaveInArray(ShadowParent, ref _shadows);
        SaveInArray(BoysParent, ref _boys);
    }

    private void Start()
    {
        foreach (var boy in _boys)
        {
            while (true)
            {
                var tempNumber = Random.Range(0, _shadows.Length);

                if (_shadows[tempNumber].transform.childCount != 1) continue;
                boy.transform.parent = _shadows[tempNumber].transform;
                boy.transform.localPosition = new Vector2(0, 0.12F);
                break;
            }
        }


        foreach (var glass in _shadows)
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