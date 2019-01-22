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

    public static IEnumerator MoveGlassesAndWaitForDone(GameObject[] glasses, float byY, int length)
    {
        for (var i = 0; i < length; i++)
        {
            var glass = glasses[i];
            glass.GetComponent<PositionChanger>().SetTarget(new Vector2(0, byY));
        }

        yield return WaitUntilChangerDone(glasses[0]);
    }

    public static IEnumerator WaitUntilChangerDone(GameObject objectToWait)
    {
        yield return new WaitUntil(() => objectToWait.GetComponent<Changer>().IsDone());
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
}