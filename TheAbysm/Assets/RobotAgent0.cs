using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RobotAgent0 : Agent
{
    Rigidbody rBody;
    public GameObject ground;
    public GameObject CheckPoint1;
    public GameObject CheckPoint2;
    public GameObject CheckPoint3;
    public Transform Target;

    Material m_GroundMaterial;
    RobotSettings m_RobotSettings;
    Renderer m_GroundRenderer;
    private readonly bool useVectorObs = true;
    //private readonly float forceMultiplier = 12;
    EnvironmentParameters m_ResetParams;


    public override void Initialize()
    {
        m_RobotSettings = FindObjectOfType<RobotSettings>();
        rBody = GetComponent<Rigidbody>();
        m_GroundRenderer = ground.GetComponent<Renderer>();
        m_GroundMaterial = m_GroundRenderer.material;
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        SetResetParameters();
    }

    public override void OnEpisodeBegin()
    {
        // If the Agent fell, zero its momentum
        //if (this.transform.localPosition.y < -0.2)  // add if when I will to change the scene
        //{
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0, 0, 0);
        this.rBody.transform.localRotation = Quaternion.Euler(0, 90, 0);

        CheckPoint1.SetActive(true);
        CheckPoint2.SetActive(true);
        CheckPoint3.SetActive(true);

        SetResetParameters();

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVectorObs)
        {
            sensor.AddObservation(StepCount / (float)MaxStep);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        MoveAgent(actionBuffers.DiscreteActions);

        // Fell off platform
        if (this.transform.localPosition.y < -0.5f)
        {
            AddReward(-0.01f);
            EndEpisode();
        }
        AddReward(-1f / MaxStep);
    }
    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
            case 5:
                dirToGo = transform.right * -0.75f;
                break;
            case 6:
                dirToGo = transform.right * 0.75f;
                break;
        }
        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        rBody.AddForce(dirToGo * m_RobotSettings.agentRunSpeed, ForceMode.VelocityChange);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        // Reached target
        if (col.gameObject.CompareTag("Target"))
        {
            AddReward(1f);  // test with add and set
            StartCoroutine(GoalScoredSwapGroundMaterial(m_RobotSettings.goalScoredMaterial, 0.5f));
            EndEpisode();
        }
        else if (col.gameObject.CompareTag("DestroyCol1"))
        {
            col.gameObject.SetActive(false);
            AddReward(0.1f);
        }
        else if (col.gameObject.CompareTag("DestroyCol2"))
        {
            col.gameObject.SetActive(false);
            AddReward(0.2f);
        }
        else if (col.gameObject.CompareTag("DestroyCol3"))
        {
            col.gameObject.SetActive(false);
            AddReward(0.3f);
        }
    }
    IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
    {
        m_GroundRenderer.material = mat;
        yield return new WaitForSeconds(time);
        m_GroundRenderer.material = m_GroundMaterial;
    }

    public void SetGroundMaterialFriction()
    {
        var groundCollider = ground.GetComponent<Collider>();

        groundCollider.material.dynamicFriction = m_ResetParams.GetWithDefault("dynamic_friction", 0);
        groundCollider.material.staticFriction = m_ResetParams.GetWithDefault("static_friction", 0);
    }

    void SetResetParameters()
    {
        SetGroundMaterialFriction();
    }

}

