using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalMovingPlatform : MonoBehaviour
{

	Transform tr;

	// Use this for initialization
	void Start ()
	{
		tr = gameObject.GetComponent<Transform> ();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		tr.position = new Vector3 (Mathf.PingPong (Time.time, 3), tr.position.y, tr.position.z);
		
	}
}
