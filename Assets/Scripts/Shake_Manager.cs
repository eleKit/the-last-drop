using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;

public class Shake_Manager : Singleton<Shake_Manager> {

    //Every how much time the input acceleration is controlled:
    public float m_Shake_Tick = 0.03f;
    //after how many up&down the shake is sent
    public float m_Shake_No_Min = 3;

    // Counting the number of shakes the phone recieve
    public int m_Shake_No_Shakes = 0;


    // Minimum acceleration to register
    public float m_Shake_Min_Accel = 0.3f;
    // After how much time without shake the shake_no counter reset
    public float m_Shake_Time_Reset = 0.3f;

    public int m_Max_Values = 10;//max number of acceleration update to calculate averange

    public Vector3 m_Current_Accel;
    public Vector3 m_Unbiased_Accel;

    private Vector3 m_Avrg_Accel;

    private Queue<Vector3> m_Que_Accels;
    private Vector3 m_Shake_Last_Peak;

    private float m_Shake_Last_Time;

    void OnEnable()
    {
        InvokeRepeating("Check_Shake", m_Shake_Tick, m_Shake_Tick);
        m_Que_Accels = new Queue<Vector3>();
    }

    void OnDisable()
    {
        CancelInvoke("Check_Shake");
    }
	
	public void Check_Shake () {


        m_Current_Accel = Input.acceleration;

        m_Que_Accels.Enqueue(m_Current_Accel);
        if (m_Que_Accels.Count > m_Max_Values)
            m_Que_Accels.Dequeue();

        m_Avrg_Accel.Set(0f, 0f, 0f);

        foreach(Vector3 elem in m_Que_Accels)
        {
            m_Avrg_Accel += elem;
        }

        m_Avrg_Accel /= m_Que_Accels.Count;

        m_Unbiased_Accel = m_Current_Accel - m_Avrg_Accel;

        if( Mathf.Abs(m_Unbiased_Accel.z) > m_Shake_Min_Accel )   // se l'accelerazione è ALMENO pari a
        {

            if ( m_Unbiased_Accel.z >= 0 )    // Se l'accelerazione è positiva, checka se il picco precedente era negativo e vice versa, se si, nuovo picco
            {
                if( m_Shake_Last_Peak.z <= 0)
                {
                    m_Shake_No_Shakes++;
                    m_Shake_Last_Peak = m_Unbiased_Accel;
                    m_Shake_Last_Time = Time.time;
                } 
            }
            else
            {
                if (m_Shake_Last_Peak.z > 0)
                {
                    m_Shake_No_Shakes++;
                    m_Shake_Last_Peak = m_Unbiased_Accel;
                    m_Shake_Last_Time  = Time.time;
                }
            }
        }

        if( Time.time - m_Shake_Last_Time > m_Shake_Time_Reset )
        {
            m_Shake_No_Shakes = 0;
        }

        if(m_Shake_No_Shakes > m_Shake_No_Min)
        {
            m_Shake_No_Shakes = 0;
            m_Shake_Last_Time = Time.time;
            EventManager.TriggerEvent("Shake");
        }

    }
}
