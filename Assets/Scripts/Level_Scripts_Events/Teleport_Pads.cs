using UnityEngine;
using System.Collections;

public class Teleport_Pads : MonoBehaviour
{
    [Tooltip("The other teleport pad gemel")]
    public GameObject m_Other_Teleport_Pad;
    [Tooltip("The teleport will have this offset in the direction facing by the pad")]
    public float m_OffSet = 1.0f;
    [Tooltip("The speed of teleported object will be multiply by this magnitude")]
    public float m_Power = 1.0f;        

    private Vector3 m_other_end_loc;

    

	// Use this for initialization
	void Start () {
        m_other_end_loc = m_Other_Teleport_Pad.transform.position + (m_Other_Teleport_Pad.transform.right * -m_OffSet);
//        Debug.Log(m_other_end_loc);
	}
	
	// Update is called once per frame
	void Update () {
	    //
	}

    void OnTriggerEnter2D( Collider2D coll )
    {
        ITeleport tp;
        // check if it's a player "ball"
        if ( coll.tag == "Player")
        {
           tp = (ITeleport)GameManager.Instance.m_Player.GetComponent(typeof(ITeleport));
        }
        else
        {
            tp = (ITeleport)gameObject.GetComponent(typeof(ITeleport));
        }

        if(tp != null)
        {
            tp.Teleport_To(m_other_end_loc, -m_Other_Teleport_Pad.transform.right * m_Power);
        }
    }
}
