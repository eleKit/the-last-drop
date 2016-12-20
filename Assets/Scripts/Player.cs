using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    Transform tr;
    
//    public GameObject m_Particle;

    [Header("Character Movement Stats -- Not in Use")]
    [Tooltip("Character increments of speed H axis")]
    public float m_Speed_H;
    [Tooltip("Character increments of speed V axis")]
    public float m_Speed_V;
    [Tooltip("Character max total speed, counting gravity too")]
    public float m_Max_Speed;

    [Space(5), Header("Character Ability, Stretching")]
    [Tooltip("Cooldown for ability on Fire1 input")]
    public float m_Ability1_CD;
    [Tooltip("Length multiplier")]
    public float m_Ability1_Length = 0.05f;
    [Tooltip("Min Length, after which the elastic will break")]
    public float m_Ability1_Min_Length = 0.1f;
    [Tooltip("Layers the stretching will stick to")]
    public LayerMask m_Ability1_Layers;
    [Tooltip("%(0.0 - 1.0) of particles used for stretching"), Range(0f, 1f)]
    public float m_Ability1_Perc_Particle_Used;
    [Tooltip("Strenght applied by the elastic")]
    public float m_Ability1_Tensile_Str = 1.0f;

    private float m_V_Axis1;
    private float m_H_Axis1;
    private float m_V_Axis2;
    private float m_H_Axis2;

    // Player Movement
    private bool m_Is_Moving = false;
    private Vector2 m_player_applied_speed;

    // Used by ability1(stretch)
    private float m_last_time_ability1;
    private LineRenderer m_Line_Renderer;
    private GameObject m_Central_Particle;
    private Rigidbody2D m_Central_Particle_rb;
    private Vector3[] m_Streching_Points;
    private Vector3 m_Screen_Size;

    // Use this for initialization
    void Start ()
    {
        tr = gameObject.GetComponent<Transform>();
    
        m_Line_Renderer = gameObject.GetComponent<LineRenderer>();
        if (m_Line_Renderer == null) Debug.Log("Found no line renderer on player!");
        m_Line_Renderer.enabled = false;

        StartCoroutine(set_central_particle());

        m_Screen_Size = new Vector3( Screen.width, Screen.height, 0f) / 2;

        // Triggers Events registration
        POLIMIGameCollective.EventManager.StartListening("Swipe", swipe);
        POLIMIGameCollective.EventManager.StartListening("MoveStart", MoveStart);
        POLIMIGameCollective.EventManager.StartListening("MoveEnd", MoveEnd);
    }

    // Update is called once per frame
    void Update() {
        m_V_Axis1 = Input.GetAxis("Vertical");
        m_H_Axis1 = Input.GetAxis("Horizontal");
        m_V_Axis2 = Input.GetAxis("Vertical2");
        m_H_Axis2 = Input.GetAxis("Horizontal2");
        


        

/********************************************/
/*    Ability(jump, shoot, stretch ecc)     */
/********************************************/

        if ( (      Input.GetButton("Fire1")                   ) &&
             (Time.time - m_last_time_ability1) > m_Ability1_CD) 
        {
                m_last_time_ability1 = Time.time;
                Stretch(new Vector2(1.0f, 1.0f).normalized);
            // Logic of ability one
        }

        if (Input.GetButton("Fire2"))
        {
            POLIMIGameCollective.EventManager.TriggerEvent("PlayerReset");
        }

        //  Debug to test input
        //        Debug.Log("H axis1: " + m_H_Axis1.ToString() + "H axis2: " +  m_H_Axis2.ToString() + "V axis1: " + m_V_Axis1.ToString() + "V axis2: " + m_V_Axis2.ToString());
    }

    void FixedUpdate()
    {
        if ( !GameManager.Instance.m_Gravity_Type )
        {   // gravity changed to the next, or previous index based on H Axis 1, check game manager
            if (m_H_Axis1 != 0.0f)
            {
                GameManager.Instance.Gravity_Change( ( m_H_Axis1 > 0 ) ? true : false );
            }
        }
        else   // continous gravity adjustments
        {
 /*           m_player_applied_speed.Set(0.0f, 0.0f);
            if (m_H_Axis1 != 0.0f) m_player_applied_speed += new Vector2(Physics2D.gravity.y, -Physics2D.gravity.x).normalized * m_Speed_H * m_H_Axis1 * Time.fixedDeltaTime * -1;//force applied perpendiculary to gravity
            if (m_V_Axis1 != 0.0f) m_player_applied_speed += new Vector2(-Physics2D.gravity.x, -Physics2D.gravity.y).normalized * m_Speed_V * m_V_Axis1 * Time.fixedDeltaTime;
            m_player_applied_speed += rb.velocity;
            rb.velocity = (rb.velocity.magnitude > m_Max_Speed) ? (m_player_applied_speed.normalized * m_Max_Speed) : m_player_applied_speed;
            if ((m_H_Axis2 != 0.0f) && (!Input.GetButton("Fire1")))
            {
                Physics2D.gravity = Quaternion.Euler(0f, 0f, m_H_Axis2 * Time.fixedDeltaTime * 100.0f) * Physics2D.gravity;
        }
 */     }

        if(m_Is_Moving)  // Is moving!
        {
            Vector2 direction = GameManager.Instance.Rotate_By_Gravity( TouchControlManager.Instance.moveDirection ).normalized; // change rotation by gravity
            
        }

        // Strecthing logic, if there is a stretch in action(which is true if the Line Render is enabled)
        if(m_Line_Renderer.enabled)
        {
            m_Central_Particle_rb.velocity = m_Central_Particle_rb.velocity + ( ( new Vector2(m_Streching_Points[0].x, m_Streching_Points[0].y) - new Vector2( m_Streching_Points[1].x, m_Streching_Points[1].y) ) * m_Ability1_Tensile_Str);
            Set_Points();
            // Current maximum lenght is: 1.0f * parts_used* m_Ability1_Length
            float lenght = Vector3.Magnitude(m_Streching_Points[0] - m_Streching_Points[1]);
            if( ( lenght > (float)GameManager.Instance.m_Player_Avatar_Cs.No_Particles() / m_Ability1_Perc_Particle_Used * m_Ability1_Length ) ||
                ( lenght < m_Ability1_Min_Length )                                                                                                )
            {
                m_Line_Renderer.enabled = false;
            }
        }
    }

    /************************************/
    /***     Trigger Events methods   ***/
    /************************************/
    void swipe()
    {
        Stretch(TouchControlManager.Instance.GetSwipeVector());
    }

    void MoveStart()
    {
        m_Is_Moving = true;
    }

    void MoveEnd()
    {
        m_Is_Moving = false;
    }

    /*********************************/
    /****    Internal Methods     ****/
    /*********************************/

    IEnumerator set_central_particle()
    {
        yield return new WaitForSeconds(.05f);
        m_Central_Particle = GameManager.Instance.m_Central_Particle;
        m_Central_Particle_rb = m_Central_Particle.GetComponent<Rigidbody2D>();
    }

    void Stretch( Vector2 direction )
    {
        direction = Input.mousePosition - m_Screen_Size;
        Vector3 direction3 = direction.normalized;
        direction = GameManager.Instance.Rotate_By_Gravity(direction);

        float parts_used = (float) GameManager.Instance.m_Player_Avatar_Cs.No_Particles() / m_Ability1_Perc_Particle_Used;

//        Debug.Log("Position: " + (new Vector2(tr.position.x, tr.position.y) + (direction * parts_used * m_Ability1_Length)) );

        RaycastHit2D[] hits = Physics2D.LinecastAll(tr.position, new Vector2 (tr.position.x, tr.position.y) + ( direction * parts_used * m_Ability1_Length ) );
        //        Debug.Log( "Hits:" + hits.Length );

        bool hit_register = false;

        foreach( RaycastHit2D elem in hits )
        {
            if (m_Ability1_Layers == (m_Ability1_Layers | (1 << elem.collider.gameObject.layer))) // Is the gameobject layer inside the layermask?
            {
                m_Line_Renderer.enabled = true;
                Set_Points( elem.point );
                hit_register = true;
                break;
            }
        }

        if(hit_register == false)
        {
            m_Line_Renderer.enabled = false;
        }

        /*
         * Possibilities:
         0) Shit solution: Line render to show blue from one point to the other, and then forcing the central particle toward that point
         1) The particles are removed and linked togheder one after the other, made not collide between each others and
            then thrown in a direction, then the player will be naturally be pushed towards the things, and if they overlaps then are absorbed back
         2) The particles are throwed with different forces in the direction of the stretch: easiest way out, but it will make the player going around strangely
            _ maybe i can compensate the impulse added to the particles so that the player can't really move much with it.
         * */

    }

    void Set_Points(Vector3 distant_point)
    {
        m_Streching_Points = new Vector3[] { new Vector3(distant_point.x, distant_point.y, 0f), new Vector3(tr.position.x, tr.position.y, 0.0f) };
        m_Line_Renderer.SetPositions(m_Streching_Points);
    }

    void Set_Points()
    {
        m_Streching_Points[1] = tr.position;
        m_Line_Renderer.SetPositions(m_Streching_Points);
    }
}
