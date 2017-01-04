using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class WindSound : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		SfxManager.Instance.Play ("wind");
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
