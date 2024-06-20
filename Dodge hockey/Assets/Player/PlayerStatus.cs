using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private float catchRange = 5;
    [SerializeField] private int life = 2;
    public int Life { get; set; }
    public float CatchRamge { set { catchRange = value; } get { return catchRange; } }

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