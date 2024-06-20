using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private float catchRange = 5;
    public float CatchRamge { set { catchRange = value; } get { return catchRange; } }

    [SerializeField] private int life = 2;
    public int Life { get; set; }

    [SerializeField] private float speed = 10;
    public float Speed { get; set; }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is cal
    // led once per frame
    void Update()
    {

    }
    
}