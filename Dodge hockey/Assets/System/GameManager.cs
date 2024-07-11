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
    [SerializeField] private GameObject goal_effect_prefab;

    private GameObject[] players = new GameObject[2];
    public GameObject[] Players { get { return players; } }
    private GameObject goal1;
    private GameObject goal2;
    private GameObject[] balls = new GameObject[2];
    private GameObject goal_effect;

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

        //Goalプレハブのクローン作成
        goal1 = Instantiate(goal_prefab);
        goal2 = Instantiate(goal_prefab);

        //座標の設定
        goal1.transform.position = new Vector3(28f, 2.7f, 0);
        goal2.transform.position = new Vector3(-28f, 2.7f, 0);

        // Goalスクリプトを追加し、識別子を設定
        goal1.GetComponent<Goal>().GoalID = 0;
        goal2.GetComponent<Goal>().GoalID = 1;

        
        //センターラインを設置
        Instantiate(center_line);

        //Ballプレハブのクローン作成
        balls[0] = Instantiate (ball_prefab);
        
        if (PlayerPrefs.GetInt("isToPlayTutorial", 0) != 0)
        {
            //  チュートリアル中ならボールを追加する。
            balls[1] = Instantiate(ball_prefab);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Ballの移動方向を設定
        Ball BallComponent = balls[0].GetComponent<Ball>();
        
        
        if(PlayerPrefs.GetInt("isToPlayTutorial") != 0)
        {
            //Ballの移動方向を設定
            balls[0].transform.position = new Vector3(3.0f, 1.6f, 12.0f);
            BallComponent.SetVelocity(new Vector3(0.003f, 0.0f, -0.01f) * BallComponent.Speed);

            BallComponent = balls[1].GetComponent<Ball>();
            balls[1].transform.position = new Vector3(-3.0f, 1.6f, 12.0f);
            BallComponent.SetVelocity(new Vector3(-0.003f, 0.0f, -0.01f) * BallComponent.Speed);

        }
        else
        {
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

    public void Goal (GameObject goal,GameObject ball)
    {
        int GoalID = goal.GetComponent<Goal>().GoalID;

        if(PlayerPrefs.GetInt("isToPlayTutorial") == 0)
        { 
            //  チュートリアル中じゃなければスコアを加算する
            var score_board = GetComponent<ScoreBoard>();

            score_board.AddScore(GoalID);

            if (score_board.GetScore(0) >= WinPoint)
            {
                SceneManager.LoadScene("result_0_win");
            }
            else if (score_board.GetScore(1) >= WinPoint)
            {
                SceneManager.LoadScene("result_1_win");
            }
        }

        se_players.PlayGoal();

        //  ゴールに入ったボールを検索して消す。
        for(int i = 0; i < balls.Length;i++)
        {
            if (balls[i] == ball)
            {
                goal_effect = Instantiate(goal_effect_prefab);
                goal_effect.transform.position = balls[i].transform.position;
                Destroy(goal_effect, 3);

                Destroy(balls[i]);


                //  リスポーンさせる                
                balls[i] = Instantiate(ball_prefab);
                Ball ball_component = balls[i].GetComponent<Ball>();

                if (GoalID == 0)
                {
                    balls[i].transform.position = new Vector3(3.0f, 1.6f, 12.0f);
                    ball_component.SetVelocity(new Vector3(0.003f, 0.0f, -0.01f) * ball_component.Speed);
                }
                else
                {
                    balls[i].transform.position = new Vector3(-3.0f, 1.6f, 12.0f);
                    ball_component.SetVelocity(new Vector3(-0.003f, 0.0f, -0.01f) * ball_component.Speed);
                }

            }
        }

    }

}
