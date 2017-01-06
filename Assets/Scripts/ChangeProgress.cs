using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ChangeProgress : MonoBehaviour
{
	//Remember you are working with an array, so count from zero!!
	private const int MaxLevel = 9;

	[Header ("Log Screen")]

	public GameObject m_log_screen;

	public Text m_log_text;

	public void ResetProgress ()
	{
		PlayerPrefs.DeleteKey ("LastWonLevel");
		m_log_text.text = "All progress have been deleted";

		StartCoroutine ("LogScreen");
	}


	public void Cheat ()
	{
		PlayerPrefs.SetInt ("LastWonLevel", MaxLevel);
		m_log_text.text = "All levels have been unlocked";

		StartCoroutine ("LogScreen");
	}


	IEnumerator LogScreen ()
	{
		m_log_screen.SetActive (true);

		yield return new WaitForSeconds (2f);

		m_log_screen.SetActive (false);
		
	}
}
