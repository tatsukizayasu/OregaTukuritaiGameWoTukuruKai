using Norikatuo.ReboundShot;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player_prefab;
    [SerializeField] private GameObject center_line;
    [SerializeField] private GameObject ball_prefab;

    private GameObject player1;
    private GameObject player2;
    private Norikatuo.ReboundShot.Ball_test ball;


    private void Awake()
    {
        player1 = Instantiate(player_prefab);
        player2 = Instantiate(player_prefab);
        player1.transform.position = new Vector3(-12.0f, 1.6f, 0);
        player2.transform.position = new Vector3( 12.0f, 1.6f, 0);
        Instantiate(center_line);
        ball = (Instantiate (ball_prefab)).GetComponent<Norikatuo.ReboundShot.Ball_test>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        ball.SetVelocity(new Vector3(0.0f,0.0f,1.0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
