using UnityEngine;
using System.Collections;

public class Gravity : MonoBehaviour
{

	float nextUsage;
	public float delay = 2f;
	public float gravityValue = 9.81f;

	private Vector2 touchOrigin = -Vector2.one;

	void Start ()
	{
		Physics2D.gravity = new Vector3 (0f, -gravityValue, 0f);
	}

	void Update ()
	{
		changeGravity ();
	}

	// Reset the gravity to point down
	public void ResetGravity ()
	{
		Physics2D.gravity = new Vector3 (0f, -gravityValue, 0f);
	}

	void changeGravity ()
	{

		//#if UNITY_EDITOR  //Unity3D editor or web player

		if (Input.GetKeyDown (KeyCode.W)) {
			Physics2D.gravity = new Vector3 (0f, gravityValue, 0f);
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			Physics2D.gravity = new Vector3 (-gravityValue, 0f, 0f);
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			Physics2D.gravity = new Vector3 (0f, -gravityValue, 0f);
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			Physics2D.gravity = new Vector3 (gravityValue, 0f, 0f);
		}

		//#else  // mobile controls

		Vector3 deviceAcc = new Vector3 (Input.acceleration.x, Input.acceleration.y, 0) * 9.81f;
		Vector3 downPull = Vector3.down * 4.81f;
		Vector3 res = (deviceAcc + downPull);
		Physics2D.gravity = res.normalized * 9.81f;



		//#endif
	}
}
