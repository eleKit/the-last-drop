using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace POLIMIGameCollective {

	/// <summary>
	/// Sound effect manager using only one audio source
	/// </summary>
	public class SfxManager : Singleton<SfxManager> {

		[Header("Sound Effects Clips")]
		public AudioClip[] m_sfx_clips;

		// audio source for sound effects
		private AudioSource m_audio_source;
		private Dictionary<string, AudioClip> sfx_list;

		void Awake() {
			m_audio_source = gameObject.GetComponent<AudioSource>();

			if (m_audio_source == null)
				Debug.LogError ("Audio Source Component not found");
			
			sfx_list = new Dictionary<string, AudioClip>();

			for (int i = 0; i < m_sfx_clips.Length; i++) {
				sfx_list [m_sfx_clips [i].name] = m_sfx_clips [i];
			}
		}

		public void Play(string name, float pitchVariance = 0)
		{
			if (sfx_list.ContainsKey(name))
			{
				if (pitchVariance != 0) m_audio_source.pitch = 1 + Random.Range(-pitchVariance, pitchVariance);

				m_audio_source.clip = sfx_list [name];
				m_audio_source.Play ();

			} else Debug.LogWarning("No sound of name " + name + " exists");
		}

		public void Mute()
		{
			m_audio_source.mute = true;
		}

		public void Unmute()
		{
			m_audio_source.mute = false;
		}

	}
}