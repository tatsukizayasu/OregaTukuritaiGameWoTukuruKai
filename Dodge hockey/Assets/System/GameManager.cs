using Norikatuo.ReboundShot;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ScoreBoard))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player_prefab;
    [SerializeField] private GameObject center_line;
    [SerializeField] private GameObject ball_prefab;
    [SerializeField] private GameObject goal_prefab;
    [SerializeField] private GameObject goal_effect;

    private GameObject[] players = new GameObject[2];
    public GameObject[] Players { get { return players; } }
    private GameObject goal1;
    private GameObject goal2;
    private GameObject ball;
    private GameObject tutorial_ball;
    private GameObject goakeffect;

    private SE_Player se_players;

    private int WinPoint = 10;


    private void Awake()
    {
        //Playerプレハブのクローン作成
        players[0] = Instantiate(player_prefab);
        players[1] = Instantiate(player_prefab);

        //座標設定用にnavmeshを解除する
        players[0].GetComponent<NavMeshAgent>().enabled = false;
        players[1].GetComponent<NavMeshAgent>().enabled = false;

        //座標の設定
        players[0].transform.position = new Vector3(-12.0f, 1.6f, 0);
        players[1].transform.position = new Vector3( 12.0f, 1.6f, 0);

        //navmeshの有効化
        players[0].GetComponent<NavMeshAgent>().enabled = true;
        players[1].GetComponent<NavMeshAgent>().enabled = true;

        if (PlayerPrefs.GetInt("isToPlayTutorial") == 0) 
        {
            //Goalプレハブのクローン作成
            goal1 = Instantiate(goal_prefab);
            goal2 = Instantiate(goal_prefab);
            //座標の設定
            goal1.transform.position = new Vector3(27f, 2.7f, 0);
            goal2.transform.position = new Vector3(-27f, 2.7f, 0);
        }
        else
        {
            tutorial_ball = Instantiate(ball_prefab);
        }
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
        
        
        if(PlayerPrefs.GetInt("isToPlayTutorial") != 0)
        {
            //Ballの移動方向を設定
            ball.transform.position = new Vector3(3.0f, 1.6f, 12.0f);
            BallComponent.SetVelocity(new Vector3(0.003f, 0.0f, -0.01f) * BallComponent.Speed);

            Ball BallComponent2 = tutorial_ball.GetComponent<Ball>();
            tutorial_ball.transform.position = new Vector3(-3.0f, 1.6f, 12.0f);
            BallComponent2.SetVelocity(new Vector3(-0.003f, 0.0f, -0.01f) * BallComponent2.Speed);

        }
        else
        {
            // GoalIdentifierスクリプトを追加し、識別子を設定
            goal1.GetComponent<Goal>().GoalID = 0;
            goal2.GetComponent<Goal>().GoalID = 1;
            BallComponent.SetVelocity(new Vector3(0.0f, 0.0f, -1.0f) * BallComponent.Speed);
        }

        se_players =  GetComponent<SE_Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Title");
        }
            
    }

    public void Goal (GameObject goal,Vector3 locathion)
    {

        int GoalID = goal.GetComponent<Goal>().GoalID;

        var scorboard = GetComponent<ScoreBoard>();

        scorboard.AddScore(GoalID);

        if(scorboard.GetScore(0) >= WinPoint)
        {
            SceneManager.LoadScene("result_0_win");
        }else if(scorboard.GetScore(1) >= WinPoint)
        {
            SceneManager.LoadScene("result_1_win");
        }

        se_players.PlayGoal();

        goakeffect = Instantiate(goal_effect);
        goakeffect.transform.position = locathion;
        Destroy(goakeffect,3);

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
