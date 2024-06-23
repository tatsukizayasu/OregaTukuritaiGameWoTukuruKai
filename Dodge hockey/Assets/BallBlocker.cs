using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBlocker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        if (ball != null)
        {
            ball.Block();
        }
    }
    //{
    //    Ball ball = collision.gameObject.GetComponent<Ball>();
    //    if (ball != null)
    //    {
    //        Vector3 new_velocity = (new Vector3(0.0f, 1.7f, 0.0f) - ball.transform.position).normalized * ball.Speed;
    //        Rigidbody rb = ball.GetComponent<Rigidbody>();
    //        if(rb != null )
    //        {   
    //            ball.transform.position = ball.transform.position + new_velocity;
    //            rb.velocity = new_velocity;
    //        }
    //    }
    //}
}
