using System.Collections;
using System.Collections.Generic;
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
    private GameObject[] _fingers;
    private List<GameObject> _boys;
    private List<GameObject> _winGlasses;
    private List<GameObject> _loseGlasses;

    #endregion

    #region Other

    private int _firstShadow;
    private int _lastShadow = -1;
    private int _distance = 1;
    private int _maxDistance = 2;
    private int _clickedBoysCount;
    private int _maxBoysCount;
    private int _currentLevel;
    private int _maxLevel = 1;
    private int _leftBorder = 2, _rightBorder = 4;
    private int _lastBorder = -1;

    #endregion

    private bool _canClick;

    public GameObject CameraChanger;

    private void Awake()
    {
        Helper.SaveFromParentToArray(ShadowsParent, ref _shadows);
        Helper.SaveFromParentToArray(GlassesParent, ref _glasses);
        Helper.SaveFromParentToArray(FingersParent, ref _fingers);
        Helper.SaveFromParentToList(BoysParent, ref _boys);

        _maxBoysCount = 2;
    }

    private void Start()
    {
        DefineOnStart();

        //TODO: Wait Until Shadow Done
        ActionAfterShadowDone();
    }

    private void DefineOnStart()
    {
        CameraChanger.GetComponent<CameraChanger>().SetCurrent(_rightBorder - _leftBorder + 1);

        foreach (var boy in _boys)
        {
            boy.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            boy.SetActive(false);
        }

        for (var i = 0; i < _glasses.Length; i++)
            Helper.SetParentAndY(_glasses[i], _shadows[i], 0.3F);

        for (var i = 0; i < _maxBoysCount; i++)
        {
            var freshBoy = Helper.GetDeActiveBoyFrom(_boys);

            while (true)
            {
                var currentShadowIndex = Random.Range(_leftBorder, _rightBorder + 1);

                if (_shadows[currentShadowIndex].transform.childCount != 1) continue;
                Helper.SetParentAndY(freshBoy, _shadows[currentShadowIndex], 0.12F);
                freshBoy.SetActive(true);

                break;
            }
        }

        _winGlasses = new List<GameObject>();
        _loseGlasses = new List<GameObject>();

        for (var i = _leftBorder; i < _rightBorder + 1; i++)
        {
            if (Helper.BoyIn(_glasses[i]))
                _winGlasses.Add(_glasses[i]);
            else
                _loseGlasses.Add(_glasses[i]);
        }

        foreach (var finger in _fingers)
            finger.GetComponent<OpacityChanger>().SetCurrent(0);

        foreach (var shadow in _shadows)
            shadow.SetActive(false);

        for (var i = _leftBorder; i < _rightBorder + 1; i++)
            _shadows[i].SetActive(true);
    }

    private void ActionAfterShadowDone()
    {
        //TODO: Wait Until Shadow Done

        StartCoroutine(ShowWhereToMemento());
    }

    private IEnumerator ShowWhereToMemento()
    {
        yield return MoveGlassesAndJump(_winGlasses, 1);

        StartCoroutine(CheckLevel());
    }

    private IEnumerator CheckLevel()
    {
        var levelCountChecker = _maxLevel - 1;
        if (levelCountChecker != 0 && levelCountChecker % _maxBoysCount == 0 && _maxBoysCount != 4)
        {
            _maxDistance++;

            bool condition;
            if (_lastBorder == -1)
                condition = Random.Range(0, 2) == 0;
            else
                condition = _lastBorder == _leftBorder;

            _lastBorder = condition ? ++_rightBorder : --_leftBorder;

            _shadows[_lastBorder].SetActive(true);

            var byX = 0.5F;
            byX *= condition ? -1 : 1;
            ShadowsParent.GetComponent<PositionChanger>().SetTarget(new Vector3(byX, 0));

            CameraChanger.GetComponent<CameraChanger>().AddToTarget(1);

            yield return Helper.WaitUntilChangerDone(ShadowsParent);
            yield return Helper.WaitUntilChangerDone(CameraChanger);

            if (levelCountChecker % (_maxBoysCount * 2) == 0)
            {
                _maxBoysCount++;
                _maxLevel = 1;

                _winGlasses.Add(_glasses[_lastBorder]);

                var freshBoy = Helper.GetDeActiveBoyFrom(_boys);
                Helper.SetParentAndY(freshBoy, _shadows[_lastBorder], 0.12F);
                freshBoy.SetActive(true);

                yield return MoveGlassesAndJump(_winGlasses, _maxBoysCount);
            }
            else
            {
                _loseGlasses.Add(_glasses[_lastBorder]);
                yield return MoveGlassesAndJump(_loseGlasses, 1);
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

    private IEnumerator MoveGlassesAndJump(List<GameObject> glasses, int jumpCount)
    {
        yield return Helper.MoveGlasses(glasses, 0.5F);

        yield return JumpBoys(jumpCount);

        yield return Helper.MoveGlasses(glasses, -0.5F);
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

        yield return Helper.WaitUntilChangerDone(_fingers[0]);
    }

    private IEnumerator JumpBoys(int jumpCount)
    {
        var directory = 0.1F;
        var active = true;
        for (var i = 0; i < jumpCount * 2; i++)
        {
            foreach (var boy in _boys)
            {
                boy.transform.localPosition += new Vector3(0, directory, 0);
                boy.transform.GetChild(0).GetChild(0).gameObject.SetActive(active);
            }

            directory *= -1;
            active = !active;

            yield return new WaitForSeconds(0.5F / jumpCount);
        }
    }

    private IEnumerator Win()
    {
        foreach (var glass in _winGlasses)
            yield return Helper.WaitUntilChangerDone(glass);

        yield return JumpBoys(_maxBoysCount);

        yield return Helper.MoveGlasses(_winGlasses, -0.5F);

        _clickedBoysCount = 0;
        _maxLevel++;

        StartCoroutine(CheckLevel());
    }

    private IEnumerator Lose(GameObject lostGlass)
    {
        yield return Helper.WaitUntilChangerDone(lostGlass);

        yield return new WaitForSeconds(0.5F);

        lostGlass.GetComponent<PositionChanger>().SetTarget(new Vector2(0, -0.5F));

        foreach (var winGlass in _winGlasses)
            if (winGlass.transform.localPosition.y == 0.3F)
                winGlass.GetComponent<PositionChanger>().SetTarget(new Vector2(0, 0.5F));
        foreach (var rightGlass in _winGlasses)
            yield return Helper.WaitUntilChangerDone(rightGlass);

        yield return new WaitForSeconds(0.5F);

        foreach (var rightGlass in _winGlasses)
            rightGlass.GetComponent<PositionChanger>().SetTarget(new Vector2(0, -0.5F));
        foreach (var rightGlass in _winGlasses)
            yield return Helper.WaitUntilChangerDone(rightGlass);

        //TODO: Fade Out And Game Over
        Debug.Log("Game Over");
    }

    public void ActionOnClick(GameObject glass)
    {
        if (!_canClick || glass.transform.localPosition.y != 0.3F) return;

        glass.GetComponent<PositionChanger>().SetTarget(new Vector2(0, 0.5F));

        IEnumerator actionToStart = null;
        if (Helper.BoyIn(glass))
        {
            if (++_clickedBoysCount == _maxBoysCount)
                actionToStart = Win();
        }
        else
            actionToStart = Lose(glass);

        if (actionToStart == null) return;
        _canClick = false;
        StartCoroutine(actionToStart);
    }
}