using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace POLIMIGameCollective {
	
	public class EventManager : MonoBehaviour {

		[Header("If set the manager is kept across scenes")]
		public bool m_DontDestroyOnLoad = true;
		private Dictionary <string, UnityEvent> eventDictionary;

		private static EventManager eventManager;

		public static EventManager instance
		{
			get
			{
				if (!eventManager)
				{
					eventManager = FindObjectOfType (typeof (EventManager)) as EventManager;

					if (!eventManager) 
					{
						Debug.LogError ("There needs to be one active EventManger script on a GameObject in your scene.");
					}
					else
					{
						eventManager.Init (); 
					}
				}

				return eventManager;
			}
		}

		void Init ()
		{
			if (eventDictionary == null)
			{
				eventDictionary = new Dictionary<string, UnityEvent>();
				if (m_DontDestroyOnLoad) DontDestroyOnLoad (gameObject);
			}
		}

		public static void StartListening (string eventName, UnityAction listener)
		{
			UnityEvent thisEvent = null;
			if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.AddListener (listener);
			} 
			else
			{
				thisEvent = new UnityEvent ();
				thisEvent.AddListener (listener);
				instance.eventDictionary.Add (eventName, thisEvent);
			}
		}

		public static void StopListening (string eventName, UnityAction listener)
		{
			if (eventManager == null) return;
			UnityEvent thisEvent = null;
			if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.RemoveListener (listener);
			}
		}

		public static void TriggerEvent (string eventName)
		{
			UnityEvent thisEvent = null;
			if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.Invoke ();
			}
		}
	}
}