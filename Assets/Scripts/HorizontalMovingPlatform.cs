using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalMovingPlatform : MonoBehaviour
{

	[Range (-1, 1)]
	public int dir = 1;


	Transform tr;

	// Use this for initialization
	void Start ()
	{
		tr = gameObject.GetComponent<Transform> ();
		//Remember! PinPong Always starts at 0 coordinates!
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		tr.position = new Vector3 (dir * Mathf.PingPong (Time.time, 3), tr.position.y, tr.position.z);
		
	}
}
