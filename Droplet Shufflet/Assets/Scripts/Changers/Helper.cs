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
        }
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