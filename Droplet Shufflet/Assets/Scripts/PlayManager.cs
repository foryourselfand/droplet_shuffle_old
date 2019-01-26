using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private List<GameObject> _allBoys;
    private List<GameObject> _currentBoys;
    private GameObject[] _fingers;
    private GameObject[] _clickedGlasses;
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
    private bool _canClick;
    private int _lastBorder = -1;

    #endregion

    public GameObject CameraChanger;

    private void Awake()
    {
        Helper.SaveFromParentToArray(ShadowsParent, ref _shadows);
        Helper.SaveFromParentToArray(GlassesParent, ref _glasses);
        Helper.SaveFromParentToArray(FingersParent, ref _fingers);
        Helper.SaveFromParentToList(BoysParent, ref _allBoys);
        _currentBoys = new List<GameObject>();

        _maxBoysCount = 2;
        _clickedGlasses = new GameObject[_allBoys.Count];
    }

    private void Start()
    {
        DefineOnStart();

        //TODO: Wait Until Shadow Done
        ActionAfterShadowDone();
    }

    private void DefineOnStart()
    {
        CameraChanger.GetComponent<CameraChanger>().SetCurrent(_maxBoysCount + 1);

        foreach (var boy in _allBoys)
            boy.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

        for (var i = 0; i < _glasses.Length; i++)
            Helper.SetParentAndY(_glasses[i], _shadows[i], 0.3F);

        for (var i = 0; i < _maxBoysCount; i++)
        {
            var currentBoyNumber = Random.Range(0, _allBoys.Count);

            GameObject freshBoy;
            Helper.SaveRemoveFromAndAddTo(out freshBoy, ref _allBoys, ref _currentBoys, currentBoyNumber);

            while (true)
            {
                var currentShadowIndex = Random.Range(_leftBorder, _rightBorder + 1);

                if (_shadows[currentShadowIndex].transform.childCount != 1) continue;
                Helper.SetParentAndY(freshBoy, _shadows[currentShadowIndex], 0.12F);

                break;
            }
        }

        _winGlasses = _glasses.Where(Helper.BoyIn).ToList();
        _loseGlasses = _glasses.Where(Helper.BoyOut).ToList();

//        foreach (var tempGlass in _winGlasses)
//            Debug.Log(tempGlass.name);
//        
//        Debug.Log("SPACE");
//        
//        foreach (var tempGlass in _loseGlasses)
//            Debug.Log(tempGlass.name);

        foreach (var finger in _fingers)
            finger.GetComponent<OpacityChanger>().SetCurrent(0);

        foreach (var boy in _allBoys)
            boy.SetActive(false);

        Helper.StartDeactivationInBounds(ref _shadows, _leftBorder, _rightBorder);
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
        var checker = _maxLevel - 1;
        if (checker != 0 && checker % _maxBoysCount == 0 && _maxBoysCount != 4)
        {
            _maxDistance++;

            bool condition;
            if (_lastBorder == -1)
                condition = Random.Range(0, 2) == 0;
            else
                condition = _lastBorder == _leftBorder;

            _lastBorder = condition ? ++_rightBorder : --_leftBorder;

            _shadows[_lastBorder].SetActive(true);
            _glasses[_lastBorder].SetActive(true);


            var byX = 0.5F;
            byX *= condition ? -1 : 1;
            ShadowsParent.GetComponent<PositionChanger>().SetTarget(new Vector3(byX, 0));

            CameraChanger.GetComponent<CameraChanger>().SetTarget(1);

            yield return Helper.WaitUntilChangerDone(ShadowsParent);
            yield return Helper.WaitUntilChangerDone(CameraChanger);

            if (checker % (_maxBoysCount * 2) == 0)
            {
                _maxBoysCount++;
                _maxLevel = 1;

                var currentBoyNumber = Random.Range(0, _allBoys.Count);

                GameObject freshBoy;
                Helper.SaveRemoveFromAndAddTo(out freshBoy, ref _allBoys, ref _currentBoys, currentBoyNumber);

                freshBoy.SetActive(true);
                Helper.SetParentAndY(freshBoy, _shadows[_lastBorder], 0.12F);

                yield return MoveGlassesAndJump(_maxBoysCount);
            }
            else
            {
                var emptyGlass = _glasses[_lastBorder];
                _loseGlasses.Add(emptyGlass);
                emptyGlass.GetComponent<PositionChanger>().SetTarget(new Vector3(0, 0.5F));
                yield return Helper.WaitUntilChangerDone(emptyGlass);

                yield return new WaitForSeconds(0.1F);

                emptyGlass.GetComponent<PositionChanger>().SetTarget(new Vector3(0, -0.5F));
                yield return Helper.WaitUntilChangerDone(emptyGlass);
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

        yield return Helper.WaitUntilChangerDone(_fingers[0]);
    }

    private IEnumerator JumpBoys(int jumpCount)
    {
        var directory = 0.1F;
        var active = true;
        for (var i = 0; i < jumpCount * 2; i++)
        {
            foreach (var boy in _currentBoys)
            {
                boy.transform.localPosition += new Vector3(0, directory, 0);
                boy.transform.GetChild(0).GetChild(0).gameObject.SetActive(active);
            }

            directory *= -1;
            active = !active;

            yield return new WaitForSeconds(1F / jumpCount);
        }
    }

    private IEnumerator Win()
    {
        for (var i = 0; i < _maxBoysCount; i++)
            yield return Helper.WaitUntilChangerDone(_clickedGlasses[i]);

        yield return JumpBoys(_maxBoysCount);

        yield return Helper.MoveGlasses(_clickedGlasses, -0.5F, 0, _maxBoysCount);

        _clickedBoysCount = 0;
        _maxLevel++;

        StartCoroutine(CheckLevel());
    }

    private IEnumerator Lose()
    {
        var loseGlass = _clickedGlasses[_clickedBoysCount];

        yield return Helper.WaitUntilChangerDone(loseGlass);

        yield return new WaitForSeconds(0.5F);

//        loseGlass.GetComponent<PositionChanger>().SetTarget(new Vector2(0, -0.5F));
//
//        var rightGlasses = _glasses.Where(Helper.BoyIn).ToList();
//
//        foreach (var rightGlass in rightGlasses)
//            if (rightGlass.transform.localPosition.y == 0.3F)
//                rightGlass.GetComponent<PositionChanger>().SetTarget(new Vector2(0, 0.5F));
//        foreach (var rightGlass in rightGlasses)
//            yield return Helper.WaitUntilChangerDone(rightGlass);
//
//        yield return new WaitForSeconds(0.5F);
//
//        foreach (var rightGlass in rightGlasses)
//            rightGlass.GetComponent<PositionChanger>().SetTarget(new Vector2(0, -0.5F));
//        foreach (var rightGlass in rightGlasses)
//            yield return Helper.WaitUntilChangerDone(rightGlass);

        //TODO: Fade Out And Game Over
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