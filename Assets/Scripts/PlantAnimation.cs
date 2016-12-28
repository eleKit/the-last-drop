﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantAnimation : MonoBehaviour
{

	// Use this for initialization
	void OnEnable ()
	{
		StartCoroutine (WaitForAnimation ());
	}

	IEnumerator WaitForAnimation ()
	{
		yield return new WaitForSeconds (2.5f);
		gameObject.SetActive (false);
	}
}
