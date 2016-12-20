using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IConsoleIteration {

    public GameObject Slab;
    public GameObject Engine;

    [Tooltip("Speed at which the door open")]
    public float m_Time;
    [Tooltip("Distance of the slab to open")]
    public float m_Distance;
    [Tooltip("Tick time, every how ofter the slab will move")]
    public float m_TickTime;

    private float m_Start_Moving = 0.0f;
    private Transform m_Slab_tr;

    void Start()
    {
        m_Slab_tr = Slab.GetComponent<Transform>();
    }

	public void Activate_Once()
	{
        // Open the fucking door!
        InvokeRepeating("OpenTheDoor", m_TickTime, m_TickTime);
    }

    void OpenTheDoor()
    {
        m_Start_Moving += m_TickTime;
        Vector3 position = new Vector3(m_Slab_tr.position.x, m_Slab_tr.position.x, Mathf.Lerp(0f, m_Distance, m_Start_Moving / m_Time));
        m_Slab_tr.position = position;
    }
}
