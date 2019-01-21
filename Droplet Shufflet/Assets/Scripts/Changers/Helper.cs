using System.Collections;
using UnityEngine;

public class Helper
{
    public void SaveInArrayFromParent(GameObject parent, ref GameObject[] objectsArray)
    {
        var childCount = parent.transform.childCount;
        objectsArray = new GameObject[childCount];

        for (var i = 0; i < childCount; i++)
        {
            objectsArray[i] = parent.transform.GetChild(i).gameObject;
//            objectToChange.transform.parent = _shadows[shadowNumber].transform;
        }
    }

    public void SetParentAndY(GameObject child, GameObject parent, float y)
    {
        child.transform.parent = parent.transform;
        child.transform.localPosition = new Vector2(0, y);
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