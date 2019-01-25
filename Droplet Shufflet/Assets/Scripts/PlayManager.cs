using System.Collections;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
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

    private GameObject _lastBoy;
    private int _firstShadow;
    private int _lastShadow = -1;
    private int _distance = 1;
    private int _maxDistance = 2;
    private int _clickedBoysCount;
    private int _maxBoysCount;
    private int _currentLevel;
    private int _maxLevel = 1;
    private int _leftBorder = 1, _rightBorder = 3;
    private bool _canClick;
    private int _lastBorder = -1;

    #endregion

    public CameraChanger CameraChanger;

    private void Awake()
    {
        Helper.SaveFromParentToArray(ShadowsParent, ref _shadows);
        Helper.SaveFromParentToArray(GlassesParent, ref _glasses);
        Helper.SaveFromParentToArray(BoysParent, ref _boys);
        Helper.SaveFromParentToArray(FingersParent, ref _fingers);

        _maxBoysCount = 2;
        _clickedGlasses = new GameObject[_boys.Length];
    }

    private void Start()
    {
        DefineOnStart();

        //TODO: Wait Until Shadow Done
        ActionAfterShadowDone();
    }

    private void DefineOnStart()
    {
        CameraChanger.SetCurrent(_maxBoysCount + 1);

        for (var i = 0; i < _glasses.Length; i++)
            Helper.SetParentAndY(_glasses[i], _shadows[i], 0.3F);


        var lastBoyNumber = -1;
        for (var i = 0; i < _maxBoysCount; i++)
        {
            GameObject currentBoy;
            while (true)
            {
                var currentBoyNumber = Random.Range(0, _boys.Length);

                if (currentBoyNumber == lastBoyNumber) continue;

                lastBoyNumber = currentBoyNumber;

                currentBoy = _boys[currentBoyNumber];

                break;
            }

            while (true)
            {
                var currentShadowIndex = Random.Range(_leftBorder, _rightBorder + 1);

                if (_shadows[currentShadowIndex].transform.childCount != 1) continue;
                Helper.SetParentAndY(currentBoy, _shadows[currentShadowIndex], 0.12F);

                break;
            }
        }

        foreach (var boy in _boys)
            if (boy.transform.parent.CompareTag("Shadow") == false)
                _lastBoy = boy;

        _lastBoy.SetActive(false);

        foreach (var finger in _fingers)
            finger.GetComponent<OpacityChanger>().SetCurrent(0);


        foreach (var finger in _fingers)
            finger.GetComponent<OpacityChanger>().SetCurrent(0);

        Helper.SetStartOpacityToBounds(ref _glasses, _leftBorder, _rightBorder);
        Helper.SetStartOpacityToBounds(ref _shadows, _leftBorder, _rightBorder);
    }

    private void ActionAfterShadowDone()
    {
        //TODO: Wait Until Shadow Done

        StartCoroutine(ShowWhereToMemento());
    }

    private IEnumerator ShowWhereToMemento()
    {
        yield return MoveGlassesAndJump(1);

        StartCoroutine(CheckLevel());
    }

    private IEnumerator CheckLevel()
    {
        if (_maxLevel % _maxBoysCount == 0)
        {
            _maxDistance++;


            bool condition;
            if (_lastBorder == -1)
                condition = Random.Range(0, 2) == 0;
            else
                condition = _lastBorder == _leftBorder;

            _lastBorder = condition ? _rightBorder + 1 : _leftBorder - 1;

            _leftBorder -= !condition ? 1 : 0;
            _rightBorder += condition ? 1 : 0;

            _shadows[_lastBorder].GetComponent<OpacityChanger>().SetTarget(0.75F);
            _glasses[_lastBorder].GetComponent<OpacityChanger>().SetTarget(0.75F);

            var byX = 0.5F;
            byX *= _lastBorder == _leftBorder ? 1 : -1;
            ShadowsParent.GetComponent<PositionChanger>().SetTarget(new Vector3(byX, 0));

            CameraChanger.SetTarget(1);

            yield return Helper.WaitUntilMoveDone(ShadowsParent);
            yield return Helper.WaitUntilFadeDone(_shadows[_lastBorder]);
            yield return Helper.WaitUntilFadeDone(_glasses[_lastBorder]);
            yield return Helper.WaitUntilChangerDone<CameraChanger>(CameraChanger.gameObject);

            if (_maxLevel % (_maxBoysCount * 2) == 0)
            {
                _lastBoy.SetActive(true);
                _maxBoysCount++;
                _maxLevel = 1;
                Helper.SetParentAndY(_lastBoy, _shadows[_lastBorder], 0.12F);

                yield return MoveGlassesAndJump(_maxBoysCount);
            }
            else
            {
                _glasses[_lastBorder].GetComponent<PositionChanger>().SetTarget(new Vector3(0, 0.5F));
                yield return Helper.WaitUntilMoveDone(_glasses[_lastBorder]);

                yield return new WaitForSeconds(0.2F);

                _glasses[_lastBorder].GetComponent<PositionChanger>().SetTarget(new Vector3(0, -0.5F));
                yield return Helper.WaitUntilMoveDone(_glasses[_lastBorder]);
            }
        }

        StartCoroutine(FingersSetting());
    }

    private IEnumerator FingersSetting()
    {
        _distance = Random.Range(1, _maxDistance);

        while (true)
        {
            _firstShadow = Random.Range(_leftBorder, _rightBorder - _distance + 1);

            if (_firstShadow == _lastShadow) continue;

            _lastShadow = _firstShadow;

            for (var i = 0; i < _fingers.Length; i++)
                Helper.SetParentAndY(_fingers[i], _shadows[_firstShadow + i * _distance], 0.82F);

            break;
        }

        ChangeScaleOnFingers();

        yield return FadeFingers(1);

        StartCoroutine(MoveShadows());
    }

    private IEnumerator MoveShadows()
    {
        var multiply = Random.Range(0, 2) == 0 ? 1 : -1;

        ChangeOrderInGlasses(_firstShadow, multiply);

        yield return Helper.MoveShadows(_shadows, _firstShadow, _distance, multiply);
        yield return Helper.MoveShadows(_shadows, _firstShadow, _distance, -multiply);

        ChangeOrderInGlasses(_firstShadow, 0);

        Helper.Swap(ref _shadows[_firstShadow], ref _shadows[_firstShadow + _distance]);
        Helper.Swap(ref _glasses[_firstShadow], ref _glasses[_firstShadow + _distance]);

        yield return FadeFingers(0);

        _currentLevel++;
        if (_currentLevel == _maxLevel)
        {
            _canClick = true;
            _currentLevel = 0;
        }
        else
            StartCoroutine(FingersSetting());
    }

    private IEnumerator MoveGlassesAndJump(int jumpCount)
    {
        yield return Helper.MoveGlasses(_glasses, 0.5F, _leftBorder, _rightBorder + 1);

        yield return JumpBoys(jumpCount);

        yield return Helper.MoveGlasses(_glasses, -0.5F, _leftBorder, _rightBorder + 1);
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

    private IEnumerator FadeFingers(float target)
    {
        foreach (var finger in _fingers)
            finger.GetComponent<OpacityChanger>().SetTarget(target);

        yield return Helper.WaitUntilFadeDone(_fingers[0]);
    }

    private IEnumerator JumpBoys(int jumpCount)
    {
        var directory = 0.1F;
        for (var i = 0; i < jumpCount * 2; i++)
        {
            foreach (var boy in _boys)
                boy.transform.localPosition += new Vector3(0, directory, 0);

            directory *= -1;

            yield return new WaitForSeconds(1F / jumpCount);
        }
    }

    private IEnumerator Win()
    {
        for (var i = 0; i < _maxBoysCount; i++)
            yield return Helper.WaitUntilMoveDone(_clickedGlasses[i]);

        yield return JumpBoys(_maxBoysCount);

        yield return Helper.MoveGlasses(_clickedGlasses, -0.5F, 0, _maxBoysCount);

        _clickedBoysCount = 0;
        _maxLevel++;

        StartCoroutine(CheckLevel());
    }

    private IEnumerator Lose()
    {
        yield return Helper.WaitUntilMoveDone(_clickedGlasses[_clickedBoysCount]);

        yield return new WaitForSeconds(1);

        yield return Helper.MoveGlasses(_clickedGlasses, -0.5F, 0, _clickedBoysCount + 1);

        Debug.Log("Game Over");
    }

    public void ActionOnClick(GameObject glass)
    {
        if (!_canClick || glass.transform.localPosition.y != 0.3F) return;

        _clickedGlasses[_clickedBoysCount] = glass;
        glass.GetComponent<PositionChanger>().SetTarget(new Vector2(0, 0.5F));

        IEnumerator coroutineToStart = null;
        if (Helper.BoyIn(glass))
        {
            if (++_clickedBoysCount == _maxBoysCount)
                coroutineToStart = Win();
        }
        else
            coroutineToStart = Lose();

        if (coroutineToStart == null) return;
        _canClick = false;
        StartCoroutine(coroutineToStart);
    }
}