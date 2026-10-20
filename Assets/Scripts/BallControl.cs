using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BallControl : MonoBehaviour
{
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float ExceedSpeed;

    private bool IsStart;
    private Vector3 point;

    private Rigidbody2D rb;
    private PlayerControl player;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent <Rigidbody2D>();
        player = GetComponentInParent<PlayerControl>();
    }

    public void StartMove(float dir)
    {
        rb.velocity = new Vector3(dir, 1, 0).normalized * MoveSpeed;
        IsStart = true;

        point = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsStart)
        {
            return;
        }
       if(transform.position == point)
        {
            return;
        }

       if(collision.collider.tag == "Brick")
        {
            Destroy(collision.collider.gameObject);
        }

        Vector2 inVec = transform.position - point;
        point = transform.position;
        Vector2 outVec = Vector2.Reflect(inVec, collision.contacts[0].normal);
        rb.velocity = outVec.normalized * MoveSpeed;


        //经过玩家平台碰撞后适当加速
        if(collision.collider.tag == "Player") 
        {
            rb.velocity += new Vector2(0, ExceedSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player.BallIsLost = true;
        Destroy(gameObject);
    }
}
