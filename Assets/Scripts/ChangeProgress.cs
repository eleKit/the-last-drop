using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeProgress : MonoBehaviour
{
	//Remember you are working with an array, so count from zero!!
	private const int MaxLevel = 9;

	public void ResetProgress ()
	{
		PlayerPrefs.DeleteKey ("LastWonLevel");
	}


	public void Cheat ()
	{
		PlayerPrefs.SetInt ("LastWonLevel", MaxLevel);
	}
}
