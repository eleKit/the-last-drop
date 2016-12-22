using UnityEngine;
using System.Collections;

public class Dynam_Particle : MonoBehaviour
{

	public Rigidbody2D rb;

	public enum STATES
	{
		WATER,
		GAS,
		LAVA,
		NONE}

	;
	//The 3 states of the particle
	STATES currentState = STATES.NONE;
	//Defines the currentstate of the particle, default is water
	public GameObject currentImage;
	//The image is for the metaball shader for the effect, it is onle seen by the liquids camera.
	public GameObject[] particleImages;
	//We need multiple particle images to reduce drawcalls
	float particleLifeTime = 3.0f, startTime;
	//How much time before the particle scalesdown and dies
	public bool scales_down = false;

	[Tooltip ("How much force the particle use vs the elements it collide with")]
	public float m_Sticknes = 0.5f;
	[Tooltip ("Which layers the drop collide with")]
	public LayerMask m_Stick_To_Layers;

	public bool m_IsSticky = false;
	public bool m_Is_InContact_With_Floor = false;

	void start ()
	{
		rb = gameObject.GetComponent<Rigidbody2D> ();
	}

	void Awake ()
	{
		rb = gameObject.GetComponent<Rigidbody2D> ();
		if (currentState == STATES.NONE)
			SetState (STATES.WATER);
	}

	void OnEnable ()
	{
		//      startTime = Time.time;
		m_Is_InContact_With_Floor = false;
		//    StartCoroutine(is_sticky());
	}

	void OnDisable ()  // Reset state of the particle so that can be placed back to the pool
	{
        
		SpringJoint2D[] joints = gameObject.GetComponents<SpringJoint2D> ();
		foreach (SpringJoint2D elem in joints) {
			Destroy (elem);
		}
		m_IsSticky = false;
		m_Is_InContact_With_Floor = false;

	}

	//The definitios to each state
	public void SetState (STATES newState)
	{
		if (newState != currentState) { //Only change to a different state
			switch (newState) {
			case STATES.WATER:
				rb.gravityScale = 1.0f; // To simulate Water density
				break;
			case STATES.GAS:
				particleLifeTime = particleLifeTime / 2.0f; // Gas lives the time the other particles
				rb.gravityScale = 0.0f;// To simulate Gas density
				gameObject.layer = LayerMask.NameToLayer ("Gas");// To have a different collision layer than the other particles (so gas doesnt rises up the lava but still collides with the wolrd)
				break;
			case STATES.LAVA:
				rb.gravityScale = 0.3f; // To simulate the lava density
				break;
			case STATES.NONE:
				gameObject.SetActive (false);
				break;
			}
			if (newState != STATES.NONE) {
				currentState = newState;
				startTime = Time.time;//Reset the life of the particle on a state change
				rb.velocity = new Vector2 ();    // Reset the particle velocity	
				currentImage.SetActive (false);
				currentImage = particleImages [(int)currentState];
				currentImage.SetActive (true);
			}
		}
	}

	// This scales the particle image acording to its velocity, so it looks like its deformable... but its not ;)
	void MovementAnimation ()
	{
		Vector3 movementScale = new Vector3 (1.0f, 1.0f, 1.0f);//Tamaño de textura no de metaball			
		movementScale.x += Mathf.Abs (rb.velocity.x) / 30.0f;
		movementScale.z += Mathf.Abs (rb.velocity.y) / 30.0f;
		movementScale.y = 1.0f;
		currentImage.gameObject.transform.localScale = movementScale;
	}

	// The effect for the particle to seem to fade away
	void ScaleDown ()
	{
		float scaleValue = 1.0f - ((Time.time - startTime) / particleLifeTime);
		Vector2 particleScale = Vector2.one;
		if (scaleValue <= 0) {
			gameObject.SetActive (false);
		} else {
			particleScale.x = scaleValue;
			particleScale.y = scaleValue;
			transform.localScale = particleScale;
		}
	}

	// To change particles lifetime externally (like the particle generator)
	public void SetLifeTime (float time)
	{
		particleLifeTime = time;
	}

	// If the particle is Sticky and hit something in the sticky layer mask, it will adhere with some force
	void OnCollisionEnter2D (Collision2D other)
	{

		// Stickness, anything not related to it before this line!
		if (!m_IsSticky)
			return; // return if not sticky!

		foreach (ContactPoint2D elem in other.contacts) {
			if (m_Stick_To_Layers == (m_Stick_To_Layers | (1 << elem.collider.gameObject.layer))) {
				rb.velocity = rb.velocity + (-elem.normal * m_Sticknes);
				m_Is_InContact_With_Floor = true;
			}
		}
	}

	void OnCollisionStay2D (Collision2D other)
	{
		// Stickness, anything not related to it before this line!
		if (!m_IsSticky)
			return; // return if not sticky!

		foreach (ContactPoint2D elem in other.contacts) {
			if (m_Stick_To_Layers == (m_Stick_To_Layers | (1 << elem.collider.gameObject.layer))) {
				//  Debug.Log("Normal: " + elem.normal);

				rb.velocity = rb.velocity + (-elem.normal * m_Sticknes);
			}
		}

	}

	void OnCollisionExit2D (Collision2D collision)
	{
		foreach (ContactPoint2D elem in collision.contacts) {
			if (m_Stick_To_Layers == (m_Stick_To_Layers | (1 << elem.collider.gameObject.layer)))
				m_Is_InContact_With_Floor = false;
		}
	}

	IEnumerator is_sticky ()
	{
		yield return new WaitForSeconds (.05f);
		if (gameObject.tag == "Player") {
			m_IsSticky = true;
		}
	}


	public Rigidbody2D get_rb ()
	{
		return rb;
	}

}