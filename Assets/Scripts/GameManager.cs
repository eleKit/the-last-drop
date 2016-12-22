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



	void Awake ()
	{
		// Loading Pools

		POLIMIGameCollective.ObjectPoolingManager.Instance.CreatePool (m_dynam_particle, m_dynam_particle_no_instaces, m_dynam_particle_no_instaces);
	}


}
