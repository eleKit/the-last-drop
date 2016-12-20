using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour {

	public float m_speed = 200f;
	Transform tr;

	void Start() {
		tr = GetComponent<Transform> () as Transform;
	}
	
	void FixedUpdate() {
		tr.position = tr.position + tr.right * m_speed * Time.fixedDeltaTime;
		if (Mathf.Abs (tr.position.x) > 220f)
			gameObject.SetActive(false);
		if (Mathf.Abs (tr.position.y) > 120f)
			gameObject.SetActive(false);
	}

	// TODO
	void OnTriggerEnter2D(Collider2D other) {
		if(other.gameObject.tag == "Player") {
			GameManager.Instance.m_Player.GetComponent<PlayerAvatar_02>().Deactivate_Particle(other.gameObject);
		}
		gameObject.SetActive(false);
	}
}
