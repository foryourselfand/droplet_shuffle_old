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

    private void Awake()
    {
        Helper.SaveInArrayFromParent(ShadowsParent, ref _shadows);
        Helper.SaveInArrayFromParent(GlassesParent, ref _glasses);
        Helper.SaveInArrayFromParent(BoysParent, ref _boys);
        Helper.SaveInArrayFromParent(FingersParent, ref _fingers);

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

            yield return Helper.WaitUntilPositionChangerDone(ShadowsParent);
            yield return Helper.WaitUntilOpacityChangerDone(_shadows[_lastBorder]);
            yield return Helper.WaitUntilOpacityChangerDone(_glasses[_lastBorder]);

            if (_maxLevel % (_maxBoysCount * 2) == 0)
            {
                _lastBoy.SetActive(true);
                Helper.SetParentAndY(_lastBoy, _shadows[_lastBorder], 0.12F);
                _maxBoysCount++;
                yield return MoveGlassesAndJump(_maxBoysCount);
            }
        }

        StartCoroutine(MoveShadows());
    }

    private IEnumerator MoveShadows()
    {
        _distance = Random.Range(1, _maxDistance);

        int firstShadow;
        while (true)
        {
            firstShadow = Random.Range(_leftBorder, _rightBorder - _distance + 1);

            if (firstShadow == _lastShadow) continue;

            _lastShadow = firstShadow;

            for (var i = 0; i < _fingers.Length; i++)
                Helper.SetParentAndY(_fingers[i], _shadows[firstShadow + i * _distance], 0.82F);

            break;
        }

        ChangeScaleOnFingers();

        yield return OpacityFingersAndWaitForDone(1);

        var multiply = Random.Range(0, 2) == 0 ? 1 : -1;

        ChangeOrderInGlasses(firstShadow, multiply);

        yield return Helper.MoveShadowsAndWaitForDone(_shadows, firstShadow, _distance, multiply);

        yield return Helper.MoveShadowsAndWaitForDone(_shadows, firstShadow, _distance, -multiply);

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

    private IEnumerator MoveGlassesAndJump(int jumpCount)
    {
        yield return Helper.MoveGlassesAndWaitForDone(_glasses, 0.5F, _leftBorder, _rightBorder + 1);

        yield return BoysJumping(jumpCount);

        yield return Helper.MoveGlassesAndWaitForDone(_glasses, -0.5F, _leftBorder, _rightBorder + 1);
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

    private IEnumerator OpacityFingersAndWaitForDone(float target)
    {
        foreach (var finger in _fingers)
            finger.GetComponent<OpacityChanger>().SetTarget(target);
        yield return Helper.WaitUntilOpacityChangerDone(_fingers[0]);
    }

    private IEnumerator BoysJumping(int jumpCount)
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
            yield return Helper.WaitUntilPositionChangerDone(_clickedGlasses[i]);


        yield return BoysJumping(_maxBoysCount);
        yield return Helper.MoveGlassesAndWaitForDone(_clickedGlasses, -0.5F, 0, _maxBoysCount);

        _clickedBoysCount = 0;
        _maxLevel++;
        StartCoroutine(CheckLevel());
    }

    private IEnumerator Lose()
    {
        yield return Helper.WaitUntilPositionChangerDone(_clickedGlasses[_clickedBoysCount]);
        yield return new WaitForSeconds(1);
        yield return Helper.MoveGlassesAndWaitForDone(_clickedGlasses, -0.5F, 0, _clickedBoysCount + 1);

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