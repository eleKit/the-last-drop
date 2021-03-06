﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearPlatform : MonoBehaviour
{
	[Range (0f, 4f)]
	public float fallDelay = 1f;

	private Rigidbody2D rb2d;
	private Collider2D coll2d;


	void Awake ()
	{
		rb2d = GetComponent<Rigidbody2D> ();
		coll2d = GetComponent<Collider2D> ();
	}




	void OnCollisionEnter2D (Collision2D other)
	{
		if (other.gameObject.CompareTag ("Player")) {
			StartCoroutine ("Fall");
		}

	}

	IEnumerator Fall ()
	{
		yield return new WaitForSeconds (fallDelay);

		rb2d.isKinematic = false;
		Destroy (coll2d);

		yield return new WaitForSeconds (2.5f);
		gameObject.SetActive (false);
	}
}
