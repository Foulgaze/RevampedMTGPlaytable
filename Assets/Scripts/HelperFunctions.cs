using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperFunctions : MonoBehaviour
{
    public static Vector3 GetExtents(Transform t)
    {
        return t.GetComponent<Renderer>().bounds.extents;
    }

    public static bool IsPointInRectTransform( Vector2 screenPoint, RectTransform transform, Camera canvasCamera )
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle( transform, screenPoint, canvasCamera, out localPoint );
        return transform.rect.Contains( localPoint );
    }
    public static void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void KillChildren(Transform t)
    {
        if(t == null)
        {
            Debug.LogError("Trying to kill null children");
            return;
        }
        foreach(Transform child in t.transform)
		{
			GameObject.Destroy(child.gameObject);
		}
    }
    public static bool IsHoldingCTRL()
    {
        return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    }

    public static int ChangeIntputField(string text,bool? increase )
    {
        int result;
        if(!int.TryParse(text, out result))
        {
            return 0;
        }
        if(increase == null)
        {
            return result;
        }
        return (bool) increase ? result + 1 : result - 1;
    }
}
