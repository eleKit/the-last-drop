using UnityEngine;
using System.Collections;

public class CoinCollect : MonoBehaviour
{
	public Timer timer;

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
		this.gameObject.SetActive (false);
		timer.AddTime ();
		Debug.Log ("Collect coin");
	}
}
