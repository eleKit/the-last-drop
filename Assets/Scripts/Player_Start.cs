using UnityEngine;
using System.Collections;

public class Player_Start : MonoBehaviour {

    public GameObject m_Player_Prefab;

	// Use this for initialization
	void Start ()
    {
        GameManager.Instance.m_Player = (GameObject)Instantiate(m_Player_Prefab, gameObject.transform.position, Quaternion.identity);
    }
}
