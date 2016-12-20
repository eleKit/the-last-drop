using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
	[Range (0f, 200f)]
	public float timeLeft = 0f;


	[Range (0f, 50f)]
	public float timeAdd = 0f;

	//public Text text;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		timeLeft -= Time.deltaTime;
		GameWinManager.Instance.m_timer_text.text = Mathf.Round (timeLeft).ToString () + " s";

		//text.text = "Time Left:" + Mathf.Round (timeLeft);

		if (timeLeft < 0) {
			GameWinManager.Instance.LoseLevel ();
		}
		
	}

	public void AddTime ()
	{
		timeLeft = timeLeft + timeAdd;
	}
}