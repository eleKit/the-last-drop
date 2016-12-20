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
		if (collision.gameObject.tag == "Player") {
			GameManager.Instance.m_Player.GetComponent<PlayerAvatar_02> ().Deactivate_Particle (collision.gameObject);
		}
		collision.gameObject.SetActive (false);
		if (collision.gameObject.name == "Ball") {
			GameWinManager.Instance.LoseLevel ();
			Debug.Log ("U ded");
		}
	}
}
