using UnityEngine;

public class RobotSettings : MonoBehaviour
{
    public float agentRunSpeed;
    public float agentRotationSpeed;
    public Material goalScoredMaterial; // when a goal is scored the ground will use this material for a few seconds.
    public Material checkpointScoredMaterial; // when fail, the ground will use this material for a few seconds.
}
