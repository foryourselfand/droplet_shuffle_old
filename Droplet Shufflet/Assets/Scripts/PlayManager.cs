using System.Collections;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public float TimeToMemento;

    #region Parents

    public GameObject ShadowsParent;
    public GameObject GlassesParent;
    public GameObject BoysParent;
    public GameObject FingersParent;

    #endregion

    #region Arrays

    private GameObject[] _shadows;
    private GameObject[] _glasses;
    private GameObject[] _boys;
    private GameObject[] _fingers;

    #endregion

    private int lastShadow = -1;

    private void Awake()
    {
        SaveInArrayFromParent(ShadowsParent, ref _shadows);
        SaveInArrayFromParent(GlassesParent, ref _glasses);
        SaveInArrayFromParent(BoysParent, ref _boys);
        SaveInArrayFromParent(FingersParent, ref _fingers);
    }

    private void Start()
    {
        ActionWhenStart();
    }

    private void ActionWhenStart()
    {
        for (var i = 0; i < _glasses.Length; i++)
            SetParentToShadowAndSetY(_glasses[i], i, 0.3F);

        for (var i = 0; i < _boys.Length; i++)
        {
            while (true)
            {
                var tempNumber = Random.Range(0, _shadows.Length);

                if (_shadows[tempNumber].transform.childCount != 1) continue;
                SetParentToShadowAndSetY(_boys[i], tempNumber, 0.12F);

                break;
            }
        }

        foreach (var finger in _fingers)
            finger.SetActive(false);

        StartCoroutine(ShowWhereToMemento());
    }

    private IEnumerator ShowWhereToMemento()
    {
        yield return MoveGlassByAndWaitForDone(0.5F);

        yield return new WaitForSeconds(TimeToMemento);
        Debug.Log("After Memento");

        yield return MoveGlassByAndWaitForDone(-0.5F);

        StartCoroutine(MoveShadows());
    }

    private IEnumerator MoveShadows()
    {
        int tempNumber;
        while (true)
        {
            tempNumber = Random.Range(0, _shadows.Length - 1);

            if (tempNumber == lastShadow) continue;

            lastShadow = tempNumber;
//            Debug.Log(string.Format("{0} {1}", _shadows[tempNumber].name, _shadows[tempNumber + 1].name));

            for (var i = 0; i < _fingers.Length; i++)
            {
                SetParentToShadowAndSetY(_fingers[i], tempNumber + i, 0.82F);
                Debug.Log(_shadows[tempNumber + i].name);
            }

            break;
        }

        foreach (var finger in _fingers)
        {
            finger.SetActive(true);
            finger.GetComponent<OpacityChanger>().SetCurrentAndTarget(0, 1);
        }

        _shadows[tempNumber].GetComponent<PositionChanger>().SetTarget(new Vector2(0.5F, -0.5F));
        _shadows[tempNumber + 1].GetComponent<PositionChanger>().SetTarget(new Vector2(-0.5F, 0.5F));

        ChangeScaleOnFingers();

        yield return new WaitUntil(() => _fingers[0].GetComponent<OpacityChanger>().IsDone());
    }

    private void SaveInArrayFromParent(GameObject parent, ref GameObject[] objectsArray)
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
        var temp = _fingers[0].transform.position.x < _fingers[1].transform.position.x ? 1 : -1;
        _fingers[0].transform.localScale = new Vector3(1 * temp, 1, 1);
        _fingers[1].transform.localScale = new Vector3(-1 * temp, 1, 1);
    }

    private IEnumerator MoveGlassByAndWaitForDone(float byY)
    {
        foreach (var glass in _glasses)
            glass.GetComponent<PositionChanger>().SetTarget(new Vector2(0, byY));
        yield return new WaitUntil(() => _glasses[0].GetComponent<PositionChanger>().IsDone());
    }
}