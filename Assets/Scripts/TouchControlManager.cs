using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class TouchControlManager : Singleton<TouchControlManager>
{

    private Rect leftR;
    private Rect rightR;

    // [Left portion] Move info 
    public Vector2 moveStartPos;
    public Vector2 moveDirection;
    private bool moving;
    private float maxMovRange;
    private float shrinkVectorFactor = 0.015f;

    // [Right portion] Vector representing swipe input
    public Vector2 swipeVector;
    private Vector2 touchOrigin;

    void Start()
    {
        // These rectangles represent the two halfs of the screen
        leftR = new Rect(0f, 0f, Screen.width * 0.5f, Screen.height);
        rightR = new Rect(Screen.width * 0.5f, 0f, Screen.width * 0.5f, Screen.height);

        maxMovRange = Screen.width * 0.05f;
    }

    void Update()
    {
        TouchInputTrigger();
    }

    void TouchInputTrigger()
    {
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // mobile controls
        // reset value each frame
        // swipeVector = Vector2.zero;

        if (Input.touchCount > 0)
        {
            for(int i=0; i < Input.touchCount; i++)
            {
                Touch touch = Input.touches[i];

                // Left portion of the screen: movement
                if (leftR.Contains(touch.position))
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            moving = true;
                            moveStartPos = touch.position;
                            EventManager.TriggerEvent("MoveStart");
                            break;
                        case TouchPhase.Moved:
                            moving = true;
                            Vector2 tempDirection = touch.position - moveStartPos;
                            moveDirection = tempDirection.magnitude > maxMovRange ? tempDirection.normalized * maxMovRange * shrinkVectorFactor : tempDirection * shrinkVectorFactor;
                            break;
                        case TouchPhase.Ended:
                            moving = false;
                            EventManager.TriggerEvent("MoveEnd");
                            break;
                    }
                    // Right portion of the screen: stretch mechanic
                } 
                else if (rightR.Contains(touch.position))
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        touchOrigin = touch.position;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        Vector2 touchEnd = touch.position;
                        swipeVector = touchEnd - touchOrigin;

                        // Trigger event: i.e. swipeVector has changed 
                        EventManager.TriggerEvent("Swipe");

                        if (moving)
                        {
                            moving = false;
                            EventManager.TriggerEvent("MoveEnd");
                        }
                    }
                }
            }
        }
#endif
    }

    // Ignore if using TriggerEvent
    public Vector2 GetSwipeVector()
    {
        return swipeVector;
    }

    public bool IsSwiping()
    {
        return swipeVector.magnitude != 0f;
    }

}
