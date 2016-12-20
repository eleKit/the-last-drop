using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserParent : MonoBehaviour {

	[Header("Movement speed"), Range(0f,5f)]
	public float m_speed;
	[Header("Movement inverts after this time")]
	public float m_sec_offset = 0;
	private float timeToTurn;
	[Header("Movement verse 1, 0 or -1")]
	public int m_mov_verse = 1 ;	

	private Transform tr;
	private Transform child1tr;
	private Transform child2tr;
	
	void Start () {
		tr = gameObject.GetComponent<Transform>() as Transform;
		child1tr = tr.GetChild(0);
		child2tr = tr.GetChild(1);
		timeToTurn = Time.time + m_sec_offset;
	}

	void Update () {
		
	}

	void FixedUpdate() {
		LaserMovement();		
	}

	void LaserMovement() {
		// Invert direction of movement if offset is reached
		if(Time.time > timeToTurn) {
			m_mov_verse *= -1;
			timeToTurn = Time.time + m_sec_offset;
		}		
		child1tr.position = child1tr.position + m_mov_verse * m_speed * transform.right * Time.fixedDeltaTime;
		child2tr.position = child2tr.position + m_mov_verse * m_speed * transform.right * Time.fixedDeltaTime;
	}

	// TODO
	public void DestroyLaser(Transform childLaserSource) {
		// TODO exploding animation
		if(childLaserSource == child1tr || childLaserSource == child2tr) {
			gameObject.SetActive(false);

		}
	}

}
