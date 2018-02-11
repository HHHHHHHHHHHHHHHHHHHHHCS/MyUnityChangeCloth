using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithMouse : MonoBehaviour
{
    [SerializeField]
    private float moveLength=5;
    private bool isClick;
    private Vector3 nowPos, oldPos;

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isClick = true;
        }
    }

    private void OnMouseUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isClick = false;
        }
    }

    private void Update()
    {
        nowPos = Input.mousePosition;
        if (isClick)
        {//鼠标按下没有松手
            Vector3 offest = nowPos - oldPos;
            if (Mathf.Abs(offest.x) > Mathf.Abs(offest.y)
                && Mathf.Abs(offest.x)>moveLength)
            {
                transform.Rotate(Vector3.up, -offest.x);
            }
        }
        oldPos = nowPos;
    }
}
