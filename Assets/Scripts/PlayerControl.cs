using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float left_boundary;
    [SerializeField] private float right_boundary;

    [SerializeField] private float MoveSpeed;

    private Transform ball_transform;

    public GameObject ballPrefab;
    public bool BallIsLost;
    // Start is called before the first frame update
    void Start()
    {
        BallIsLost = false;
        ball_transform = GetComponentInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if(horizontal != 0)
        {
            transform.Translate(Vector3.right * horizontal * MoveSpeed * Time.deltaTime);
            if(transform.position.x > right_boundary || transform.position.x < left_boundary)
            {
                transform.Translate(Vector3.left * horizontal * MoveSpeed * Time.deltaTime);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(transform.childCount > 0)
            {
                GetComponentInChildren<BallControl>().StartMove(horizontal);
                transform.GetChild(0).parent = null;
                
            }
        }

        if (BallIsLost)
        {
            GameObject ball = Instantiate(ballPrefab, ball_transform);
            ball.transform.parent = transform;
            BallIsLost = false;
        }
    }
}
