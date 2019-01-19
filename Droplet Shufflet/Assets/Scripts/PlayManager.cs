using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public GameObject ShadowsParent;
    public GameObject GlassesParent;
    public GameObject BoysParent;
    public GameObject FingersParent;

    private GameObject[] _shadows;
    private GameObject[] _glasses;
    private GameObject[] _boys;
    private GameObject[] _fingers;

    private void Awake()
    {
        SaveInArray(ShadowsParent, ref _shadows);
        SaveInArray(GlassesParent, ref _glasses);
        SaveInArray(BoysParent, ref _boys);
        SaveInArray(FingersParent, ref _fingers);
    }

    private void Start()
    {
        ActionWhenStart();
    }

    private void ActionWhenStart()
    {
        for (var i = 0; i < _glasses.Length; i++)
        {
            SetParentToShadowAndSetY(_glasses[i], i, 0.3F);
        }

        for (var i = 0; i < _boys.Length; i++)
        {
            while (true)
            {
                var tempNumber = Random.Range(0, _shadows.Length);

                if (_shadows[tempNumber].transform.childCount != 1) continue;

                SetParentToShadowAndSetY(_boys[i], tempNumber, 0.12F);
                SetParentToShadowAndSetY(_fingers[i], tempNumber, 0.82F);

                break;
            }
        }

        ChangeScaleOnFingers();

        foreach (var glass in _glasses)
        {
            Debug.Log(glass.name);
            glass.GetComponent<PositionChanger>().SetTarget(new Vector2(glass.transform.position.x, 0.5F));
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

    private void SetParentToShadowAndSetY(GameObject objectToChange, int shadowNumber, float yToChange)
    {
        objectToChange.transform.parent = _shadows[shadowNumber].transform;
        objectToChange.transform.localPosition = new Vector2(0, yToChange);
    }

    private void ChangeScaleOnFingers()
    {
        var temp = _fingers[0].transform.position.x < _fingers[1].transform.position.x ? -1 : 1;
        _fingers[0].transform.localScale = new Vector3(1 * temp, 1, 1);
        _fingers[1].transform.localScale = new Vector3(-1 * temp, 1, 1);
    }
}