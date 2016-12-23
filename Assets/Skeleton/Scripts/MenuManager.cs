using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using POLIMIGameCollective;

public class MenuManager : Singleton<MenuManager>
{

	public GameObject	m_splashscreen;
	public GameObject	m_mainmenu;
	public GameObject	m_tutorial;
	public GameObject	m_score;
	public GameObject	m_settings;
	public GameObject	m_about;
	public GameObject	m_playlevel;

	public enum eMenuScreen
	{
SplashScreen = 0,
		MainMenu = 1,
		Tutorial = 2,
		Score = 3,
		Settings = 4,
		About = 5,
		PlayLevel = 6}

	;

	[Header ("Start with Splashscreen?")]
	public bool m_start_with_splashscreen = true;

	private static bool m_has_shown_splashscreen = false;
	// Use this for initialization
	void Start ()
	{
		if (!m_has_shown_splashscreen && m_start_with_splashscreen) {
			SwitchMenuTo (eMenuScreen.SplashScreen);
			m_has_shown_splashscreen = true;
		} else {
			SwitchMenuTo (eMenuScreen.MainMenu);
		}
		MusicManager.Instance.StopAll ();
		MusicManager.Instance.PlayMusic ("MenuMusic");
	}

	/// <summary>
	/// Switch the current screen to the target one
	/// </summary>
	/// <param name="screen">Screen.</param>
	public void SwitchMenuTo (eMenuScreen screen)
	{
		ClearScreens ();
		switch (screen) {
		case eMenuScreen.SplashScreen:
			if (m_splashscreen != null)
				m_splashscreen.SetActive (true);
			break;
		case eMenuScreen.MainMenu:
			if (m_mainmenu != null)
				m_mainmenu.SetActive (true);
			break;
		case eMenuScreen.Tutorial:
			if (m_tutorial != null)
				m_tutorial.SetActive (true);
			break;
		case eMenuScreen.Score:
			if (m_score != null)
				m_score.SetActive (true);
			break;
		case eMenuScreen.Settings:
			if (m_settings != null)
				m_settings.SetActive (true);
			break;
		case eMenuScreen.About:
			if (m_about != null)
				m_about.SetActive (true);
			break;
		}
	}

	/// <summary>
	/// Clear all the screens
	/// </summary>
	void ClearScreens ()
	{
		if (m_splashscreen != null)
			m_splashscreen.SetActive (false);
		if (m_mainmenu != null)
			m_mainmenu.SetActive (false);
		if (m_tutorial != null)
			m_tutorial.SetActive (false);
		if (m_score != null)
			m_score.SetActive (false);
		if (m_settings != null)
			m_settings.SetActive (false);
		if (m_about != null)
			m_about.SetActive (false);
		if (m_playlevel != null)
			m_playlevel.SetActive (false);
	}

	/// <summary>
	/// Returns to the main screen
	/// </summary>
	public void SwitchToMainMenu ()
	{
		SwitchMenuTo (eMenuScreen.MainMenu);
	}


	/// <summary>
	/// Switch to the tutorial screen
	/// </summary>
	public void SwitchToTutorial ()
	{
		SwitchMenuTo (eMenuScreen.Tutorial);
	}


	/// <summary>
	/// Switch to the tutorial score screen
	/// </summary>
	public void SwitchToScore ()
	{
		SwitchMenuTo (eMenuScreen.Score);
	}

	public void SwitchToAbout ()
	{
		SwitchMenuTo (eMenuScreen.About);
	}

	public void SwitchToSettings ()
	{
		SwitchMenuTo (eMenuScreen.Settings);
	}

	public void Play ()
	{
		MusicManager.Instance.StopAll ();
		MusicManager.Instance.PlayMusic ("GameplayMusic");
		SceneManager.LoadScene ("DropGame");
	}

}
