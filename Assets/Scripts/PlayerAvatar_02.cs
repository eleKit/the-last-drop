using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAvatar_02 : MonoBehaviour, ITeleport
{
    Transform tr;
    //    CircleCollider2D m_Circle_Coll;

    public string m_Layer_Static;
    public string m_Layer_Player;
    public LayerMask m_Layer_Raycast;
    public GameObject m_Particle;

    [Header("Shape Stats"), Tooltip("Number of particles around the center"), Range(8, 100)]
    public int m_No_Particles = 8;
    [Tooltip("Ideal radius of the drop")]
    public float m_Radius = 1.0f;

    [Tooltip("Strenght of the bounds toward center")]
    public float m_Center_Bound_Freq;
    [Tooltip("Strenght of the surface buond")]
    public float m_Surface_Buond;

    [Header("Iteractions with other scripts")]
    [Tooltip("Time must pass between a teleport and the other in secs")]
    public float m_Min_Time_ToTeleport = 0.2f;

    [Tooltip("Check if a particle is in contact with the floor every this seconds"), Range(0.008f, 0.1f)]
    public float m_CheckForContact_Repeat_Time = 0.008f;


    // List to store values of the verts in the procedural mesh, based on the numbers of raycasts
    // Record [0] store the center of the mesh information.
    private List<RB_vert> m_Vlist = new List<RB_vert>();

    private Vector2 m_old_speed;
    private bool m_isgrounded;
    private float m_Last_Teleport;

    private Vector2[] m_CosSin;
    float m_Radii_Segment; // radial segment size by number of raycasts


    public struct RB_vert
    {
        public static GameObject Center;
        public GameObject particle;
        public Transform tr;
        public Rigidbody2D rb;
        public SpringJoint2D to_center;
//      public SpringJoint2D to_prev;
        
        public RB_vert( GameObject part_ref, Vector3 arg_position, Quaternion arg_rotation)
        {
            part_ref.tag = "Player";
            particle = part_ref;

            part_ref.transform.position = arg_position;
            part_ref.transform.rotation = arg_rotation;

            tr = particle.GetComponent<Transform>();
            rb = particle.GetComponent<Rigidbody2D>();

 //         to_prev = null;
            to_center = null;
        }

        public void set_center()
        {
            Center = particle;
        }

        public void center_spring( float freq )
        {
            to_center = particle.AddComponent<SpringJoint2D>();
            to_center.enableCollision = false;
            to_center.connectedBody = Center.GetComponent<Rigidbody2D>();
            to_center.frequency = freq;
        }
/*
        public void prev_spring(GameObject prev_ref)
        {
            to_prev = particle.AddComponent<SpringJoint2D>();
            to_prev.connectedBody = prev_ref.GetComponent<Rigidbody2D>();
        }
*/
        public Vector3 get_center_position()
        {
            return Center.transform.position;
        }

        public void set_bound_tocenter(float str)
        {
            //            to_center.frequency = str;
            //            if (to_center == null) Debug.Log("Cacchiarola");
            SpringJoint2D[] joints = particle.GetComponents<SpringJoint2D>();
            joints[0].frequency = str;
        }

        public void set_bound_surface(float str)
        {
            SpringJoint2D[] joints = particle.GetComponents<SpringJoint2D>();
            joints[1].frequency = str;
        }

        public void set_location( Vector3 arg_location)
        {
            particle.transform.position =  arg_location;
        }
    }

    void OnValidate()
    {
        if (m_Vlist.Count == 0) return;
        Debug.Log(m_Vlist.Count);
        Set_Buond_To_Center( m_Center_Bound_Freq );
 //       Set_Surface_Buond();
    }

    // Use this for initialization
    void Start()
    {
        tr = gameObject.GetComponent<Transform>();

        //        Physics2D.IgnoreLayerCollision( LayerMask.NameToLayer(m_Layer_Player), LayerMask.NameToLayer(m_Layer_Static));

        calc_cossin(); // Setup everything needed depending on the number of "Raycasts", like CosSin, number of
                       // vertices for the mesh generation ecc

        make_vertex_list(); // actually building the list of vertices used by mesh maker
        

        GameManager.Instance.m_Central_Particle = Get_Central_Particle(); // used to force the movement of the player


        POLIMIGameCollective.EventManager.StartListening("PlayerReset", PlayerReset);

        InvokeRepeating( "Check_For_Contact", m_CheckForContact_Repeat_Time, m_CheckForContact_Repeat_Time);
    }

    void Update()
    {
        tr.position = m_Vlist[0].get_center_position();
    }

    void OnEnable()
    {
        InvokeRepeating("Check_For_Contact", m_CheckForContact_Repeat_Time, m_CheckForContact_Repeat_Time);
        Debug.Log("One Time");
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    /************************************/
    /***    Invoke and coroutines     ***/
    /************************************/
    void Check_For_Contact()
    {
        foreach( RB_vert elem in m_Vlist )
        {
        }
    }

    /************************************/
    /******** Internal methods **********/
    /************************************/

    void calc_cossin() // Pre Computer Sincos based on the number of raycasts and set
                       // radii_segment, number of vertices ecc.
    {
        m_CosSin = new Vector2[m_No_Particles];
        m_Radii_Segment = (Mathf.PI * 2) / m_No_Particles;

        for (int i = 0; i < m_No_Particles; i++)
        {
            m_CosSin[i] = new Vector2(Mathf.Cos((i + 1) * m_Radii_Segment), Mathf.Sin((i + 1) * m_Radii_Segment)); //+1 because center is not calculated with cossin
        }

    }

    void make_vertex_list()
    {
        m_Vlist.Add(new RB_vert(POLIMIGameCollective.ObjectPoolingManager.Instance.GetObject(m_Particle.name), tr.position, Quaternion.identity));
        m_Vlist[0].set_center();
        
        Vector3 position = Vector3.zero;
        

        for (int i = 0; i < m_No_Particles; i++)
        {
            position.Set(m_Radius * m_CosSin[i].x, m_Radius * m_CosSin[i].y, tr.position.z);
            m_Vlist.Add(new RB_vert(POLIMIGameCollective.ObjectPoolingManager.Instance.GetObject(m_Particle.name), tr.position + position, Quaternion.identity));
            m_Vlist[i + 1].center_spring( m_Center_Bound_Freq );

            // Surface bound removed, it's not really usefull
         /*   if (i != 0)
            {
                if (i == m_No_Particles - 1)
                {
                    m_Vlist[1].prev_spring(m_Vlist[i + 1].particle);
                    m_Vlist[i + 1].prev_spring(m_Vlist[i].particle);
                } else
                {
                    m_Vlist[i + 1].prev_spring(m_Vlist[i].particle);
                }
            }*/
        }
    }

    /****************************/
    /****  PUBLIC METHODS *******/
    /****************************/

    public void Set_Buond_To_Center( float freq )
    {
        for(int i = 1; i < m_Vlist.Count; i++) //start from 1, skipping center
        {
            //Debug.Log(i);
            m_Vlist[i].set_bound_tocenter( freq );
        }
    }

    // Remove specific particle
    public void Deactivate_Particle( GameObject particle )
    {
        for( int i = 0; i < m_Vlist.Count; i++ )
        {
            if(m_Vlist[i].particle == particle)
            {
                if( i == 0 ) // 0 is the center, it must be preserved until everything else is destroyed
                {

                } else
                {
                    m_Vlist.RemoveAt(i);
                    particle.SetActive(false);
                    break;
                }
            }
        }
    }

    // Remove N particles at random
    public void Deactivate_Particle( int no_particles )
    {
        for (int i = 0; i < no_particles; i++)
        {
            int random_element = Random.Range(1, m_Vlist.Count - 1 );
            m_Vlist[random_element].particle.SetActive(false);
            m_Vlist.RemoveAt(i);
        }
    }

    // Return a random particle reference save from the center
    public GameObject Get_Rand_Particle()
    {
        return m_Vlist[ Random.Range(1, m_Vlist.Count - 1) ].particle;
    }

    // Return number of particles
    public int No_Particles()
    {
        return m_Vlist.Count;
    }

    public GameObject Get_Central_Particle()
    {
        return m_Vlist[0].particle;
    }

    public void AddSpeed( Vector2 Speed )
    {
    }

    /*
        public void Set_Surface_Buond()
        {
            for (int i = 1; i < m_No_Particles + 1; i++) //start from 1, skipping center
            {
                Debug.Log(i);
                m_Vlist[i].set_bound_surface( m_Surface_Buond );
            }
        }
    */

    /***************************************/
    /********* TRIGGER EVENTS **************/
    /***************************************/
    public void PlayerReset()
    {

        for (int i = 0; i < m_Vlist.Count; i++)
        {
            m_Vlist[i].particle.SetActive(false);
        }

        m_Vlist.Clear();

        calc_cossin(); 
        make_vertex_list(); 
        GameManager.Instance.m_Central_Particle = Get_Central_Particle();
 //       Set_Buond_To_Center(m_Center_Bound_Freq);
        //      Debug.Log("Reset!");
    }

    /***************************************/
    /*********    INTERFACES ***************/
    /***************************************/

    public void Teleport_To(Vector3 relative_position, Vector3 direction)
    {
        if( Time.time - m_Last_Teleport >= m_Min_Time_ToTeleport )
        {
            for (int i = 0; i < m_Vlist.Count; i++)
            {
                m_Vlist[i].set_location(relative_position);
                float speed = m_Vlist[i].rb.velocity.magnitude;
                m_Vlist[i].rb.velocity = direction * speed;
            }
            m_Last_Teleport = Time.time;
        }
        
    }
}
