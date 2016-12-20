using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoAutorotation : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{

		Screen.autorotateToPortrait = true;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.autorotateToLandscapeLeft = false;
		Screen.autorotateToLandscapeRight = false;
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
