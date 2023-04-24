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


    // Canvas

    float currentTime = 0f;
    float startingTime = 20f;
    public TextMeshProUGUI txtCountdown;

    // Target Orientation

    public Transform actual_target;
    public Transform frame;  // the frame of the drone
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
        //m_OrientationCube = GetComponentInChildren<OrientationCubeController>();
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

        // Orientation target
        //actual_target = CheckPoint1.transform;

        //Target.localPosition = new Vector3(10f, 0.83f, -9);
        currentTime = startingTime;

    }

    //public override void CollectObservations(VectorSensor sensor)
    //{
    //    if (useVectorObs)
    //    {
    //        sensor.AddObservation(StepCount / (float)MaxStep);
    //    }
    //}

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


        // Fell off platform
        //if (this.transform.localPosition.y < -0.5f)
        //{
        //    //Debug.Log("<0");
        //    //Debug.Log(Vector3.Distance(this.transform.localPosition, Target.localPosition));
        //    AddReward(-0.1f);
        //    EndEpisode();
        //}

        currentTime -= 1 * Time.deltaTime;
        txtCountdown.text = currentTime.ToString("F1");
        if (currentTime <= 0)
        {
            currentTime = 0;
            AddReward(-0.05f);
            EndEpisode();
        }

        // Penalty given each step to encourage agent to finish task quickly.
        AddReward(-0.1f / MaxStep);
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
            AddReward(0.1f);
        }
        else if (col.gameObject.CompareTag("CheckPoint2"))
        {
            col.gameObject.SetActive(false);
            CheckPoint3.SetActive(true);
            AddReward(0.2f);
        }
        else if (col.gameObject.CompareTag("CheckPoint3"))
        {
            col.gameObject.SetActive(false);
            CheckPoint4.SetActive(true);
            AddReward(0.3f);
        }
        else if (col.gameObject.CompareTag("CheckPoint4"))
        {
            col.gameObject.SetActive(false);
            CheckPoint5.SetActive(true);
            AddReward(0.4f);
        }
        else if (col.gameObject.CompareTag("CheckPoint5"))
        {
            col.gameObject.SetActive(false);
            Target.SetActive(true);
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
        //m_OrientationCube.UpdateOrientation(frame, actual_target);
        
        //m_WorldDirToWalk = target.position - hips.position;
        //m_OrientationCube.UpdateOrientation(hips, target);
        if (m_DirectionIndicator)
        {
            Debug.Log("Cha");
            m_DirectionIndicator.MatchOrientation(CheckPoint1.transform);
        }
    }
    void FixedUpdate()
    {
        UpdateOrientationObjects();
    }
}
