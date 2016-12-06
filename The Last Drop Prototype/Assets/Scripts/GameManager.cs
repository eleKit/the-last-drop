﻿using UnityEngine;
using POLIMIGameCollective;
using System.Collections;

public class GameManager : Singleton<GameManager> {
 //   public static GameManager Instance = null;
    

    [Space(5), Header("Prefab for object pooling"), Tooltip("Liquid Particles object")]
    public GameObject m_dynam_particle;
    [Tooltip("Number of instances"), Range(0, 300)]
    public int m_dynam_particle_no_instaces = 30;

    [Space(5), Header("Gravity"), Tooltip("Type of gravity changes: defined vector directions(uncheck), or continuus(check)")]
    public bool m_Gravity_Type = false;

    // Gravity defined by vector is changed using Gravity_Change( bool clockwise) method in this script
    [Tooltip("The directions that gravity can assume")]
    public Vector2[] m_Gravity_Vectors = { new Vector2(0f, -9.8f),
                                           new Vector2(-9.8f, 0f),
                                           new Vector2(0f, 9.8f),
                                           new Vector2(9.8f, 0f), };
    private int m_current_grav_ind = 0;

    [Tooltip("How much time it gets to change gravity direction"), Range(0.1f,3f)]
    public float m_Gravity_change_CD = 0.3f;

    private float m_last_gravity_change = 0.0f;

    // Used player references(scripts, informations ecc)
    public GameObject m_Player;
    public PlayerAvatar_02 m_Player_Avatar_Cs;



    void Awake()
    {
/*        if( Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
*/
        // Loading Pools

        POLIMIGameCollective.ObjectPoolingManager.Instance.CreatePool(m_dynam_particle, m_dynam_particle_no_instaces, m_dynam_particle_no_instaces);
    }

    void Start()
    {
        m_Player = GameObject.Find("Player");
        if (m_Player == null)
        {
            Debug.Log("Found no Player in scene");
        }
        else
        {
            m_Player_Avatar_Cs = m_Player.GetComponent<PlayerAvatar_02>() as PlayerAvatar_02;
        }
    }

    public void Gravity_Change( bool clockwise )
    {
        if( (Time.time - m_last_gravity_change) > m_Gravity_change_CD)
        {
            // if clockwise add one to ind, or remove one if counter-clockwise
            // if ind is equal than gravity vector length, set it to 0
            m_current_grav_ind += (clockwise) ? -1 : 1;
            if (m_current_grav_ind == m_Gravity_Vectors.Length) m_current_grav_ind = 0;
            if (m_current_grav_ind < 0) m_current_grav_ind = m_Gravity_Vectors.Length - 1;
            Physics2D.gravity = m_Gravity_Vectors[m_current_grav_ind]; // set current gravity

            m_last_gravity_change = Time.time;
        }
    }
}
