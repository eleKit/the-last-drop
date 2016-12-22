﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using POLIMIGameCollective;

// This class manages all the game architecture, loading a level, pausing,  restarting, winning, losing

public class GameWinManager : Singleton<GameWinManager>
{

	// index of current level
	private int current_level = 0;


	// index of the first level accesible by the player at the first opening
	private const int tutorial = 0;

	[Header ("Cheat Flag")]
	public bool cheat;


	[Header ("EndLevel Screen")]
	public GameObject m_endlevel_screen;

	[Header ("Timer Screen")]
	public GameObject m_timer_screen;
	public Text m_timer_text;

	[Header ("Loading Screen")]
	public GameObject m_loading_screen;


	[Header ("LoseLevel Screen")]
	public GameObject m_loselevel_screen;

	[Header ("PauseLevel Screen")]
	public GameObject m_pauselevel_screen;


	[Header ("Choose-Levels Screen")]
	public GameObject m_levels_screen;



	/* array of gameplay screens, each index is one level, this objects are never used directly nor mutated,
	 * but cloned (Object.Instantiate) in m_playing_screen each time a new level is started to ensure a clean state */
	[Header ("Gameplay Screens")]
	public GameObject[] m_gameplay_screens;

	private GameObject m_playing_screen;

	//if timer is implemented use this
	[Range (0f, 4f)]
	private float m_current_time = 0;

	public float CurrentTime { get { return m_current_time; } }

	/* array of booleans with the same length of levels and same indexes, if a level is playable because the gamers 
	 * won it the m_levels_accessible[i] is true otherwise is false and that level isn't accessible */

	[Header ("Levels accessible")]
	public bool[] m_levels_accessible;

	[Header ("Buttons in Choose-Levels Screen")]
	public GameObject[] m_levels_buttons;


	[Header ("Loading time for Level")]
	[Range (0f, 4f)]
	public float m_loading_time = 0.5f;


	[Header ("Gravity Input")]
	public Gravity gravityInput;

	[Header ("PlayerAvatar")]
	public PlayerAvatar_02 playerAvatar;



	void Start ()
	{
		//set all the levels except the first as not accessible
		for (int i = 0; i < m_gameplay_screens.Length; i++) {
			if (i == tutorial || cheat) {
				m_levels_accessible [i] = true;
				m_levels_buttons [i].SetActive (true);
			} else {
				m_levels_accessible [i] = false;
				m_levels_buttons [i].SetActive (false);
			}
			//deactivate all the level screens, they will never be used directly 
			m_gameplay_screens [i].SetActive (false);
		}

		ClearScreens ();

		// activate level chooser
		m_levels_screen.SetActive (true);

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Backspace)) {
			ReloadLevel ();
		}

		if (Input.GetKeyDown (KeyCode.P)) {
			PauseLevel ();
		}
	}



	//called when the user choses one level
	public void ChooseLevel (int n)
	{
		if (m_levels_accessible [n]) {
			current_level = n;
			StartCoroutine (LoadLevel ());
		}
		//else it does nothing and the button doesn't work
	
	}



	// loads the current level

	IEnumerator LoadLevel ()
	{

		yield return new WaitForSeconds (m_loading_time);

		//initialization
		this.ClearScreens ();
		gravityInput.ResetGravity ();


		//duplicate the required level and activate it
		m_playing_screen = Instantiate (m_gameplay_screens [current_level]);
		m_playing_screen.SetActive (true);

		//activate timer screen
		m_timer_screen.SetActive (true);

		// Reset drop
		playerAvatar.PlayerReset ();
		playerAvatar.ActivateParticles ();
	
	
	}


	//triggered by the button "next level"
	public void NextLevel ()
	{
		current_level++;
		StartCoroutine (LoadLevel ());

	}



	//triggered by the button "play again" in Lose/Win screens
	public void ReloadLevel ()
	{
		StartCoroutine (LoadLevel ());
	}





	//called when the player reaches the end of the level
	public void WinLevel ()
	{
		// set as accessible (true) the next level if the current one is won
		if (current_level + 1 < m_levels_accessible.Length) {
			m_levels_accessible [current_level + 1] = true;
			m_levels_buttons [current_level + 1].SetActive (true);
		}
		this.EndLevel ();
		m_endlevel_screen.SetActive (true);

	}


	// called to destroy the current level screen
	// never called directly by the UI
	void EndLevel ()
	{
		this.ClearScreens ();
		// destroy the currently allocated level screen when a level ends winning/losing
		Destroy (m_playing_screen);
		playerAvatar.DeactivateParticles ();
		
	}


	//called when the player loses in a level
	public void LoseLevel ()
	{
		this.EndLevel ();
		m_loselevel_screen.SetActive (true);
	}


	//called when the player pauses the game
	public void PauseLevel ()
	{
		m_playing_screen.SetActive (false);
		playerAvatar.DeactivateParticles ();
		m_pauselevel_screen.SetActive (true);
	}


	//triggered by the button "continue" in the pause screen
	public void ResumeLevel ()
	{
		m_pauselevel_screen.SetActive (false);
		m_playing_screen.SetActive (true);
		playerAvatar.ActivateParticles ();
	}


	public void ListLevels ()
	{
		this.EndLevel ();
		m_levels_screen.SetActive (true);
	}



	public void LoadingLevel ()
	{
		this.EndLevel ();
		m_loading_screen.SetActive (true);
		this.ReloadLevel ();
	}


	// deactivate all the screens
	void ClearScreens ()
	{
		if (m_endlevel_screen != null)
			m_endlevel_screen.SetActive (false);
		if (m_levels_screen != null)
			m_levels_screen.SetActive (false);
		if (m_playing_screen != null)
			m_playing_screen.SetActive (false);
		if (m_loselevel_screen != null)
			m_loselevel_screen.SetActive (false);
		if (m_pauselevel_screen != null)
			m_pauselevel_screen.SetActive (false);
		if (m_loading_screen != null)
			m_loading_screen.SetActive (false);
		if (m_timer_screen != null)
			m_timer_screen.SetActive (false);
		
	}



	//come back to main menu
	public void SwitchToMenu ()
	{
		SceneManager.LoadScene ("Menu");
	}
		 

}