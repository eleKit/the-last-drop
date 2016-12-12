﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console : MonoBehaviour {

    public GameObject[] object_Linked;
    [Tooltip("Time the console will take to recover, in secs")]
    public float m_Time_To_Recover;

    private float m_last_use;
    private int m_Player_Particle_Inside;

    // Activate only once
    private bool used = false;

	// Use this for initialization
	void Start () {
		
	}
	
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")    
        {
            if( m_Player_Particle_Inside == 0 )
            {
                POLIMIGameCollective.EventManager.StartListening( "Shake" , activate);
            }

            m_Player_Particle_Inside++;
        }
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_Player_Particle_Inside--;

            if (m_Player_Particle_Inside == 0)
            {
                POLIMIGameCollective.EventManager.StopListening("Shake", activate);
            }

            //Debug.Log( m_Player_Particle_Inside );
        }
    }

    void activate()
    {
        Debug.Log("Shake registered in console!");
        
        if(used == false && Time.time - m_last_use > m_Time_To_Recover)
        {
            used = true;
            // Use The console!
            foreach(GameObject go in object_Linked)
            {
                // Cast to interface_console
                IConsoleIteration iconGO = (IConsoleIteration)go.GetComponent(typeof(IConsoleIteration));
                if (iconGO != null)
                {
                    iconGO.Activate_Once();
                }
            }

            m_last_use = Time.time;
        }
    }
}