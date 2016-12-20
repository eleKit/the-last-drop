using UnityEngine;
using System.Collections;

public class Gravity : MonoBehaviour
{

	float nextUsage;
	public float delay = 2f;
	public float gravityValue = 9.81f;
	
	Vector3 gravity;

	private Vector2 touchOrigin = -Vector2.one;

	void Start ()
	{
		gravity = Physics2D.gravity;
		gravity = new Vector3 (0f, -gravityValue, 0f);
		nextUsage = Time.time + delay;
	}

	void Update ()
	{
		changeGravity ();
		Physics2D.gravity = gravity;
	}

	// Reset the gravity to point down
	public void ResetGravity ()
	{
		gravity = new Vector3 (0f, -gravityValue, 0f);
		Physics2D.gravity = gravity;
	}

	void changeGravity ()
	{

		//#if UNITY_STANDALONE || UNITY_WEBPLAYER  Unity3D editor or web player

		if (Input.GetKeyDown (KeyCode.W) && Time.time > nextUsage) {
			nextUsage = Time.time + delay;
			gravity = new Vector3 (0f, gravityValue, 0f);
		}
		if (Input.GetKeyDown (KeyCode.A) && Time.time > nextUsage) {
			nextUsage = Time.time + delay;
			gravity = new Vector3 (-gravityValue, 0f, 0f);
		}
		if (Input.GetKeyDown (KeyCode.S) && Time.time > nextUsage) {
			nextUsage = Time.time + delay;
			gravity = new Vector3 (0f, -gravityValue, 0f);
		}
		if (Input.GetKeyDown (KeyCode.D) && Time.time > nextUsage) {
			nextUsage = Time.time + delay;
			gravity = new Vector3 (gravityValue, 0f, 0f);
		}

		//#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE // mobile controls

		/*int horizontal = 0;
		int vertical = 0;

		if (Input.touchCount > 0) {
        	Touch myTouch = Input.touches[0];

			if (myTouch.phase == TouchPhase.Began) {
            	touchOrigin = myTouch.position;
            } else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0) {
            	Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;

                touchOrigin.x = -1;
			if (Mathf.Abs(x) > Mathf.Abs(y))
            	horizontal = x > 0 ? 1 : -1;
        	else
				vertical = y > 0 ? 1 : -1;
            }
        }

		if (vertical > 0 && Time.time > nextUsage) {
     		nextUsage = Time.time + delay;
			gravity = new Vector3(0f, gravityValue, 0f);
		}
		if (horizontal < 0 && Time.time > nextUsage) {
     		nextUsage = Time.time + delay;
			gravity = new Vector3(-gravityValue, 0f, 0f);
		}
		if (vertical < 0 && Time.time > nextUsage) {
     		nextUsage = Time.time + delay;
			gravity = new Vector3(0f, -gravityValue, 0f);
		}
		if (horizontal > 0 && Time.time > nextUsage) {
     		nextUsage = Time.time + delay;
			gravity = new Vector3(gravityValue, 0f, 0f);
		}

		#endif*/
	}
}
