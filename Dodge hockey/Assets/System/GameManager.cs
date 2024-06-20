using Norikatuo.ReboundShot;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ScoreBoard))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player_prefab;
    [SerializeField] private GameObject center_line;
    [SerializeField] private GameObject ball_prefab;
    [SerializeField] private GameObject goal_prefab;

    private GameObject[] players = new GameObject[2];
    public GameObject[] Players { get { return players; } }
    private GameObject goal1;
    private GameObject goal2;
    private GameObject ball;

    private int[] PlayerPoint = {0,0};


    private void Awake()
    {
        //Playerプレハブのクローン作成
        players[0] = Instantiate(player_prefab);
        players[1] = Instantiate(player_prefab);
        //座標の設定
        players[0].transform.position = new Vector3(-12.0f, 1.6f, 0);
        players[1].transform.position = new Vector3( 12.0f, 1.6f, 0);

        //Goalプレハブのクローン作成
        goal1 = Instantiate(goal_prefab);
        goal2 = Instantiate(goal_prefab);
        //座標の設定
        goal1.transform.position = new Vector3(27f, 2.7f, 0);
        goal2.transform.position = new Vector3(-27f, 2.7f, 0);

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
        BallComponent.SetVelocity(new Vector3(0.0f, 0.0f, 1.0f) * BallComponent.Speed);

        // GoalIdentifierスクリプトを追加し、識別子を設定
        goal1.GetComponent<Goal>().GoalID = 0;
        goal2.GetComponent<Goal>().GoalID = 1;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Goal (GameObject goal)
    {
        int GoalID = goal.GetComponent<Goal>().GoalID;

        GetComponent<ScoreBoard>().AddScore(GoalID);

        Destroy(ball);

        //Ballプレハブのクローン作成
        ball = Instantiate(ball_prefab);
        Ball BallComponent = ball.GetComponent<Ball>();
        if (GoalID == 0)
        {
            ball.transform.position = new Vector3(3.0f, 1.6f, 12.0f);
            BallComponent.SetVelocity(new Vector3(0.003f, 0.0f, -0.01f) * BallComponent.Speed);
        }
        else
        {
            ball.transform.position = new Vector3(-3.0f, 1.6f, 12.0f);
            BallComponent.SetVelocity(new Vector3(-0.003f, 0.0f, -0.01f) * BallComponent.Speed);
        }
    }
}
