using System.Collections;
using UnityEngine;

public class Helper
{
    public void SaveInArrayFromParent(GameObject parent, ref GameObject[] objectsArray)
    {
        var childCount = parent.transform.childCount;
        objectsArray = new GameObject[childCount];

        for (var i = 0; i < childCount; i++)
            objectsArray[i] = parent.transform.GetChild(i).gameObject;
    }

    public void SetParentAndY(GameObject child, GameObject parent, float y)
    {
        child.transform.parent = parent.transform;
        child.transform.localPosition = new Vector2(0, y);
    }

    public IEnumerator MoveGlassesAndWaitForDone(GameObject[] glasses, float byY, int length)
    {
        for (var i = 0; i < length; i++)
        {
            var glass = glasses[i];
            glass.GetComponent<PositionChanger>().SetTarget(new Vector2(0, byY));
        }

        yield return WaitUntilChangerDone(glasses[0]);
    }

    public IEnumerator WaitUntilChangerDone(GameObject objectToWait)
    {
        yield return new WaitUntil(() => objectToWait.GetComponent<Changer>().IsDone());
    }

    public void Swap(ref GameObject first, ref GameObject second)
    {
        var temp = first;
        first = second;
        second = temp;
    }
}