using UnityEngine;
using System.Collections;

public class DeadlyPlatform : MonoBehaviour
{

	void OnCollisionEnter2D (Collision2D collision)
	{
		if (collision.gameObject.tag == "Player") {
			GameManager.Instance.m_Player.GetComponent<PlayerAvatar_02> ().Deactivate_Particle (collision.gameObject);
		}

        /*   DONT USE THIS SHIT ON MY PLAYER, READ GIT HISTORY ON WHY! OR ASK ME WHY!!! SECOND AND LAST WARNING!!!!
         *   
		collision.gameObject.SetActive (false);
		if (collision.gameObject.name == "Ball") {
			GameWinManager.Instance.LoseLevel ();
			Debug.Log ("U ded");
		}
        */
	}
}
