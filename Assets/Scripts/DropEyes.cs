using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropEyes : MonoBehaviour
{

	public GameObject Drop;

	Transform tr;

	// Use this for initialization
	void Start ()
	{

		tr = gameObject.GetComponent<Transform> ();


		
	}
	
	// Update is called once per frame
	void Update ()
	{
		tr.position = Drop.gameObject.transform.position;
		
	}
}
