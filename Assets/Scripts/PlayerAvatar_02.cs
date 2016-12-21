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
	private int m_TotalParticles;
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
		public Rigidbody2D rb;
		public SpringJoint2D to_center;
		//      public SpringJoint2D to_prev;
        
		public RB_vert (GameObject part_ref, Vector3 arg_position, Quaternion arg_rotation)
		{
			part_ref.tag = "Player";
			particle = part_ref;

			part_ref.transform.position = arg_position;
			part_ref.transform.rotation = arg_rotation;

			tr = particle.GetComponent<Transform> ();
			rb = particle.GetComponent<Rigidbody2D> ();

			particle_script = particle.GetComponent<Dynam_Particle> ();
			particle_script.m_IsSticky = true;

			//         to_prev = null;
			to_center = null;
		}

		public void set_center ()
		{
			Center = particle;
			particle_script.m_IsSticky = false;
		}

		public void center_spring (float freq)
		{
			to_center = particle.AddComponent<SpringJoint2D> ();
			to_center.enableCollision = false;
			to_center.connectedBody = Center.GetComponent<Rigidbody2D> ();
			to_center.frequency = freq;
		}
		/*
        public void prev_spring(GameObject prev_ref)
        {
            to_prev = particle.AddComponent<SpringJoint2D>();
            to_prev.connectedBody = prev_ref.GetComponent<Rigidbody2D>();
        }
*/
		public Vector3 get_center_position ()
		{
			return Center.transform.position;
		}

		public void set_bound_tocenter (float str)
		{
			//            to_center.frequency = str;
			//            if (to_center == null) Debug.Log("Cacchiarola");
			SpringJoint2D[] joints = particle.GetComponents<SpringJoint2D> ();
			joints [0].frequency = str;
		}

		public void set_bound_surface (float str)
		{
			SpringJoint2D[] joints = particle.GetComponents<SpringJoint2D> ();
			joints [1].frequency = str;
		}

		public void set_location (Vector3 arg_location)
		{
			particle.transform.position = arg_location;
		}
	}
		

	// Use this for initialization
	void Start ()
	{
		tr = gameObject.GetComponent<Transform> ();

		m_Start_Position_Object = GameObject.Find ("PlayerStart");
		if (m_Start_Position_Object != null) {
			m_Start_Position = m_Start_Position_Object.transform.position;
			tr.position = m_Start_Position;
		} else {
			m_Start_Position = tr.position;
		}

		//        m_Circle_Coll = gameObject.GetComponent<CircleCollider2D>();
		//        Physics2D.IgnoreLayerCollision( LayerMask.NameToLayer("Player_Avatar"), LayerMask.NameToLayer("Metaballs"));

		calc_cossin (); // Setup everything needed depending on the number of "Raycasts", like CosSin, number of
		// vertices for the mesh generation ecc

		make_vertex_list (); // actually building the list of vertices used by mesh maker



		GameManager.Instance.m_Central_Particle = Get_Central_Particle (); // used to force the movement of the player



/*      POLIMIGameCollective.EventManager.StartListening("PlayerReset", PlayerReset);

        POLIMIGameCollective.EventManager.StartListening("EndLevel", PlayerDestroy);*/
		POLIMIGameCollective.EventManager.StartListening ("LoadLevel", PlayerReset);

/*
        POLIMIGameCollective.EventManager.StartListening("PauseLevel", PlayerDestroy);
        POLIMIGameCollective.EventManager.StartListening("ResumeLevel", PlayerReset);
*/

		/*
                InvokeRepeating( "Check_For_Contact", m_CheckForContact_Repeat_Time, m_CheckForContact_Repeat_Time);
                Debug.Log("Enable");
        */
	}

	void Update ()
	{
		tr.position = m_vertex_list [0].get_center_position ();
	}

	void OnEnable ()
	{
		InvokeRepeating ("Check_For_Contact", m_CheckForContact_Repeat_Time, m_CheckForContact_Repeat_Time);
		Debug.Log ("Enable");
	}

	void OnDisable ()
	{
		CancelInvoke ("Check_For_Contact");
		Debug.Log ("Disable");
	}

	/************************************/
	/***    Invoke and coroutines     ***/
	/************************************/
	void Check_For_Contact ()
	{
		m_Num_In_Contact = 0;
		m_TotalParticles = m_vertex_list.Count;
		for (int i = 1; i < m_TotalParticles; i++) {
			if (m_vertex_list [i].particle_script.m_Is_InContact_With_Floor) {
				m_Num_In_Contact += 1;
			}
		}
		//Debug.Log("Num in contacts:" + m_Num_In_Contact);
	}

	/************************************/
	/******** Internal methods **********/
	/************************************/

	void calc_cossin () // Pre Computer Sincos based on the number of raycasts and set
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
			m_vertex_list [0].rb.AddForce (Speed * m_Num_In_Contact);
		} else {
			m_vertex_list [0].rb.AddForce ((Speed * m_Num_In_Contact) + (Speed.normalized * m_Air_Control * m_vertex_list.Count));
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
		for (int i = 0; i < m_vertex_list.Count; i++) {
			m_vertex_list [i].particle.SetActive (false);
		}

		m_vertex_list.Clear ();

		m_Start_Position_Object = GameObject.Find ("PlayerStart");
		if (m_Start_Position_Object != null) {
			m_Start_Position = m_Start_Position_Object.transform.position;
		}
		tr.position = m_Start_Position;

		calc_cossin (); 
		make_vertex_list (); 
		GameManager.Instance.m_Central_Particle = Get_Central_Particle ();
		CameraManager.Instance.Reset_To_Start ();
		GameManager.Instance.Gravity_Reset ();

		//       Set_Buond_To_Center(m_Center_Bound_Freq);
		//      Debug.Log("Reset!");
	}

	public void PlayerDestroy ()
	{

		for (int i = 0; i < m_vertex_list.Count; i++) {
			m_vertex_list [i].particle.SetActive (false);
		}
		m_vertex_list.Clear ();
	}
}
