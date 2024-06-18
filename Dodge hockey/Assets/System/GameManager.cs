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
    [SerializeField] private GameObject goal_prefab;

    private GameObject player1;
    private GameObject player2;
    private GameObject goal1;
    private GameObject goal2;
    private GameObject ball;

    private int[] PlayerPoint = {0,0};


    private void Awake()
    {
        //Playerプレハブのクローン作成
        player1 = Instantiate(player_prefab);
        player2 = Instantiate(player_prefab);
        //座標の設定
        player1.transform.position = new Vector3(-12.0f, 1.6f, 0);
        player2.transform.position = new Vector3( 12.0f, 1.6f, 0);

        //Goalプレハブのクローン作成
        goal1 = Instantiate(goal_prefab);
        goal2 = Instantiate(goal_prefab);
        //座標の設定
        goal1.transform.position = new Vector3(-27f, 2.7f, 0);
        goal2.transform.position = new Vector3(27f, 2.7f, 0);

        //センターラインを設置
        Instantiate(center_line);

        //Ballプレハブのクローン作成
        ball = Instantiate (ball_prefab);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Ballの移動方向を設定
        Ball BallComponent = ball.GetComponent<Ball>();
        BallComponent.SetVelocity(new Vector3(0.0f, 0.0f, 1.0f) * BallComponent.speed);

        // GoalIdentifierスクリプトを追加し、識別子を設定
        goal1.GetComponent<Goal>().GoalID = 1;
        goal2.GetComponent<Goal>().GoalID = 2;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Goal (GameObject goal)
    {
        int GoalID = goal.GetComponent<Goal>().GoalID;

        PlayerPoint[GoalID-1]++;

        Destroy(ball);

        //Ballプレハブのクローン作成
        ball = Instantiate(ball_prefab);
        ball.transform.position = new Vector3(-12.0f, 1.6f, 0);
        Ball BallComponent = ball.GetComponent<Ball>();
        BallComponent.SetVelocity(new Vector3(0.0f, 0.0f, 1.0f) * BallComponent.speed);

        Debug.Log("P1");
        Debug.Log(PlayerPoint[0]);
        Debug.Log("P2");
        Debug.Log(PlayerPoint[1]);
    }
}
