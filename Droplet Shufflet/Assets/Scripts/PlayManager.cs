using System.Collections;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public float TimeToMemento;

    private Helper _helper;

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

    private int _lastShadow = -1;

    private void Awake()
    {
        _helper = new Helper();

        _helper.SaveInArrayFromParent(ShadowsParent, ref _shadows);
        _helper.SaveInArrayFromParent(GlassesParent, ref _glasses);
        _helper.SaveInArrayFromParent(BoysParent, ref _boys);
        _helper.SaveInArrayFromParent(FingersParent, ref _fingers);
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

        StartCoroutine(ShowWhereToMemento());
    }

    private IEnumerator ShowWhereToMemento()
    {
        yield return MoveGlassByAndWaitForDone(0.5F);

        yield return new WaitForSeconds(TimeToMemento);

        yield return MoveGlassByAndWaitForDone(-0.5F);

        StartCoroutine(MoveShadows());
    }

    private IEnumerator MoveShadows()
    {
        int firstShadow;
        while (true)
        {
            firstShadow = Random.Range(0, _shadows.Length - 1);

            if (firstShadow == _lastShadow) continue;

            _lastShadow = firstShadow;

            for (var i = 0; i < _fingers.Length; i++)
                SetParentToShadowAndSetY(_fingers[i], firstShadow + i, 0.82F);

            break;
        }

        foreach (var finger in _fingers)
            finger.GetComponent<OpacityChanger>().SetTarget(1);
        ChangeScaleOnFingers();

        var multiply = Random.Range(0, 2) == 0 ? 1 : -1;

        _glasses[firstShadow].GetComponent<SpriteRenderer>().sortingOrder = 4 - multiply;
        _glasses[firstShadow + 1].GetComponent<SpriteRenderer>().sortingOrder = 4 + multiply;

        yield return MoveShadowAndWaitForDone(firstShadow, multiply);

        yield return MoveShadowAndWaitForDone(firstShadow, -multiply);

        foreach (var finger in _fingers)
            finger.GetComponent<OpacityChanger>().SetTarget(0);

        _helper.Swap(ref _fingers[0], ref _fingers[1]);
        _helper.Swap(ref _shadows[firstShadow], ref _shadows[firstShadow + 1]);
    }

    private void SetParentToShadowAndSetY(GameObject objectToChange, int shadowNumber, float yToChange)
    {
        objectToChange.transform.parent = _shadows[shadowNumber].transform;
        objectToChange.transform.localPosition = new Vector2(0, yToChange);
    }

    private void ChangeScaleOnFingers()
    {
        var fingerScale = _fingers[0].transform.position.x < _fingers[1].transform.position.x ? 1 : -1;
        _fingers[0].transform.localScale = new Vector3(1 * fingerScale, 1, 1);
        _fingers[1].transform.localScale = new Vector3(-1 * fingerScale, 1, 1);
    }

    private IEnumerator MoveGlassByAndWaitForDone(float byY)
    {
        foreach (var glass in _glasses)
            glass.GetComponent<PositionChanger>().SetTarget(new Vector2(0, byY));
        yield return _helper.WaitUntilChangerDone(_glasses[0]);
    }

    private IEnumerator MoveShadowAndWaitForDone(int firstShadow, int multiply)
    {
        _shadows[firstShadow].GetComponent<PositionChanger>().SetTarget(new Vector2(0.5F, multiply * 0.5F));
        _shadows[firstShadow + 1].GetComponent<PositionChanger>().SetTarget(new Vector2(-0.5F, -multiply * 0.5F));

        yield return _helper.WaitUntilChangerDone(_shadows[firstShadow]);
    }
}