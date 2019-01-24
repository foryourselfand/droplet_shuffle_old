using System.Collections;
using UnityEngine;

public class Helper
{
    public static void SaveInArrayFromParent(GameObject parent, ref GameObject[] objectsArray)
    {
        var childCount = parent.transform.childCount;
        objectsArray = new GameObject[childCount];

        for (var i = 0; i < childCount; i++)
            objectsArray[i] = parent.transform.GetChild(i).gameObject;
    }

    public static void SetParentAndY(GameObject child, GameObject parent, float y)
    {
        child.transform.parent = parent.transform;
        child.transform.localPosition = new Vector2(0, y);
    }

    public static void SetStartOpacityToBounds(ref GameObject[] gameObjects, int leftBound, int rightBound)
    {
        for (var i = 0; i < leftBound; i++)
            gameObjects[i].GetComponent<OpacityChanger>().SetCurrent(0);

        for (var i = rightBound + 1; i < gameObjects.Length; i++)
            gameObjects[i].GetComponent<OpacityChanger>().SetCurrent(0);
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

    public static IEnumerator MoveGlassesAndWaitForDone(GameObject[] glasses, float y, int leftBorder, int rightBorder)
    {
        for (var i = leftBorder; i < rightBorder; i++)
            glasses[i].GetComponent<PositionChanger>().SetTarget(new Vector2(0, y));

        yield return WaitUntilPositionChangerDone(glasses[leftBorder]);
    }

    public static IEnumerator MoveShadowsAndWaitForDone(GameObject[] shadows, int firstShadow, int distance,
        int multiply)
    {
        shadows[firstShadow].GetComponent<PositionChanger>()
            .SetTarget(new Vector2(distance * 0.5F, multiply * 0.5F));

        shadows[firstShadow + distance].GetComponent<PositionChanger>()
            .SetTarget(new Vector2(distance * -0.5F, -multiply * 0.5F));

        yield return WaitUntilPositionChangerDone(shadows[firstShadow]);
    }

    public static IEnumerator WaitUntilPositionChangerDone(GameObject objectToWait)
    {
        yield return new WaitUntil(() => objectToWait.GetComponent<PositionChanger>().IsDone());
    }

    public static IEnumerator WaitUntilOpacityChangerDone(GameObject objectToWait)
    {
        yield return new WaitUntil(() => objectToWait.GetComponent<OpacityChanger>().IsDone());
    }
}