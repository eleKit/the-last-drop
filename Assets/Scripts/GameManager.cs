using UnityEngine;
using UnityEngine.UI;
using POLIMIGameCollective;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
	[Space (5), Header ("Prefab for object pooling"), Tooltip ("Liquid Particles object")]
	public GameObject m_dynam_particle;
	[Tooltip ("Number of instances"), Range (0, 300)]
	public int m_dynam_particle_no_instaces = 30;

	[Header ("Public variable used externally, Do not set them")]
	// Used player references(scripts, informations ecc)
    public GameObject m_Player;
	public PlayerAvatar_02 m_Player_Avatar_Cs;
	public GameObject m_Central_Particle;
	public bool m_Player_IsStretching;

	void Awake ()
	{
		// Loading Pools

		POLIMIGameCollective.ObjectPoolingManager.Instance.CreatePool (m_dynam_particle, m_dynam_particle_no_instaces, m_dynam_particle_no_instaces);
	}

	void Start ()
	{
		m_Player = GameObject.Find ("Player");
		if (m_Player == null) {
			Debug.Log ("Found no Player in scene");
		} else {
			m_Player_Avatar_Cs = m_Player.GetComponent<PlayerAvatar_02> () as PlayerAvatar_02;
		}
	}
}
