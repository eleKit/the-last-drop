using UnityEngine;
using System.Collections;

public class DeadlyPlatform : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{

	}

	void OnCollisionEnter2D (Collision2D collision)
	{
		//TODO check if setActive false the object or start an animation
		collision.gameObject.SetActive (false);
		GameWinManager.Instance.LoseLevel ();
	}
}
