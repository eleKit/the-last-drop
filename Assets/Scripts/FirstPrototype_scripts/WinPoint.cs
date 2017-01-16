using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinPoint : MonoBehaviour
{
	public GameObject m_plant_animation;

	public ColorTrees m_trees_coloured;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}



	void OnTriggerEnter2D (Collider2D other)
	{
		Debug.Log ("You win");
		GameObject go = POLIMIGameCollective.ObjectPoolingManager.Instance.GetObject (m_plant_animation.name);
		go.transform.position = this.gameObject.transform.position;
		go.transform.rotation = this.gameObject.transform.rotation;

		m_trees_coloured.ColorTree ();
		//TODO SoundManager.Instance.WinGingle ();
		gameObject.SetActive (false);
		GameWinManager.Instance.WinLevel ();
	}
}
