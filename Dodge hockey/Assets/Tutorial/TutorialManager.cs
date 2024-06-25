using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(GameManager))]
[RequireComponent(typeof(PlayerInput))]
public class TutorialManager : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject[] players;
    private PlayerController[] playerControllers;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        players = gameManager.Players;

        Time.timeScale = 0;
        //playerControllers[i] = players[i++].GetComponent<PlayerController>();
        //playerControllers[i] = players[i++].GetComponent<PlayerController>();

        //if (playerControllers[0] != null)
        //{
        //    playerControllers[0].IsTutorial  = true;
        //}
    }



    // Update is called once per frame
    void Update()
    {

    }

    private void OnMove(InputValue value)
    {
        
    }

}
