using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAvatar_02 : MonoBehaviour
{
	Transform tr;
	//    CircleCollider2D m_Circle_Coll; // Collider used for triggering player event(consoles for example)

	public string m_Layer_Static;
	public string m_Layer_Player;
	public LayerMask m_Layer_Raycast;
	public GameObject m_Particle;

	[Header ("Starting Position object"), Tooltip ("if null it will start from transform position of player")]
	public GameObject m_Start_Position_Object;

	[Header ("Shape Stats"), Tooltip ("Number of particles around the center"), Range (8, 100)]
	public int m_No_Particles = 8;
	[Tooltip ("Ideal radius of the drop")]
	public float m_Radius = 1.0f;
	[Tooltip ("The avatar will surrender to death after his particle count drop under this")]
	public int m_min_particles = 5;

	[Tooltip ("Strenght of the bounds toward center")]
	public float m_Center_Bound_Freq;
	[Tooltip ("Strenght of the surface buond")]
	public float m_Surface_Buond;

	[Header ("Iteractions with other scripts")]
	[Tooltip ("Time must pass between a teleport and the other in secs")]
	public float m_Min_Time_ToTeleport = 0.2f;

	[Tooltip ("Air control streght, then is multiplied with the number of particles")]
	public float m_Air_Control = 1.0f;
	[Tooltip ("Check if a particle is in contact with the floor every this seconds"), Range (0.008f, 0.1f)]
	public float m_CheckForContact_Repeat_Time = 0.008f;


	// List to store values of the verts in the procedural mesh, based on the numbers of raycasts
	// Record [0] store the center of the mesh information.
	private List<RB_vert> m_vertex_list = new List<RB_vert> ();

	private float m_Last_Teleport;

	/// <summary>
	/// m_Num_In_Contact tell us how many particle are "sticked" to a surface, it should be used to move around the blob
	/// It's speed must be proportional to the number of particle in contact
	/// </summary>
	public int m_Num_In_Contact;
	//Used by checkforcontact, to get a rough(not real time!!!) estimate on how big is the blob
	private Vector3 m_Start_Position;

	private Vector2[] m_CosSin;
	float m_Radii_Segment;
	// radial segment size by number of raycasts


	public class RB_vert : MonoBehaviour
	{
		public static GameObject Center;
		public GameObject particle;
		public Dynam_Particle particle_script;
		public Transform tr;
		public Rigidbody2D rigidBody;
		public SpringJoint2D to_center;

		// TODO: put center particle in constructor and join next methods
		public RB_vert (GameObject part_ref, Vector3 initial_position, Quaternion initial_rotation)
		{
			part_ref.tag = "Player";
			particle = part_ref;

			part_ref.transform.position = initial_position;
			part_ref.transform.rotation = initial_rotation;

			tr = particle.GetComponent<Transform> ();
			rigidBody = particle.GetComponent<Rigidbody2D> ();

			particle_script = particle.GetComponent<Dynam_Particle> ();
			particle_script.m_IsSticky = true;

			to_center = null;
		}

		// Set the current particle as the central particle
		// TODO: make static
		public void set_center ()
		{
			Center = particle;
			particle_script.m_IsSticky = false;
		}

		// Creates a spring joint from the center to the current particle
		public void center_spring (float freq)
		{
			to_center = particle.AddComponent<SpringJoint2D> ();
			to_center.enableCollision = false;
			to_center.connectedBody = Center.GetComponent<Rigidbody2D> ();
			to_center.frequency = freq;
		}

		public static Vector3 get_center_position ()
		{
			return Center.transform.position;
		}
	}


	// Use this for initialization
	void Start ()
	{
		tr = gameObject.GetComponent<Transform> ();

		calc_cossin (); // Setup everything needed depending on the number of "Raycasts", like CosSin, number of
		// vertices for the mesh generation ecc


		PlayerReset ();

/*      POLIMIGameCollective.EventManager.StartListening("PlayerReset", PlayerReset);

        POLIMIGameCollective.EventManager.StartListening("EndLevel", PlayerDestroy);*/
		POLIMIGameCollective.EventManager.StartListening ("LoadLevel", PlayerReset);

/*
        POLIMIGameCollective.EventManager.StartListening("PauseLevel", PlayerDestroy);
        POLIMIGameCollective.EventManager.StartListening("ResumeLevel", PlayerReset);
*/
	}

	void Update ()
	{
		tr.position = RB_vert.get_center_position ();
	}

	// TODO: remove
	void OnEnable ()
	{
		InvokeRepeating ("Check_For_Contact", m_CheckForContact_Repeat_Time, m_CheckForContact_Repeat_Time);
		Debug.Log ("Enable");
	}

	// TODO: remove
	void OnDisable ()
	{
		CancelInvoke ("Check_For_Contact");
		Debug.Log ("Disable");
	}

	/************************************/
	/***    Invoke and coroutines     ***/
	/************************************/
	// TODO: remove
	void Check_For_Contact ()
	{
		m_Num_In_Contact = 0;
		for (int i = 1; i < m_vertex_list.Count; i++) {
			if (m_vertex_list [i].particle_script.m_Is_InContact_With_Floor) {
				m_Num_In_Contact += 1;
			}
		}
		//Debug.Log("Num in contacts:" + m_Num_In_Contact);
	}

	/************************************/
	/******** Internal methods **********/
	/************************************/

	void calc_cossin () // Pre-compute Sin/cos based on the number of raycasts and set
                       // radii_segment, number of vertices ecc.
	{
		m_CosSin = new Vector2[m_No_Particles];
		m_Radii_Segment = (Mathf.PI * 2) / m_No_Particles;

		for (int i = 0; i < m_No_Particles; i++) {
			m_CosSin [i] = new Vector2 (Mathf.Cos ((i + 1) * m_Radii_Segment), Mathf.Sin ((i + 1) * m_Radii_Segment)); //+1 because center is not calculated with cossin
		}

	}

	void make_vertex_list ()
	{
		m_vertex_list.Clear ();

		// Generate central particle
		m_vertex_list.Add (new RB_vert (POLIMIGameCollective.ObjectPoolingManager.Instance.GetObject (m_Particle.name),
			tr.position, Quaternion.identity));
		m_vertex_list [0].set_center ();

		Vector3 position = Vector3.zero;

		// Generate other particles
		for (int i = 0; i < m_No_Particles; i++) {
			position.Set (m_Radius * m_CosSin [i].x, m_Radius * m_CosSin [i].y, tr.position.z);
			RB_vert new_particle = new RB_vert (POLIMIGameCollective.ObjectPoolingManager.Instance.GetObject (m_Particle.name),
				                       tr.position + position,
				                       Quaternion.identity);
			m_vertex_list.Add (new_particle);
			new_particle.center_spring (m_Center_Bound_Freq);
		}
	}

	/****************************/
	/****  PUBLIC METHODS *******/
	/****************************/



	// Return number of particles
	public int No_Particles ()
	{
		return m_vertex_list.Count;
	}

	public GameObject Get_Central_Particle ()
	{
		return m_vertex_list [0].particle;
	}

	// TODO: remove
	public void AddSpeed (Vector2 Speed)
	{
		if (GameManager.Instance.m_Player_IsStretching == false) {
			m_vertex_list [0].rigidBody.AddForce (Speed * m_Num_In_Contact);
		} else {
			m_vertex_list [0].rigidBody.AddForce ((Speed * m_Num_In_Contact) + (Speed.normalized * m_Air_Control * m_vertex_list.Count));
		}
	}

	// TODO: remove
	public void Grow (int no_particles)
	{
		Debug.Log ("Add particle: " + no_particles);
		Vector3 position = Vector3.zero;
		for (int i = 0; i < no_particles; i++) {
			int rand_sincos_ind = Random.Range (0, m_No_Particles);

			position.Set (m_Radius * m_CosSin [rand_sincos_ind].x, m_Radius * m_CosSin [rand_sincos_ind].y, tr.position.z);
			m_vertex_list.Add (new RB_vert (POLIMIGameCollective.ObjectPoolingManager.Instance.GetObject (m_Particle.name), tr.position + position, Quaternion.identity));
			m_vertex_list [m_vertex_list.Count - 1].center_spring (m_Center_Bound_Freq);
		}
	}

	/***************************************/
	/********* TRIGGER EVENTS **************/
	/***************************************/
	public void PlayerReset ()
	{

		m_Start_Position_Object = GameObject.Find ("PlayerStart");
		if (m_Start_Position_Object != null) {
			m_Start_Position = m_Start_Position_Object.transform.position;
			tr.position = m_Start_Position;
		} else {
			m_Start_Position = tr.position;
		}

		make_vertex_list ();
		GameManager.Instance.m_Central_Particle = Get_Central_Particle ();
		//TODO: remove from GameManager GameManager.Instance.Gravity_Reset ();

	}

	public void DeactivateParticles ()
	{
		for (int i = 0; i < m_vertex_list.Count; i++) {
			m_vertex_list [i].particle.SetActive (false);
		}
	}
}
