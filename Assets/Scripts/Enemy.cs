using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float m_speed = 2f;
	// Test clip
	public AudioClip testClip;

	private int verse = 1;
	private Transform tr;
	private Vector2 raycastDirection = Vector2.down; //new Vector2(1f,-1f); diagonal vector
	private SpriteRenderer sr;

	private Animator animator;
	private bool moving = false;
	private bool shooting = false;

    void Start () {
        tr = GetComponent<Transform>() as Transform;
		sr = GetComponent<SpriteRenderer>() as SpriteRenderer;
		animator = GetComponent<Animator>() as Animator;
		animator.SetBool("Moving", moving);
    }

    void FixedUpdate () {
		if(m_speed == 0f) {
			moving = false;
		}
		Move();

		// Shoot player if seen in straight line
		// TODO
		/*
		RaycastHit2D[] hits = Physics2D.RaycastAll(tr.position, verse * Vector2.right);
		if(hits != null) {
			foreach(RaycastHit2D hit in hits) {
				if(hit.collider.gameObject.tag == "Player") {
					Shoot(hit.collider.gameObject);
				}
			}
		}
		*/
    }

	void Move() {
		moving = true;
		RaycastHit2D[] hits = Physics2D.RaycastAll(tr.position, raycastDirection, 0.5f);

		// If there's no platform under this collider2D
		if(hits.Length <= 1) {
			verse *= -1;	// Move in other direction
			sr.flipX = verse > 0 ? false : true;	// Flip the sprite
			// For diagonal vectors
			// raycastDirection = new Vector2(verse * 1f, -1f); // Flip the raycast direction
		}
		// Move
		tr.position = tr.position + verse * m_speed * transform.right * Time.fixedDeltaTime;
	}

	// TODO
	void Shoot(GameObject player) {
		shooting = true;
	}

	// [TEMP] SetActive(false) if collides with player
	void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.tag == "Player")
		// Play test sound when this dies
			SoundManager.Instance.PlayModPitch(testClip);
            gameObject.SetActive(false);
    }
}
