using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private float catchRange = 5;
    public float CatchRamge { set { catchRange = value; } get { return catchRange; } }

    public int default_life = 3;
    private int life = 3;
    public int Life { get { return life; }set { life = value; } }

    public float default_speed = 10.0f;
    private float speed = 10;
    public float Speed { get { return speed; }set { speed = value; } }

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