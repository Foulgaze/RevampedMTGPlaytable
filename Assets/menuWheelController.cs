using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuWheelController : MonoBehaviour
{
    [SerializeField] RectTransform startingPosition;
    [SerializeField] RectTransform circleSymbol;
    [SerializeField] RectTransform circle;

    [SerializeField] List<Sprite> sprites= new List<Sprite>();

    float radius;
    float time = 0;
    int currentIndex = 0;
    bool inWindow = true;

    void Start()
    {
        radius = Vector2.Distance(startingPosition.transform.position, circle.transform.position);
    }

    Vector2 calculateNewCirclePosition(float time)
    {
        Vector2 newPosition = circle.transform.position + new Vector3(radius * Mathf.Cos(time), radius * Mathf.Sin(time));
        if(circleSymbol.anchoredPosition.y > 0 && newPosition.x > Screen.width/2)
        {
            if(inWindow)
            {
                currentIndex += 1;
                currentIndex %= sprites.Count;
                circleSymbol.GetComponent<Image>().sprite = sprites[currentIndex];
                inWindow = false;
            }
        }
        else
        {
            inWindow = true;
        }
        return newPosition;
    }

    void FixedUpdate()
    {
        circleSymbol.transform.position = calculateNewCirclePosition(time);
        time += Time.deltaTime*2;
    }
}
