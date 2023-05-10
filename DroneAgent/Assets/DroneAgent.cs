using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.ArrowIndicator;
using System;

public class DroneAgent : Agent
{
    // Drone parameters
    Rigidbody ourDrone;
    private float upForce = 98.1f;
    private float movementForwardSpeed = 500f;  // 500
    private float tiltAmountForward = 0f;
    private float tiltVelocityForward;
    
    private float wantedYRotation;
    public float currentYRotation;
    private float rotateAmoutByKeys = 2.5f;
    private float rotationYVelocity;
    
    private Vector3 velocityToSmoothDampToZero;

    private float sideMovementAmount = 300f;
    private float tiltAmountSideways;
    private float tiltAmountVelocity;

    // Target

    public GameObject Target;
    public GameObject CheckPoint1;
    public GameObject CheckPoint2;
    public GameObject CheckPoint3;
    public GameObject CheckPoint4;
    public GameObject CheckPoint5;

    //Target the agent will walk towards during training.
    [Header("Target To Fly Towards")] public Transform target;

    //The direction an agent will walk during training.
    private Vector3 m_WorldDirToWalk = Vector3.right;

    // Canvas
    float currentTime = 0f;
    float startingTime = 20f;
    public TextMeshProUGUI txtCountdown;

    // Drone parts
    public Transform drone;

    //This will be used as a stabilized model space reference point for observations
    //Because ragdolls can move erratically during training, using a stabilized reference transform improves learning
    OrientationCubeController m_OrientationCube;

    // Direction Indicator
    //public bool updatedByAgent;
    //public Transform transformToFollow; //ex: hips or body
    //public Transform targetToLookAt; //target in the scene the indicator will point to
    //public float heightOffset;
    //private float m_StartingYPos;
    DirectionIndicator m_DirectionIndicator;


