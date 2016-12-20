using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAvatar : MonoBehaviour {
    Transform tr;
    Rigidbody2D rb;
    PolygonCollider2D m_Poly_Coll;
//    CircleCollider2D m_Circle_Coll;

    public string m_Layer_Static;
    public string m_Layer_Player;
    public LayerMask m_Layer_Raycast;

    [Header("Shape Stats"), Tooltip("Number of raycast shoot from the center"), Range(8,100)]
    public int m_Raycasts = 8;
    [Tooltip("Distance of the Raycast at rest, radius of the drop")]
    public float m_Radius = 1.0f;
    [Tooltip("maximum % of change in the gravity axis"), Range(-5f, 5f)]
    public float m_Max_Change = 0.2f;
    [Tooltip("Gravity multiplier, it will multiply the effect of gravity changing the offset of the circle collider"), Range(-5f, 5f)]
    public float m_Gravity_Mult = 0.2f;
    [Tooltip("Acceleration multiplier, it will multiply the effect of acceleration"), Range(-1f, 1f)]
    public float m_Accel_Mult = 0.2f;
    public AnimationCurve m_GravDistance_Curve;
    
    // List to store values of the verts in the procedural mesh, based on the numbers of raycasts
    // Record [0] store the center of the mesh information.
    private List<RB_vert> m_Vlist = new List<RB_vert>();
    
    private Vector2 m_old_speed;
    private bool m_isgrounded;

    private Vector2[] m_CosSin;
    float m_Radii_Segment; // radial segment size by number of raycasts

    public struct RB_vert
    {
        public Vector3 position;
        public Vector3 last_position;
        public Vector3 base_position;
        public float distance_to_collide;

        public RB_vert( Vector2 local_loc )
        {
            last_position = local_loc;
            position = local_loc;
            base_position = local_loc;

        distance_to_collide = 0f;
        }
    }

	// Use this for initialization
	void Start () {
        tr = gameObject.GetComponent<Transform>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        m_Poly_Coll = gameObject.GetComponent<PolygonCollider2D>();
//        m_Circle_Coll = gameObject.GetComponent<CircleCollider2D>();
//        m_Circle_Coll.radius = m_Radius * 0.8f;

//        Physics2D.IgnoreLayerCollision( LayerMask.NameToLayer(m_Layer_Player), LayerMask.NameToLayer(m_Layer_Static));

        calc_cossin(); // Setup everything needed depending on the number of "Raycasts", like CosSin, number of
                       // vertices for the mesh generation ecc

        make_vertex_list(); // actually building the list of vertices used by mesh maker

        Mesh_Maker(); // function to create the mesh. It's also used during update to re-make it(only way to update)

        coll_update(); // update the collider matching the 

    }
	
	// Update is called once per frame
	void FixedUpdate () {
 //       raycast_adjustment();
 //       Mesh_Maker(); // function to create the mesh. It's also used during update to re-make it(only way to update)

//        coll_update(); // update the collider matching the 
    }

    /************************************/
    /******** Internal methods **********/
    /************************************/
    void calc_cossin() // Pre Computer Sincos based on the number of raycasts and set
                       // radii_segment, number of vertices ecc.
    {
        m_CosSin = new Vector2[m_Raycasts];
        m_Radii_Segment = (Mathf.PI * 2) / m_Raycasts;

        for ( int i = 0; i < m_Raycasts; i++)
        {
            m_CosSin[i] = new Vector2(Mathf.Cos( (i+1) * m_Radii_Segment), Mathf.Sin( (i+1) * m_Radii_Segment)); //+1 because center is not calculated with cossin
        }

    }
    
    void make_vertex_list()  // First time run to make the vertex list used by procedural mesh and polygon collider path
    { 
        m_Vlist.Clear();
        m_Vlist.Add(new RB_vert( new Vector2(0f,0f)));
        Vector2 position = new Vector2();

//        int num_Vertices = 0;

        for (int i = 0; i < m_Raycasts; i++)
        {
            position.Set(m_Radius * m_CosSin[i].x , m_Radius * m_CosSin[i].y ); // position in circle
                                                                                                                 // determine how long is the raycast based on:
                                                                                                                 // acceleration
                                                                                                                 // gravity  Physics2D.gravity   * m_gravity_mult
                                                                                                                 // collider viscosity(physics property drag)   <<<---- Later on!
            
            m_Vlist.Add( new RB_vert( position ));
        }

        // Setup the Polygon Collider
    }

    Mesh Mesh_Maker()
    {
        Mesh new_mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = new_mesh;

        int mesh_verts = m_Raycasts + 1;
        Vector3[] cozy_verts = new Vector3[mesh_verts];
        Vector2[] cozy_uv = new Vector2[mesh_verts];

        int mesh_triangle = mesh_verts * 3;
        int[] cozy_triang = new int[mesh_triangle];

        //        Array.Resize<Vector3>(ref cozy_verts, mesh_verts);
        //        Array.Resize<int>(ref cozy_triang, mesh_triangle);

        // building vertex [] and uv[]
        for (int i = 0; i < mesh_verts; i++)
        {
            cozy_verts[i] = m_Vlist[i].position;
            cozy_uv[i]    = new Vector2(m_Vlist[i].position.x, m_Vlist[i].position.y);

            //          cozy_verts[i] = (i == 0 ? new Vector3(0, 0, 0) : m_Vlist[i - 1].position);
            //          cozy_uv[i] = (i == 0 ? new Vector2(0, 0) : new Vector2(m_Vlist[i - 1].position.x, m_Vlist[i - 1].position.y));
        }

        //build triangle []
        for (int i = 0; i <= m_Raycasts; i++)
        {
            cozy_triang[i * 3] = 0; // First vertex of every face is the center(0,0,0)
            cozy_triang[i * 3 + 1] = i; // Second vertes is the actual index
            cozy_triang[i * 3 + 2] = (i == m_Raycasts) ? 1 : i + 1; // Third one is the next index, or the first one if it's the last index
        }
        new_mesh.vertices = cozy_verts;
        new_mesh.triangles = cozy_triang;
        new_mesh.uv = cozy_uv;

        return new_mesh;
    }

    void coll_update()
    {
        Vector2[] verts = new Vector2[m_Raycasts];
        for (int i = 0; i < m_Raycasts; i++)
        {
            verts[i] = m_Vlist[i+1].position; // skip record 0, since it's useless for perimeter path
        }
        m_Poly_Coll.SetPath(0, verts);
    }

    void raycast_adjustment()
    {
        // changing circle collider offset, based on gravity(used to limit movement thru static layers)
        //gravity factor will move the offset of the circle collider
        // offset goal = gravity * mult
        //        m_Circle_Coll.offset = Physics2D.gravity.normalized * m_Gravity_Mult;

        //
        //        Vector2 grav_effect = new Vector2(Vector2.Angle(Physics2D.gravity, new Vector2(1, 0)) / 180f + m_Gravity_Mult, Vector2.Angle(Physics2D.gravity, new Vector2(0, 1)) / 180f + m_Gravity_Mult);


        //        Debug.Log(grav_effect);
        // loop m_Vlist skipping the first record(collider center)
/*
        float m_Grav_Adjustment = 0.0f;
        RaycastHit2D hitinfo = Physics2D.Linecast( tr.position, (Physics2D.gravity.normalized * m_Radius * 1.1f) + new Vector2(tr.position.x, tr.position.y), m_Layer_Raycast);
        if (hitinfo)
        {
                Debug.Log((hitinfo.point - new Vector2(tr.position.x, tr.position.y)).magnitude);
        }

        for (int i = 1; i < m_Raycasts+1; i++)
        {
            // Linecast to check distance from colliders, if none is found return infinity
            RB_vert cozy_ele = m_Vlist[i];
            hitinfo = Physics2D.Linecast( m_Vlist[i].position + tr.position, (m_Vlist[i].position * 2) + tr.position, m_Layer_Raycast );

            cozy_ele.distance_to_collide = (hitinfo == true) ? hitinfo.distance : Mathf.Infinity;
*/
//            cozy_ele.position = new Vector3( cozy_ele.base_position.x * grav_effect.x, cozy_ele.base_position.y * grav_effect.y);
            /*
                        float grav_angle = Vector2.Angle(Physics2D.gravity, cozy_ele.base_position);
                        // Angle between gravity direction and local base position
                        grav_angle = m_GravDistance_Curve.Evaluate(grav_angle / 180.0f);
                        cozy_ele.position = cozy_ele.base_position * (grav_angle + 1);
            */
            // more grav.norm is similar to base_pose.norm the more it's influence on the length.

            // this difference has a magnitude between 0 and 2, 0 being the most similar to gravity, and 2 most opposite, using curve we get a multiplier between 0 and 1
            //            Vector2 factor = ( Physics2D.gravity.normalized)+ new Vector2(cozy_ele.base_position.normalized.x, cozy_ele.base_position.normalized.y);
            //            factor.Set(factor.x / 2f, factor.y / 2f);
            //            Vector2 factor = new Vector2(Mathf.Clamp((Physics2D.gravity.normalized.x / cozy_ele.base_position.normalized.x), -1f, 1f),
            //                                         Mathf.Clamp((Physics2D.gravity.normalized.y / cozy_ele.base_position.normalized.y), -1f, 1f));
            //            cozy_ele.position = cozy_ele.base_position * (m_GravDistance_Curve.Evaluate(factor.magnitude) + 1f);
            //            Debug.Log("Position x: " + cozy_ele.position.x + " y: " + cozy_ele.position.y);
            // Calculate a factor used later on, to determine how much a vertex can be stretched from center
            // Depending on gravity vector, accel, air friction

            //acceleration will deform

/*            m_Vlist[i] = cozy_ele;
        } */
    }
}
