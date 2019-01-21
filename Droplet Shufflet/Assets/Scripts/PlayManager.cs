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
    private GameObject[] _clickedGlasses;

    #endregion

    #region Other

    private Helper _helper;
    private int _lastShadow = -1;
    private bool _canClick;
    private int _clickedBoysCount;
    private int _maxBoysCount;

    #endregion

    private void Awake()
    {
        _helper = new Helper();

        _helper.SaveInArrayFromParent(ShadowsParent, ref _shadows);
        _helper.SaveInArrayFromParent(GlassesParent, ref _glasses);
        _helper.SaveInArrayFromParent(BoysParent, ref _boys);
        _helper.SaveInArrayFromParent(FingersParent, ref _fingers);

        _maxBoysCount = _boys.Length;
        _clickedGlasses = new GameObject[_maxBoysCount];
    }

    private void Start()
    {
        ActionWhenStart();
    }

    private void ActionWhenStart()
    {
        for (var i = 0; i < _glasses.Length; i++)
            _helper.SetParentAndY(_glasses[i], _shadows[i], 0.3F);

        foreach (var boy in _boys)
        {
            while (true)
            {
                var tempNumber = Random.Range(0, _shadows.Length);

                if (_shadows[tempNumber].transform.childCount != 1) continue;
                _helper.SetParentAndY(boy, _shadows[tempNumber], 0.12F);

                break;
            }
        }

        StartCoroutine(ShowWhereToMemento());
    }

    private IEnumerator ShowWhereToMemento()
    {
        yield return MoveGlassesAndWaitForDone(0.5F);

        yield return new WaitForSeconds(TimeToMemento);

        yield return MoveGlassesAndWaitForDone(-0.5F);

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
                _helper.SetParentAndY(_fingers[i], _shadows[firstShadow + i], 0.82F);

            break;
        }

        ChangeScaleOnFingers();

        yield return OpacityFingersAndWaitForDone(1);

        var multiply = Random.Range(0, 2) == 0 ? 1 : -1;

        _glasses[firstShadow].GetComponent<SpriteRenderer>().sortingOrder = 4 - multiply;
        _glasses[firstShadow + 1].GetComponent<SpriteRenderer>().sortingOrder = 4 + multiply;

        yield return MoveShadowsAndWaitForDone(firstShadow, multiply);

        yield return MoveShadowsAndWaitForDone(firstShadow, -multiply);

        _helper.Swap(ref _fingers[0], ref _fingers[1]);
        _helper.Swap(ref _shadows[firstShadow], ref _shadows[firstShadow + 1]);

        yield return OpacityFingersAndWaitForDone(0);

        _canClick = true;
    }

    private void ChangeScaleOnFingers()
    {
        var fingerScale = _fingers[0].transform.position.x < _fingers[1].transform.position.x ? 1 : -1;
        _fingers[0].transform.localScale = new Vector3(1 * fingerScale, 1, 1);
        _fingers[1].transform.localScale = new Vector3(-1 * fingerScale, 1, 1);
    }

    private IEnumerator MoveGlassesAndWaitForDone(float byY)
    {
        foreach (var glass in _glasses)
            glass.GetComponent<PositionChanger>().SetTarget(new Vector2(0, byY));
        yield return _helper.WaitUntilChangerDone(_glasses[0]);
    }

    private IEnumerator MoveShadowsAndWaitForDone(int firstShadow, int multiply)
    {
        _shadows[firstShadow].GetComponent<PositionChanger>().SetTarget(new Vector2(0.5F, multiply * 0.5F));
        _shadows[firstShadow + 1].GetComponent<PositionChanger>().SetTarget(new Vector2(-0.5F, -multiply * 0.5F));

        yield return _helper.WaitUntilChangerDone(_shadows[firstShadow]);
    }

    private IEnumerator OpacityFingersAndWaitForDone(float target)
    {
        foreach (var finger in _fingers)
            finger.GetComponent<OpacityChanger>().SetTarget(target);
        yield return _helper.WaitUntilChangerDone(_fingers[0]);
    }

    public void ActionOnClick(GameObject glass)
    {
        if (!_canClick || glass.transform.localPosition.y != 0.3F) return;
        _clickedGlasses[_clickedBoysCount] = glass;
        glass.GetComponent<PositionChanger>().SetTarget(new Vector2(0, 0.5F));
        if (BoyIn(glass))
        {
            _clickedBoysCount++;
            if (_clickedBoysCount == _maxBoysCount)
            {
                _canClick = false;
                StartCoroutine(Win());
            }
        }
        else
        {
            _canClick = false;
            Debug.Log("Game Over");
        }
    }

    private bool BoyIn(GameObject glass)
    {
        foreach (Transform child in glass.transform.parent)
            if (child.CompareTag("Boy"))
                return true;

        return false;
    }

    private IEnumerator Win()
    {
        foreach (var glass in _clickedGlasses)
        {
            yield return _helper.WaitUntilChangerDone(glass);
        }

        var directory = 0.1F;
        for (var i = 0; i < _maxBoysCount; i++)
        {
            foreach (var boy in _boys)
            {
                boy.transform.localPosition += new Vector3(0, directory, 0);
            }

            directory *= -1;

            yield return new WaitForSeconds(1);
        }
    }
}