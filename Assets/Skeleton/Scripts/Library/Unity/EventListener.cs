using UnityEngine;
using System.Collections;

namespace POLIMIGameCollective {
	
	/// <summary>
	/// Example class that shows how to listen to specific events and to act upon triggering
	/// </summary>

	public class EventListener : MonoBehaviour {
		public GameObject m_grunt_prefab;

		void OnEnable() {
			EventManager.StartListening ("Spawn", Spawn);
		}

		void OnDisable() {
			EventManager.StopListening ("Spawn", Spawn);
		}

		// Update is called once per frame
		void Spawn () {
			EventManager.StopListening ("Spawn", Spawn);

			Debug.Log ("Spawn event has been triggered");

			EventManager.StartListening ("Spawn", Spawn);

		}
	}
}