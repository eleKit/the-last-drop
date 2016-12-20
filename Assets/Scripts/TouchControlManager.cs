using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class TouchControlManager : Singleton<TouchControlManager> {

	private Rect leftR;
	private Rect rightR;
	private Vector2 touchOrigin = -Vector2.one;

	// [Left portion] Move info 
	public Vector2 moveStartPos;
	public Vector2 moveDirection;
	// [Right portion] Vector representing swipe input
	public Vector2 swipeVector;
	
	void Start() {
		// These rectangles represent the two halfs of the screen
		leftR = new Rect(0f, 0f, Screen.width * 0.5f, Screen.height);
     	rightR = new Rect(Screen.width * 0.5f, 0f, Screen.width * 0.5f, Screen.height);
	}

	void Update () {
		TouchInputTrigger();
	}

	// Import this in GameManager
	void TouchInputTrigger() {
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // mobile controls
		// reset value each frame
		swipeVector = Vector2.zero;

		if (Input.touchCount > 0) {
			
			Touch touch = Input.touches[0];

			// Left portion of the screen: movement
			if(leftR.Contains(touch.position)) {
				switch(touch.phase) {
					case TouchPhase.Began:
						moveStartPos = touch.position;
						EventManager.TriggerEvent("MoveStart");
						break;
					case TouchPhase.Moved:
						moveDirection = touch.position - moveStartPos;
						break;
					case TouchPhase.Ended:
						EventManager.TriggerEvent("MoveEnd");
						break;
				}		
            }

			// Right portion of the screen: stretch mechanic
        	if(rightR.Contains(touch.position)) {
				if (touch.phase == TouchPhase.Began) {
            		touchOrigin = touch.position;
            	} else if (touch.phase == TouchPhase.Ended && touchOrigin.x >= 0) {
            		Vector2 touchEnd = touch.position;                    
                	swipeVector = new Vector2(touchEnd.x - touchOrigin.x, touchEnd.y - touchOrigin.y);
					touchOrigin.x = -1;
					// Trigger event: i.e. swipeVector has changed 
					EventManager.TriggerEvent("Swipe");
            	}
			}
        }
#endif
	}

	// Ignore if using TriggerEvent
	public Vector2 GetSwipeVector() {
		return swipeVector;		
	}

	public bool IsSwiping() {
		return swipeVector.magnitude != 0f;
	}

}
