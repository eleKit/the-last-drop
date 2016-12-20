using UnityEngine;
using System.Collections;
using POLIMIGameCollective;

public class SoundManager : Singleton<SoundManager> {
		
        [Header("Effect sound")]
        public AudioSource efxSource;
		[Header("Music sound")]
        public AudioSource musicSource;
        public float lowPitchRange = 0.95f;
        public float highPitchRange = 1.05f;
              
        
        //Used to play single sound clips.
        public void PlaySingle(AudioClip clip)
        {
            efxSource.clip = clip;          
            efxSource.Play ();
        }

        public void PlayModPitch (AudioClip clip)
        {
            float randomPitch = Random.Range(lowPitchRange, highPitchRange);
            efxSource.pitch = randomPitch;
            efxSource.clip = clip;
            efxSource.Play();
        }
}
