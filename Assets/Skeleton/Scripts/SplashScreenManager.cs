using UnityEngine;
using System.Collections;

public class SplashScreenManager : MonoBehaviour {

	public GameObject[] screens = new GameObject[2];
	public float m_splashscreen_totaltime = 4f;
	private float m_splashscreen_showtime = 0f;

	void OnEnable() {
		for (int i = 0; i < screens.Length; i++)
			screens [i].SetActive(false);
		
		m_splashscreen_showtime = m_splashscreen_totaltime / screens.Length;
//		Debug.Log (m_splashscreen_showtime);

		StartCoroutine (FadeScreens ());
	}

	IEnumerator FadeScreens() {
		for (int i = 0; i < screens.Length; i++) {
			screens [i].SetActive(true);
			yield return new WaitForSeconds (m_splashscreen_showtime);
			screens [i].SetActive(false);
			yield return null;
		}
		MenuManager.Instance.SwitchToMainMenu ();
	}

	void OnDisable() {
		for (int i = 0; i < screens.Length; i++)
			screens [i].SetActive (false);
	}


}