    public override void Initialize()
    {
        // Orientation target
        m_OrientationCube = GetComponentInChildren<OrientationCubeController>();
        m_DirectionIndicator = GetComponentInChildren<DirectionIndicator>();

        currentTime = startingTime;
        ourDrone = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        currentYRotation = 0f;
        wantedYRotation = 0;
        rotationYVelocity = 0;
        ourDrone.transform.localPosition = new Vector3(0f, 0f, 0f);
        ourDrone.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        ourDrone.angularVelocity = Vector3.zero;
        ourDrone.velocity = Vector3.zero;


        CheckPoint1.SetActive(true);
        CheckPoint2.SetActive(false);
        CheckPoint3.SetActive(false);
        CheckPoint4.SetActive(false);
        CheckPoint5.SetActive(false);
        Target.SetActive(false);

        target = CheckPoint1.transform;
        m_DirectionIndicator.targetToLookAt = target;

        //Target.localPosition = new Vector3(10f, 0.83f, -9);
        currentTime = startingTime;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var cubeForward = m_OrientationCube.transform.forward;

        //rotation deltas
        sensor.AddObservation(Quaternion.FromToRotation(drone.forward, cubeForward));
        //sensor.AddObservation(Quaternion.FromToRotation(head.forward, cubeForward));

        //Position of target position relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformPoint(target.transform.position));

        //sensor.AddObservation(StepCount / (float)MaxStep);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //Debug.Log(actual_target.position);

        MovementUpDown(actionBuffers);
        MovementForward(actionBuffers);
        Rotation(actionBuffers.DiscreteActions);
        ClampingSpeedValues(actionBuffers);
        Swerwe(actionBuffers);

        ourDrone.AddRelativeForce(Vector3.up * upForce);
        ourDrone.rotation = Quaternion.Euler(new Vector3(tiltAmountForward, currentYRotation, tiltAmountSideways));


        // Canva update and reward
        currentTime -= 1 * Time.deltaTime;
        txtCountdown.text = currentTime.ToString("F1");
        if (currentTime <= 0)
        {
            currentTime = 0;
            AddReward(-0.05f);  // Penalization for exceeded
            //EndEpisode();  // while testing 
        }

        // Penalty given each step to encourage agent to finish task quickly.
        AddReward(-0.1f / MaxStep);

        // Testing values
        //Debug.Log(m_OrientationCube.transform.rotation);
        //Debug.Log(m_DirectionIndicator.transform.rotation);
        //Debug.Log(ourDrone.rotation);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        var continuousActionsOut = actionsOut.ContinuousActions;

        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.L))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.I))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.J))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.K))
        {
            discreteActionsOut[0] = 2;
        }
    }


    public void MovementUpDown(ActionBuffers actionBuffers)
    {
        var actionZ = actionBuffers.ContinuousActions[0];  // horizontal
        var actionX = actionBuffers.ContinuousActions[1];  // vertical

        var action = actionBuffers.DiscreteActions[0];

        if ((Mathf.Abs(actionX) > 0.2f || Mathf.Abs(actionZ) > 0.2f))
        {
            if (action==1 || action==2)
            {
                ourDrone.velocity = ourDrone.velocity;
            }
            if (!(action==1) && !(action==2) && !(action==4) && !(action==3))
            {
                ourDrone.velocity = new Vector3(ourDrone.velocity.x, Mathf.Lerp(ourDrone.velocity.y, 0, Time.deltaTime * 5), ourDrone.velocity.z);
                upForce = 281;
            }
            if (!(action==1) && !(action==2) && action==4 || action==3)
            {
                ourDrone.velocity = new Vector3(ourDrone.velocity.x, Mathf.Lerp(ourDrone.velocity.y, 0, Time.deltaTime * 5), ourDrone.velocity.z);
                upForce = 110;
            }
            if (action==4 || action==3)
            {
                upForce = 410;
            }
        }
        if ((Mathf.Abs(actionX) < 0.2f && Mathf.Abs(actionZ) > 0.2f))
        {
            upForce = 135;
        }

        if (action==1)
        {
            upForce = 450f;
            if (Mathf.Abs(actionZ) > 0.2f)
            {
                upForce = 500f;
            }
        }
        else if (action==2)
        {
            upForce = -200f;
        }
        else if (!(action==1) && !(action==2) && (Mathf.Abs(actionX) < 0.2f && Mathf.Abs(actionZ) < 0.2f))
        {
            upForce = 98.1f;
        }
    }
    public void MovementForward(ActionBuffers actionBuffers)
    {
        var actionZ = actionBuffers.ContinuousActions[0];  // horizontal
        var actionX = actionBuffers.ContinuousActions[1];  // vertical

        if (actionX != 0)
        {
            ourDrone.AddRelativeForce(Vector3.forward * actionX * movementForwardSpeed);
            tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 20 * actionX, ref tiltVelocityForward, 0.1f);
        }
    }
    public void Rotation(ActionSegment<int> act)
    {
        var action = act[0];
        if (action==4)
        {
            wantedYRotation -= rotateAmoutByKeys;
        }
        if (action==3)
        {
            wantedYRotation += rotateAmoutByKeys;
        }
        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, 0.25f);
    }
    public void ClampingSpeedValues(ActionBuffers actionBuffers)
    {
        var actionZ = actionBuffers.ContinuousActions[0];  // horizontal
        var actionX = actionBuffers.ContinuousActions[1];  // vertical

        if (Mathf.Abs(actionX) > 0.2f && Mathf.Abs(actionZ) > 0.2f)
        {
            ourDrone.velocity = Vector3.ClampMagnitude(ourDrone.velocity, Mathf.Lerp(ourDrone.velocity.magnitude, 10f, Time.deltaTime * 5f));
        }
        if (Mathf.Abs(actionX) > 0.2f && Mathf.Abs(actionZ) < 0.2f)
        {
            ourDrone.velocity = Vector3.ClampMagnitude(ourDrone.velocity, Mathf.Lerp(ourDrone.velocity.magnitude, 10f, Time.deltaTime * 5f));
        }
        if (Mathf.Abs(actionX) < 0.2f && Mathf.Abs(actionZ) > 0.2f)
        {
            ourDrone.velocity = Vector3.ClampMagnitude(ourDrone.velocity, Mathf.Lerp(ourDrone.velocity.magnitude, 5f, Time.deltaTime * 5f));
        }
        if (Mathf.Abs(actionX) < 0.2f && Mathf.Abs(actionZ) < 0.2f)
        {
            ourDrone.velocity = Vector3.SmoothDamp(ourDrone.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.95f);
        }
    }
    public void Swerwe(ActionBuffers actionBuffers)
    {
        var actionZ = actionBuffers.ContinuousActions[0];  // horizontal
        var actionX = actionBuffers.ContinuousActions[1];  // vertical

        if (Mathf.Abs(actionZ) > 0.2f)
        {
            ourDrone.AddRelativeForce(Vector3.right * actionZ * sideMovementAmount);
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, -20 * actionZ, ref tiltAmountVelocity, 0.1f);
        }
        else
        {
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 0, ref tiltAmountVelocity, 0.1f);
        }
    }


    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("CheckPoint1"))
        {
            col.gameObject.SetActive(false);
            CheckPoint2.SetActive(true);
            target = CheckPoint2.transform;
            m_DirectionIndicator.targetToLookAt = target;
            AddReward(0.1f);
        }
        else if (col.gameObject.CompareTag("CheckPoint2"))
        {
            col.gameObject.SetActive(false);
            CheckPoint3.SetActive(true);
            target = CheckPoint3.transform;
            m_DirectionIndicator.targetToLookAt = target;
            AddReward(0.2f);
        }
        else if (col.gameObject.CompareTag("CheckPoint3"))
        {
            col.gameObject.SetActive(false);
            CheckPoint4.SetActive(true);
            target = CheckPoint4.transform;
            m_DirectionIndicator.targetToLookAt = target;
            AddReward(0.3f);
        }
        else if (col.gameObject.CompareTag("CheckPoint4"))
        {
            col.gameObject.SetActive(false);
            CheckPoint5.SetActive(true);
            target = CheckPoint5.transform;
            m_DirectionIndicator.targetToLookAt = target;
            AddReward(0.4f);
        }
        else if (col.gameObject.CompareTag("CheckPoint5"))
        {
            col.gameObject.SetActive(false);
            Target.SetActive(true);
            target = Target.transform;
            m_DirectionIndicator.targetToLookAt = target;
            AddReward(0.4f);
        }
        else if (col.gameObject.CompareTag("Target"))
        {
            AddReward(1.0f);
            EndEpisode();
        }
    }
    void UpdateOrientationObjects()
    {
        m_WorldDirToWalk = target.position - drone.position;
        m_OrientationCube.UpdateOrientation(drone, target);

        if (m_DirectionIndicator)
        {
            m_DirectionIndicator.MatchOrientation(m_OrientationCube.transform);
        }
    }
    void FixedUpdate()
    {
        UpdateOrientationObjects();

        var cubeForward = m_OrientationCube.transform.forward;

        // Rotation alignment with checkpoint direction.
        // This reward will approach 1 if it faces the target direction perfectly and approach zero as it deviates
        var lookAtTargetReward = (Vector3.Dot(cubeForward, drone.forward) + 1) * .5F;

        // Testing reward value
        Debug.Log(lookAtTargetReward);

        //Check for NaNs
        if (float.IsNaN(lookAtTargetReward))
        {
            throw new ArgumentException(
                "NaN in lookAtTargetReward.\n" +
                $" cubeForward: {cubeForward}\n" +
                $" head.forward: {drone.forward}"
            );
        }

        // Positive reward if the drone faces the target direction perfectly and approach zero as it deviates
        AddReward(lookAtTargetReward);  
    }
}
