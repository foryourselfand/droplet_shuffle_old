using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class Helper
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

    public static void SaveRemoveFromAndAddTo(out GameObject temp, ref List<GameObject> from, ref List<GameObject> to,
        int index)
    {
        temp = from[index];
        to.Add(temp);
        from.RemoveAt(index);
    }

    public static IEnumerator MoveGlasses(GameObject[] glasses, float y, int leftBorder, int rightBorder)
    {
        for (var i = leftBorder; i < rightBorder; i++)
            glasses[i].GetComponent<PositionChanger>().SetTarget(new Vector2(0, y));

        yield return WaitUntilMoveDone(glasses[leftBorder]);
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

        yield return WaitUntilMoveDone(shadows[firstShadow]);
    }

    public static IEnumerator WaitUntilChangerDone<T>(GameObject objectToWait)
    {
        yield return new WaitUntil(() => (objectToWait.GetComponent<T>() as Changer).IsDone());
    }

    public static IEnumerator WaitUntilMoveDone(GameObject objectToWait)
    {
        yield return WaitUntilChangerDone<PositionChanger>(objectToWait);
    }

    public static IEnumerator WaitUntilFadeDone(GameObject objectToWait)
    {
        yield return WaitUntilChangerDone<OpacityChanger>(objectToWait);
    }
}