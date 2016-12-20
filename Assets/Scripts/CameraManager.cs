using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>{
    Transform tr;

    [Tooltip("Parent of the two gameplay area camera(the ones that have to follow the player")]
    public GameObject m_Player_Camera;
    [Tooltip("Camera focussed object")]
    public GameObject m_Camera_Focus;
    [Tooltip("Multiplier to the speed at which the camera follow the player")]
    public float m_Camera_Speed = 4.0f;
    [Tooltip("Multiplier to the speed at which the camera will rotate towards the gravity")]
    public float m_Camera_Rotation_Speed = 4.0f;
    [Tooltip("The camera will not adjust position when xy coords are this discrete distance from the player")]
    public float m_Camera_Abs_Alt_Distance = 0.2f;

    private bool m_isCameraMoving = false;
    private bool m_isCameraRotating = false;

    private Vector3 m_cam_grav_vector;
    private float m_Last_Cam_Update_Time;

    private Vector3 m_Target_Position;

    private Quaternion m_Target_Rotation;

    // Use this for initialization
    void Start () {
        tr = gameObject.GetComponent<Transform>();
        if (m_Camera_Focus == null) m_Camera_Focus = GameObject.Find("Player");

        GameObject Player_start = GameObject.Find("PlayerStart");

        m_cam_grav_vector = new Vector3(Physics2D.gravity.x, Physics2D.gravity.y, 0.0f);
        if (m_Player_Camera == null) m_Player_Camera = GameObject.Find("Player_Cameras");

        m_Player_Camera.transform.rotation.SetLookRotation(m_cam_grav_vector, Vector3.up);

        Vector3 new_position = (Player_start != null) ? Player_start.transform.position : m_Camera_Focus.transform.position;
        new_position.z = m_Player_Camera.transform.position.z;
        m_Player_Camera.transform.position = new_position;
    }

    void Update()
    {
        if ((Physics2D.gravity.x != m_cam_grav_vector.x) ||
            (Physics2D.gravity.y != m_cam_grav_vector.y)     )
        {
            m_isCameraRotating = true;
            m_cam_grav_vector = Physics2D.gravity;
            m_Target_Rotation = Quaternion.LookRotation(new Vector3(0, 0, 1), -m_cam_grav_vector);
        }

        //       m_isCameraMoving = ;
        if (!Aprox(m_Player_Camera.transform.position, m_Camera_Focus.transform.position))
        {
            //Debug.Log(tr.transform.position + " " + m_Camera_Focus.transform.position);
            Vector3 new_position = Vector3.Lerp(m_Player_Camera.transform.position, m_Camera_Focus.transform.position, Time.deltaTime * m_Camera_Speed);
            new_position.z = m_Player_Camera.transform.position.z;
            m_Player_Camera.transform.position = new_position;
        }

    }

    void LateUpdate()
    {
        
        m_Player_Camera.transform.rotation = Quaternion.Lerp( m_Player_Camera.transform.rotation, Quaternion.LookRotation(new Vector3(0, 0, 1), -m_cam_grav_vector), Time.deltaTime * m_Camera_Rotation_Speed );
        /*
    private float m_Target_Angle;
    private float m_Current_Angle;
    */
    }

    bool Aprox(Vector3 arg1, Vector3 arg2)  // Return true if two vector are similar enough(based on m_Camera_Abs_Alt_Distance)
    {
        Vector2 argo1 = arg1;
        Vector2 argo2 = arg2;
        return (Mathf.Abs( Vector3.Magnitude(argo1 - argo2) ) < m_Camera_Abs_Alt_Distance) ? true : false;
    }

    public void Reset_To_Start()
    {
        GameObject Player_start = GameObject.Find("PlayerStart");
        Vector3 new_position = (Player_start != null) ? Player_start.transform.position : m_Camera_Focus.transform.position;
        new_position.z = m_Player_Camera.transform.position.z;
        m_Player_Camera.transform.position = new_position;
    }
}
