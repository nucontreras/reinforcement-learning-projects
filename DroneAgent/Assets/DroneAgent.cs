using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class DroneAgent : Agent
{
    // Drone parameters
    Rigidbody ourDrone;
    public float upForce;
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


    public override void Initialize()
    {
        //currentTime = startingTime;
        //m_RobotSettings = FindObjectOfType<RobotSettings>();
        ourDrone = GetComponent<Rigidbody>();
        //m_GroundRenderer = ground.GetComponent<Renderer>();
        //m_GroundMaterial = m_GroundRenderer.material;
    }

    public override void OnEpisodeBegin()
    {
        this.ourDrone.angularVelocity = Vector3.zero;
        this.ourDrone.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        //this.ourDrone.transform.localRotation = Quaternion.Euler(0, 90, 0);

        //Checkpoint1.SetActive(true);
        //Checkpoint2.SetActive(true);
        //Checkpoint3.SetActive(true);
        //Checkpoint4.SetActive(true);

        //Target.localPosition = new Vector3(10f, 0.83f, -9);
        //currentTime = startingTime;

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

        //MoveAgent(actionBuffers.DiscreteActions);

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

        //currentTime -= 1 * Time.deltaTime;
        //txtCountdown.text = currentTime.ToString("F1");
        //if (currentTime <= 0)
        //{
        //    currentTime = 0;
        //    AddReward(-0.05f);
        //    EndEpisode();
        //}

        // Penalty given each step to encourage agent to finish task quickly.
        AddReward(-0.1f / MaxStep);
    }

    //public void MoveAgent(ActionSegment<int> act)
    //{
    //    var dirToGo = Vector3.zero;
    //    var rotateDir = Vector3.zero;

    //    var action = act[0];

    //    switch (action)
    //    {
    //        case 1:
    //            dirToGo = transform.forward * 1f;
    //            break;
    //        case 2:
    //            dirToGo = transform.forward * -1f;
    //            break;
    //        case 3:
    //            rotateDir = transform.up * 1f;
    //            break;
    //        case 4:
    //            rotateDir = transform.up * -1f;
    //            break;
    //        case 5:
    //            dirToGo = transform.right * -0.75f;
    //            break;
    //        case 6:
    //            dirToGo = transform.right * 0.75f;
    //            break;
    //        case 7:
    //            dirToGo = transform.right * -0.75f;
    //            break;
    //        case 8:
    //            dirToGo = transform.right * 0.75f;
    //            break;
    //    }
    //}

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

    // No ML Agents program

    //void Awake()
    //{
    //    ourDrone = GetComponent<Rigidbody>();
    //    //ourDrone.transform.localRotation = Quaternion.Euler(0, 180, 0);
    //}
    ////void Start()
    ////{

    ////}
    //private void FixedUpdate()
    //{
    //    MovementUpDown();
    //    MovementForward();
    //    Rotation();
    //    ClampingSpeedValues();
    //    Swerwe();

    //    ourDrone.AddRelativeForce(Vector3.up * upForce);
    //    ourDrone.rotation = Quaternion.Euler(new Vector3(tiltAmountForward, currentYRotation, tiltAmountSideways));
    //}

    public void MovementUpDown(ActionBuffers actionBuffers)
    {
        var actionZ = actionBuffers.ContinuousActions[0];  // horizontal
        var actionX = actionBuffers.ContinuousActions[1];  // vertical

        var action = actionBuffers.DiscreteActions[0];

        if ((Mathf.Abs(actionX) > 0.2f || Mathf.Abs(actionZ) > 0.2f))
        {
            if (action==1 || action==2)
            {
                Debug.Log("action==1 || action==2");
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
            Debug.Log("action==1");
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
}
