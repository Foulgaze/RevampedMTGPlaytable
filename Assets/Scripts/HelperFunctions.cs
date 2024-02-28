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
    void ShuffleList<T>(List<T> list)
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
}
