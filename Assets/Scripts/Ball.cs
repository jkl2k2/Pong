using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    private Vector3 previousVelocity;
    private IEnumerator coroutine;
    private int leftScore;
    private int rightScore;
    
    public Rigidbody rb;
    public Transform leftPaddleTrans;
    public Transform rightPaddleTrans;
    public AudioClip powerUpAudio;
    public TextMeshProUGUI p1Score;
    public TextMeshProUGUI p2Score;

    private void Start()
    {
        leftScore = 0;
        rightScore = 0;
        
        double rand = Random.Range(-1f, 1f);

        if (rand < 0)
        {
            coroutine = DelayedStart(2f, -1);
            StartCoroutine(coroutine);
        }
        else
        {
            coroutine = DelayedStart(2f, 1);
            StartCoroutine(coroutine);
        }
    }

    private void FixedUpdate()
    {
        previousVelocity = rb.velocity;
        
        if (rb.position.y >= 29)
        {
            rb.position = new Vector3(rb.position.x, 26.5f, 0f);
        }

        if (rb.position.y <= 6.5)
        {
            rb.position = new Vector3(rb.position.x, 8f, 0f);
        }
    }

    private IEnumerator DelayedStart(float wait, int direction)
    {
        yield return new WaitForSeconds(wait);
        LaunchBall(direction);
    }
    
    private IEnumerator DelayedBallScaleReset(float wait)
    {
        yield return new WaitForSeconds(wait);
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
    
    private IEnumerator DelayedPaddleScaleReset(float wait)
    {
        yield return new WaitForSeconds(wait);
        leftPaddleTrans.localScale = new Vector3(1f, 1f, 1f);
        rightPaddleTrans.localScale = new Vector3(1f, 1f, 1f);
    }
    
    private void LaunchBall(int direction)
    {
        GetComponent<TrailRenderer>().emitting = true;

        if (direction == -1)
        {
            rb.velocity = new Vector3(-15f, 0f, 0f);
        }
        else if (direction == 1)
        {
            rb.velocity = new Vector3(15f, 0f, 0f);
        }
        else
        {
            Debug.Log("LaunchBall() direction argument catch");
        }
        
        StopAllCoroutines();
    }

    private void UpdateScoring(int direction)
    {
        if (direction == -1)
        {
            Debug.Log("Player 2 scored!!!");
            rightScore++;
            p2Score.SetText(rightScore.ToString());
        } else if (direction == 1)
        {
            Debug.Log("Player 1 scored!!!");
            leftScore++;
            p1Score.SetText(leftScore.ToString());
        }
        else
        {
            Debug.Log("ResetBall() scoring update value catch");
        }
    }

    private void ResetScores()
    {
        leftScore = 0;
        rightScore = 0;
        p2Score.SetText(rightScore.ToString());
        p1Score.SetText(leftScore.ToString());
    }
    
    private void ResetBall(int direction)
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        
        audioSources[1].Play();

        GetComponent<TrailRenderer>().emitting = false;
        
        rb.position = new Vector3(0f, 17.5f, 0f);
        rb.velocity = new Vector3(0f, 0f, 0f);

        UpdateScoring(direction);

        if (leftScore == 11)
        {
            Debug.Log("Game Over, Left Paddle Wins");
            ResetScores();
        }
        else if (rightScore == 11)
        {
            Debug.Log("Game Over, Right Paddle Wins");
            ResetScores();
        }
        
        Debug.Log("Current score: " + leftScore + " -- " + rightScore);

        coroutine = DelayedStart(2f, direction);
        StartCoroutine(coroutine);
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();

        audioSources[0].pitch = Math.Abs(rb.velocity.x) * 0.125f - 0.875f;
        
        audioSources[0].Play();
        
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        int rand = (int) Math.Round(Random.Range(0f, 5f));

        switch (rand)
        {
            case(0):
                renderer.material.SetColor("_Color", Color.red);
                break;
            case(1):
                renderer.material.SetColor("_Color", Color.blue);
                break;
            case(2):
                renderer.material.SetColor("_Color", Color.green);
                break;
            case(3):
                renderer.material.SetColor("_Color", Color.cyan);
                break;
            case(4):
                renderer.material.SetColor("_Color", Color.yellow);
                break;
            case(5):
                renderer.material.SetColor("_Color", Color.magenta);
                break;
        }
        
        if (collision.gameObject.tag.Equals("PlayerMiddle"))
        {
            // Debug.Log("Collided with PlayerMiddle");

            if (previousVelocity.x > 0)
                previousVelocity.x += 1;
            
            if (previousVelocity.x < 0)
                previousVelocity.x -= 1;

            rb.velocity = Vector3.Reflect(previousVelocity, Vector3.right);
        } else if (collision.gameObject.tag.Equals("PlayerTop"))
        {
            // Debug.Log("Collided with PlayerTop");

            if (previousVelocity.y < 0)
                previousVelocity.y = -previousVelocity.y;

            if (previousVelocity.y == 0)
                previousVelocity.y = 4.5f;
            
            if (previousVelocity.x > 0)
                previousVelocity.x += 1;
            
            if (previousVelocity.x < 0)
                previousVelocity.x -= 1;

            rb.velocity = Vector3.Reflect(previousVelocity, Vector3.right);
        } else if (collision.gameObject.tag.Equals("PlayerBottom"))
        {
            // Debug.Log("Collided with PlayerBottom");

            if (previousVelocity.y > 0)
                previousVelocity.y = -previousVelocity.y;
            
            if (previousVelocity.y == 0)
                previousVelocity.y = -4.5f;
            
            if (previousVelocity.x > 0)
                previousVelocity.x += 1;
            
            if (previousVelocity.x < 0)
                previousVelocity.x -= 1;

            rb.velocity = Vector3.Reflect(previousVelocity, Vector3.right);
        } else if (collision.gameObject.tag.Equals("Bounce Walls"))
        {
            // Debug.Log("Collided with wall");
            rb.velocity = Vector3.Reflect(previousVelocity, Vector3.up);
        } else if (collision.gameObject.tag.Equals("ResetLeft"))
        {
            ResetBall(-1);
        } else if (collision.gameObject.tag.Equals("ResetRight"))
        {
            ResetBall(1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("PowerUpBigBall"))
        {
            transform.localScale = new Vector3(2f, 2f, 2f);

            coroutine = DelayedBallScaleReset(5);
            StartCoroutine(coroutine);
        } else if (other.gameObject.tag.Equals("PowerUpBigPaddle"))
        {
            leftPaddleTrans.localScale = new Vector3(1f, 1.5f, 1f);
            rightPaddleTrans.localScale = new Vector3(1f, 1.5f, 1f);
            
            coroutine = DelayedPaddleScaleReset(5);
            StartCoroutine(coroutine);
        } else if (other.gameObject.tag.Equals("PowerUpCrazyBall"))
        {
            if (rb.velocity.x < 0)
            {
                rb.velocity = new Vector3(rb.velocity.x - 8, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector3(rb.velocity.x + 8, rb.velocity.y);
            }
        }
        
        GetComponents<AudioSource>()[1].PlayOneShot(powerUpAudio);
        other.gameObject.SetActive(false);
    }
}
