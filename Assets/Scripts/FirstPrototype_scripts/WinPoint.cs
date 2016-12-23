using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinPoint : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	void OnCollisionEnter2D (Collision2D other)
	{
		Debug.Log ("You win");
		GameWinManager.Instance.WinLevel ();
	}
}
