using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE_Player : MonoBehaviour
{
    public AudioClip Catch;
    public AudioClip Throw;
    public AudioClip BallPlayerHit;
    public AudioClip BallHtiWall;
    public AudioClip BallHtiWall2;
    public AudioClip Goal;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    //�L���b�`SE
    public void PlayCatch()
    {
        audioSource.PlayOneShot(Catch);
    }

    //������SE
    public void PlayThrow()
    {
        audioSource.PlayOneShot(Throw);
    }

    //�{�[�����v���C���[�ɓ�����SE
    public void PlayBallPlayerHit()
    {
        audioSource.PlayOneShot(BallPlayerHit);
    }

    //�{�[�����ǂɓ�����SE
    public void PlayBallHtiWall()
    {
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            //print("0");
            audioSource.PlayOneShot(BallHtiWall);
        }
        else
        {
            //print("1");
            audioSource.PlayOneShot(BallHtiWall2);
        }
        
        
    }
    //Goal
    public void PlayGoal()
    {
        audioSource.PlayOneShot(Goal);
    }


}
