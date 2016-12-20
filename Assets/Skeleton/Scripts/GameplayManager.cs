using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using POLIMIGameCollective;

public class GameplayManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		MusicManager.Instance.PlayMusic ("GameplayMusic");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha1))
			SfxManager.Instance.Play ("creature");
		else if (Input.GetKeyDown (KeyCode.Alpha2))
			SfxManager.Instance.Play ("jump");
		else if (Input.GetKeyDown (KeyCode.Alpha3))
			SfxManager.Instance.Play ("laser");
		else if (Input.GetKeyDown (KeyCode.Alpha4))
			SfxManager.Instance.Play ("lose");
		else if (Input.GetKeyDown (KeyCode.Alpha5))
			SfxManager.Instance.Play ("pickup");
		else if (Input.GetKeyDown (KeyCode.Alpha6))
			SfxManager.Instance.Play ("radar");
		else if (Input.GetKeyDown (KeyCode.Alpha7))
			SfxManager.Instance.Play ("rumble");
		else if (Input.GetKeyDown (KeyCode.Space)) {
			MusicManager.Instance.StopAll ();
			MusicManager.Instance.PlayMusic ("MenuMusic");
			SceneManager.LoadScene ("Menu");
		}
	}
}
