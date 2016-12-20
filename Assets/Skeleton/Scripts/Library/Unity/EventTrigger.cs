using UnityEngine;
using System.Collections;

namespace POLIMIGameCollective {

	/// <summary>
	/// Example class that shows how to trigger specific events
	/// </summary>
	public class EventTrigger : MonoBehaviour {

		// Update is called once per frame
		void Update () {
			if (Input.GetKeyDown (KeyCode.S))
				EventManager.TriggerEvent ("Spawn");
		}
	}
}