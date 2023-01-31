using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Jumper : Agent
{
    [SerializeField] private float jumpForce;
    [SerializeField] private KeyCode jumpKey;
    
    private bool jumpIsReady = true;
    private Rigidbody rBody;
    private Vector3 startingPosition;
    private int score = 0;
    public event Action OnReset;
    

    public override void Initialize() 
    {
        rBody = GetComponent<Rigidbody>();
        startingPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (jumpIsReady) {
            RequestDecision();
            }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var movement = actionBuffers.DiscreteActions[0];
        if (movement == 1) {
            Jump();
        }
    }

    public override void OnEpisodeBegin()
    {
        // Reset enviroment
        Reset();
    }

    public override void Heuristic(in ActionBuffers actionBuffersOut)
    {
        // Player Input 
        var discreteActions = actionBuffersOut.DiscreteActions;
        discreteActions[0] = 0;
        if (Input.GetKey(jumpKey))
            discreteActions[0] = 1;
    }       

    private void Jump()
    {
        if (jumpIsReady)
        {
            rBody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
            jumpIsReady = false;
        }
    }

    private void Reset()
    {
        score = 0;
        jumpIsReady = true;
        
        //Reset Movement and Position
        transform.position = startingPosition;
        rBody.velocity = Vector3.zero;
        
        OnReset?.Invoke();
    }

    private void OnCollisionEnter(Collision collidedObj)
    {
        if (collidedObj.gameObject.CompareTag("Street"))
            jumpIsReady = true;

        else if (collidedObj.gameObject.CompareTag("Mover") || collidedObj.gameObject.CompareTag("DoubleMover")) { 
            AddReward(-1.0f);
            EndEpisode();
            }
    }

    private void OnTriggerEnter(Collider collidedObj)
    {
        if (collidedObj.gameObject.CompareTag("score"))
        {
            AddReward(0.1f);
            score++;
            ScoreCollector.Instance.AddScore(score);
        }
    }
}
