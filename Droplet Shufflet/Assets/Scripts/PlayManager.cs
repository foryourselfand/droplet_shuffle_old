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

    private int _lastShadow = -1;
    private int _distance = 1;
    private int _clickedBoysCount;
    private int _maxBoysCount;
    private int _currentLevel;
    private int _maxLevel = 1;
    private bool _canClick;

    #endregion

    private void Awake()
    {
        Helper.SaveInArrayFromParent(ShadowsParent, ref _shadows);
        Helper.SaveInArrayFromParent(GlassesParent, ref _glasses);
        Helper.SaveInArrayFromParent(BoysParent, ref _boys);
        Helper.SaveInArrayFromParent(FingersParent, ref _fingers);

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
            Helper.SetParentAndY(_glasses[i], _shadows[i], 0.3F);

        foreach (var boy in _boys)
        {
            while (true)
            {
                var tempNumber = Random.Range(0, _shadows.Length);

                if (_shadows[tempNumber].transform.childCount != 1) continue;
                Helper.SetParentAndY(boy, _shadows[tempNumber], 0.12F);

                break;
            }
        }

        StartCoroutine(ShowWhereToMemento());
    }

    private IEnumerator ShowWhereToMemento()
    {
        yield return MoveGlassesAndWaitForDone(_glasses, 0.5F);

        yield return new WaitForSeconds(TimeToMemento);

        yield return BoysJumping(1);

        yield return MoveGlassesAndWaitForDone(_glasses, -0.5F);

        StartCoroutine(MoveShadows());
    }

    private IEnumerator MoveShadows()
    {
        int firstShadow;
        if (_maxLevel >= 3)
            _distance = Random.Range(1, 3);
        while (true)
        {
            firstShadow = Random.Range(0, _shadows.Length - _distance);

            if (firstShadow == _lastShadow) continue;

            _lastShadow = firstShadow;

            for (var i = 0; i < _fingers.Length; i++)
                Helper.SetParentAndY(_fingers[i], _shadows[firstShadow + i * _distance],
                    0.82F); //TODO: Level Increasing

            break;
        }

        ChangeScaleOnFingers();

        yield return OpacityFingersAndWaitForDone(1);

        var multiply = Random.Range(0, 2) == 0 ? 1 : -1;

        ChangeOrderInGlasses(firstShadow, multiply);

        yield return MoveShadowsAndWaitForDone(firstShadow, multiply);

        yield return MoveShadowsAndWaitForDone(firstShadow, -multiply);

        ChangeOrderInGlasses(firstShadow, 0);

        Helper.Swap(ref _shadows[firstShadow], ref _shadows[firstShadow + _distance]);
        Helper.Swap(ref _glasses[firstShadow], ref _glasses[firstShadow + _distance]);

        yield return OpacityFingersAndWaitForDone(0);

        _currentLevel++;
        if (_currentLevel == _maxLevel)
        {
            _canClick = true;
            _currentLevel = 0;
        }
        else
            StartCoroutine(MoveShadows());
    }

    private void ChangeScaleOnFingers()
    {
        var fingerScale = _fingers[0].transform.position.x < _fingers[1].transform.position.x ? 1 : -1;
        _fingers[0].transform.localScale = new Vector3(1 * fingerScale, 1, 1);
        _fingers[1].transform.localScale = new Vector3(-1 * fingerScale, 1, 1);
    }

    private void ChangeOrderInGlasses(int firstShadow, int multiply)
    {
        _glasses[firstShadow].GetComponent<SpriteRenderer>().sortingOrder = 4 - multiply;
        _glasses[firstShadow + _distance].GetComponent<SpriteRenderer>().sortingOrder = 4 + multiply;
    }

    private IEnumerator MoveGlassesAndWaitForDone(GameObject[] glasses, float byY)
    {
        yield return Helper.MoveGlassesAndWaitForDone(glasses, byY, glasses.Length);
    }

    private IEnumerator MoveShadowsAndWaitForDone(int firstShadow, int multiply)
    {
        _shadows[firstShadow].GetComponent<PositionChanger>()
            .SetTarget(new Vector2(_distance * 0.5F, multiply * 0.5F));
        _shadows[firstShadow + _distance].GetComponent<PositionChanger>()
            .SetTarget(new Vector2(_distance * -0.5F, -multiply * 0.5F));

        yield return Helper.WaitUntilChangerDone(_shadows[firstShadow]);
    }

    private IEnumerator OpacityFingersAndWaitForDone(float target)
    {
        foreach (var finger in _fingers)
            finger.GetComponent<OpacityChanger>().SetTarget(target);
        yield return Helper.WaitUntilChangerDone(_fingers[0]);
    }

    public void ActionOnClick(GameObject glass)
    {
        if (!_canClick || glass.transform.localPosition.y != 0.3F) return;
        _clickedGlasses[_clickedBoysCount] = glass;
        glass.GetComponent<PositionChanger>().SetTarget(new Vector2(0, 0.5F));
        if (Helper.BoyIn(glass))
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
            StartCoroutine(Lose());
        }
    }

    private IEnumerator BoysJumping(int jumpCount)
    {
        var directory = 0.1F;
        for (var i = 0; i < jumpCount * 2; i++)
        {
            foreach (var boy in _boys)
            {
                boy.transform.localPosition += new Vector3(0, directory, 0);
            }

            directory *= -1;

            yield return new WaitForSeconds(0.5F);
        }
    }

    private IEnumerator Win()
    {
        foreach (var glass in _clickedGlasses)
            yield return Helper.WaitUntilChangerDone(glass);

        yield return BoysJumping(_maxBoysCount);

        yield return MoveGlassesAndWaitForDone(_clickedGlasses, -0.5F);

        _clickedBoysCount = 0;
        _maxLevel++;
        StartCoroutine(MoveShadows());
    }

    private IEnumerator Lose()
    {
        yield return Helper.WaitUntilChangerDone(_clickedGlasses[_clickedBoysCount]);
        yield return new WaitForSeconds(TimeToMemento);
        yield return Helper.MoveGlassesAndWaitForDone(_clickedGlasses, -0.5F, _clickedBoysCount + 1);
        Debug.Log("Game Over");
    }
}