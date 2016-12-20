using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class Enemy : MonoBehaviour
{

    [Range(0f, 5f)]
    public float m_speed = 1f;
    // Test clip
    public AudioClip testClip;
    public GameObject m_shot_prefab;

    private int verse = 1;
    private Transform tr;
    private SpriteRenderer sr;

    [Range(0.1f,2f)]
    public float raycastMagnitude = 0.5f;
    private Vector2 rcFloorDir; //new Vector2(1f,-1f); diagonal vector
    private Vector2 rcRightDir;
    private Vector2 rcLeftDir;    

    // Animator related variables
    private Animator animator;
    private bool moving = false;
    private bool shooting = false;

    void Start()
    {
        tr = GetComponent<Transform>() as Transform;
        sr = GetComponent<SpriteRenderer>() as SpriteRenderer;
        animator = GetComponent<Animator>() as Animator;
        rcFloorDir = -tr.up;
        rcRightDir = tr.right;
        rcLeftDir = -rcRightDir;
    }

    void Update()
    {
        animator.SetFloat("Speed", m_speed);
        animator.SetBool("Shooting", shooting);
    }

    void FixedUpdate()
    {
        if (m_speed == 0f) moving = false;
        Move();

        // Shoot player if seen in straight line
        /*
        RaycastHit2D[] hits = Physics2D.RaycastAll(tr.position, verse * Vector2.right);
        if (hits != null)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    Shoot(hit.collider.gameObject);
                }
            }
        } */

    }

    void Idle()
    {
        moving = false;
        m_speed = 0;
    }

    void Move()
    {
        moving = true;
        RaycastHit2D[] hits = Physics2D.RaycastAll(tr.position, rcFloorDir, raycastMagnitude);
        RaycastHit2D[] hitsRight = Physics2D.RaycastAll(tr.position, rcRightDir, raycastMagnitude);
        RaycastHit2D[] hitsLeft = Physics2D.RaycastAll(tr.position, rcLeftDir, raycastMagnitude);

        if(hits.Length <= 1 || hitsRight.Length > 1 || hitsLeft.Length > 1) Turn();

        // Move
        tr.position = tr.position + verse * m_speed * transform.right * Time.fixedDeltaTime;
    }

    void Turn()
    {
        verse *= -1;    // Move in other direction
        sr.flipX = verse > 0 ? false : true;    // Flip the sprite
    }

    // TODO
    void Shoot(GameObject player)
    {
        shooting = true;
        // GameObject go = ObjectPoolingManager.Instance.GetObject(m_shot_prefab.name);
        Vector2 direction = player.transform.position - tr.position;

        // SoundManager.Instance.PlayModPitch(shoot_clip);
    }

    // [TEMP] SetActive(false) if collides with player
    /*
    void OnCollisionEnter2D(Collision2D other)
    {
        //Turn();
        if(other.gameObject.tag == "Player")
        {
            // Play test sound when this dies
            SoundManager.Instance.PlayModPitch(testClip);
        }
    }
    */
}
