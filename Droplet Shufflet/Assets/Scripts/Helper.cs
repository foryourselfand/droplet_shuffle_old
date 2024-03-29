using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static void SaveFromParentToArray(GameObject parent, ref GameObject[] objectsArray)
    {
        var childCount = parent.transform.childCount;
        objectsArray = new GameObject[childCount];

        for (var i = 0; i < childCount; i++)
            objectsArray[i] = parent.transform.GetChild(i).gameObject;
    }

    public static void SaveFromParentToList(GameObject parent, ref List<GameObject> objectsArray)
    {
        var childCount = parent.transform.childCount;
        objectsArray = new List<GameObject>();

        for (var i = 0; i < childCount; i++)
            objectsArray.Add(parent.transform.GetChild(i).gameObject);
    }

    public static void SetParentAndY(GameObject child, GameObject parent, float y)
    {
        child.transform.parent = parent.transform;
        child.transform.localPosition = new Vector2(0, y);
    }

    public static GameObject GetDeActiveBoyFrom(List<GameObject> boys)
    {
        foreach (var boy in boys)
        {
            if (boy.activeSelf) continue;
            return boy;
        }

        return null;
    }

    public static bool BoyIn(GameObject glass)
    {
        foreach (Transform child in glass.transform.parent)
            if (child.CompareTag("Boy"))
                return true;

        return false;
    }

    public static void Swap(ref GameObject first, ref GameObject second)
    {
        var temp = first;
        first = second;
        second = temp;
    }

    public static IEnumerator MoveGlasses(List<GameObject> glasses, float y)
    {
        foreach (var glass in glasses)
            glass.GetComponent<PositionChanger>().SetTarget(new Vector2(0, y));

        yield return WaitUntilChangerDone(glasses[0]);
    }

    public static IEnumerator MoveShadows(GameObject[] shadows, int firstShadow, int distance,
        int multiply)
    {
        shadows[firstShadow].GetComponent<PositionChanger>()
            .SetTarget(new Vector2(distance * 0.5F, multiply * 0.5F + multiply * (distance - 1) * 0.1F),
                (distance - 1) / 2F);

        shadows[firstShadow + distance].GetComponent<PositionChanger>()
            .SetTarget(new Vector2(distance * -0.5F, -multiply * 0.5F - multiply * (distance - 1) * 0.1F),
                (distance - 1) / 2F);

        yield return WaitUntilChangerDone(shadows[firstShadow]);
    }

    public static IEnumerator WaitUntilChangerDone(GameObject objectToWait)
    {
        yield return new WaitUntil(() => objectToWait.GetComponent<_Changer>().IsDone());
    }
}