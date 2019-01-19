using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public GameObject ShadowParent;
    public GameObject BoysParent;
    public GameObject FingersParent;

    private GameObject[] _shadows;
    private GameObject[] _boys;
    private GameObject[] _fingers;

    private void Awake()
    {
        SaveInArray(ShadowParent, ref _shadows);
        SaveInArray(BoysParent, ref _boys);
        SaveInArray(FingersParent, ref _fingers);
    }

    private void Start()
    {
        ActionWhenStart();
    }

    private void ActionWhenStart()
    {
        for (var i = 0; i < _boys.Length; i++)
        {
            while (true)
            {
                var tempNumber = Random.Range(0, _shadows.Length);

                if (_shadows[tempNumber].transform.childCount != 1) continue;
                _boys[i].transform.parent = _shadows[tempNumber].transform;
                _boys[i].transform.localPosition = new Vector2(0, 0.12F);
                _fingers[i].transform.parent = _shadows[tempNumber].transform;
                _fingers[i].transform.localPosition = new Vector2(0, 0.82F);
                break;
            }
        }

        ChangeScaleOnFingers();

//        foreach (var shadow in _shadows)
//        {
//            shadow.GetComponent<PositionChanger>().SetTarget(new Vector2(shadow.transform.localPosition.x, 0.5F));
//        }
    }

    private void ChangeScaleOnFingers()
    {
        var temp = _fingers[0].transform.position.x < _fingers[1].transform.position.x ? -1 : 1;
        _fingers[0].transform.localScale = new Vector3(1 * temp, 1, 1);
        _fingers[1].transform.localScale = new Vector3(-1 * temp, 1, 1);
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