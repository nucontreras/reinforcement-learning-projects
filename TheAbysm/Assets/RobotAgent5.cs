using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RobotAgent5 : Agent
{
    float currentTime = 0f;
    float startingTime = 20f;
    public TextMeshProUGUI txtCountdown;

    Rigidbody rBody;
    public GameObject ground;
    public GameObject Checkpoint1;
    public GameObject Checkpoint2;
    public GameObject Checkpoint3;
    public GameObject Checkpoint4;
    public Transform Target;

    Material m_GroundMaterial;
    RobotSettings m_RobotSettings;
    Renderer m_GroundRenderer;
    private readonly bool useVectorObs = true;

    public override void Initialize()
    {
        currentTime = startingTime;
        m_RobotSettings = FindObjectOfType<RobotSettings>();
        rBody = GetComponent<Rigidbody>();
        m_GroundRenderer = ground.GetComponent<Renderer>();
        m_GroundMaterial = m_GroundRenderer.material;
    }

    public override void OnEpisodeBegin()
    {
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0, 0.2f, 0);
        this.rBody.transform.localRotation = Quaternion.Euler(0, 90, 0);

        Checkpoint1.SetActive(true);
        Checkpoint2.SetActive(true);
        Checkpoint3.SetActive(true);
        Checkpoint4.SetActive(true);

        //Target.localPosition = new Vector3(10f, 0.83f, -9);
        currentTime = startingTime;

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
            //Debug.Log("<0");
            AddReward(-0.1f);
            EndEpisode();
        }
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
        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        rBody.AddForce(dirToGo * m_RobotSettings.agentRunSpeed,
            ForceMode.VelocityChange);
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
            AddReward(0.8f);
            StartCoroutine(GoalScoredSwapGroundMaterial(m_RobotSettings.goalScoredMaterial, 0.4f));
            EndEpisode();
        }
        else if (col.gameObject.CompareTag("DestroyCol1"))
        {
            col.gameObject.SetActive(false);
            StartCoroutine(GoalScoredSwapGroundMaterial(m_RobotSettings.checkpointScoredMaterial, 0.2f));
            AddReward(0.1f);
        } else if (col.gameObject.CompareTag("DestroyCol2"))
        {
            col.gameObject.SetActive(false);
            StartCoroutine(GoalScoredSwapGroundMaterial(m_RobotSettings.checkpointScoredMaterial, 0.2f));
            AddReward(0.2f);
        } else if (col.gameObject.CompareTag("DestroyCol3"))
        {
            col.gameObject.SetActive(false);
            StartCoroutine(GoalScoredSwapGroundMaterial(m_RobotSettings.checkpointScoredMaterial, 0.2f));
            AddReward(0.3f);
        }
        else if (col.gameObject.CompareTag("DestroyCol4"))
        {
            col.gameObject.SetActive(false);
            StartCoroutine(GoalScoredSwapGroundMaterial(m_RobotSettings.checkpointScoredMaterial, 0.2f));
            AddReward(0.4f);
        }
    }
    IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
    {
        m_GroundRenderer.material = mat;
        yield return new WaitForSeconds(time);
        m_GroundRenderer.material = m_GroundMaterial;
    }
}

