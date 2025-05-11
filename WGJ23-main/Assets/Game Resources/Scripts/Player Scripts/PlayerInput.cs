using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    internal PlayerManager manager;

    public static event Action swiped;

    private Vector2 tStartPos, tEndPos;
    private Touch touch;
    internal string dir;

    private void Awake()
    {
        manager = GetComponent<PlayerManager>();
    }

    void Update()
    {
        if (!GameManager.instance.pause)
        {
            pcControls();
            touchControls();
        }

    }

    void touchControls()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                tStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                tEndPos = touch.position;

                //determine direction
                dir = determineSwipeDir(tStartPos, tEndPos);

                swiped?.Invoke();
            }
        }
    }
    string determineSwipeDir(Vector2 startPos, Vector2 endPos)
    {
        float horizontal, vertical;
        horizontal = endPos.x - startPos.x;
        vertical = endPos.y - startPos.y;

        if(Mathf.Abs(horizontal) > Mathf.Abs(vertical)) {
            if(horizontal > 0)
            {
                return "right";
            }
            else
            {
                return "left";
            }
        }
        else
        {
            if (vertical > 0)
            {
                return "up";
            }
            else
            {
                return "down";
            }
        }
    }

    void pcControls()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            dir = "left";
            swiped?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            dir = "right";
            swiped?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            dir = "up";
            swiped?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            dir = "down";
            swiped?.Invoke();
        }
    }
}
