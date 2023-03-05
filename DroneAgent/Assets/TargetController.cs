using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using UnityEngine.Events;
using System;

/// <summary>
/// Utility class to allow target placement and collision detection with an agent
/// Add this script to the target you want the agent to touch.
/// Callbacks will be triggered any time the target is touched with a collider tagged as 'tagToDetect'
/// </summary>
public class TargetController : MonoBehaviour
{

    [Header("Collider Tag To Detect")]
    public string tagToDetect = "Player"; //collider tag to detect 

    // native array
    [Header("Target positions")]
    public Vector3[] targets = new Vector3[5];
    private int index = 0;

    //[Header("Target Placement")]
    //public float spawnRadius; //The radius in which a target can be randomly spawned.
    //public bool respawnIfTouched; //Should the target respawn to a different position when touched

    //[Header("Target Fell Protection")]
    //public bool respawnIfFallsOffPlatform = true; //If the target falls off the platform, reset the position.
    //public float fallDistance = 5; //distance below the starting height that will trigger a respawn 


    private Vector3 m_startingPos; //the starting position of the target
    private Agent m_agentTouching; //the agent currently touching the target

    [System.Serializable]
    public class TriggerEvent : UnityEvent<Collider>
    {
    }

    [Header("Trigger Callbacks")]
    public TriggerEvent onTriggerEnterEvent = new TriggerEvent();
    //public TriggerEvent onTriggerStayEvent = new TriggerEvent();
    //public TriggerEvent onTriggerExitEvent = new TriggerEvent();

    //[System.Serializable]
    //public class CollisionEvent : UnityEvent<Collision>
    //{
    //}

    //[Header("Collision Callbacks")]
    //public CollisionEvent onCollisionEnterEvent = new CollisionEvent();
    //public CollisionEvent onCollisionStayEvent = new CollisionEvent();
    //public CollisionEvent onCollisionExitEvent = new CollisionEvent();

    // Start is called before the first frame update

    private void Start()
    {
        targets[0] = new Vector3(0f, 18f, 35f);
        targets[1] = new Vector3(0f, 65f, 140f);
        targets[2] = new Vector3(150, 65f, 290f);
        targets[3] = new Vector3(420f, 50f, 190f);
        targets[4] = new Vector3(22f, 30f, -100f);
        
        transform.position = targets[index];
        //Vector3 V = array[0];
    }
    //void OnEnable()
    //{
    //    m_startingPos = transform.position;
    //    if (respawnIfTouched)
    //    {
    //        MoveTargetToAnotherPosition();
    //    }
    //}

    //void Update()
    //{
    //    if (respawnIfFallsOffPlatform)
    //    {
    //        if (transform.position.y < m_startingPos.y - fallDistance)
    //        {
    //            Debug.Log($"{transform.name} Fell Off Platform");
    //            MoveTargetToAnotherPosition();
    //        }
    //    }
    //}

    /// <summary>
    /// Moves target to a random position within specified radius.
    /// </summary>
    public void MoveTargetToAnotherPosition()
    {
        //var newTargetPos = m_startingPos + (Random.insideUnitSphere * spawnRadius);
        //newTargetPos.y = m_startingPos.y;
        index++;
        if (index > 4)
        {
            // addreward positive. it's finished.
        }
        else
        {
            transform.position = targets[index];
        }
    }

    //private void OnCollisionEnter(Collision col)
    //{
    //    if (col.transform.CompareTag(tagToDetect))
    //    {
    //        onCollisionEnterEvent.Invoke(col);
    //        if (respawnIfTouched)
    //        {
    //            MoveTargetToAnotherPosition();
    //        }
    //    }
    //}

    //private void OnCollisionStay(Collision col)
    //{
    //    if (col.transform.CompareTag(tagToDetect))
    //    {
    //        onCollisionStayEvent.Invoke(col);
    //    }
    //}

    //private void OnCollisionExit(Collision col)
    //{
    //    if (col.transform.CompareTag(tagToDetect))
    //    {
    //        onCollisionExitEvent.Invoke(col);
    //    }
    //}

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(tagToDetect))
        {
            Debug.Log("Collision");
            //onTriggerEnterEvent.Invoke(col);
            MoveTargetToAnotherPosition();
        }
    }

    //private void OnTriggerStay(Collider col)
    //{
    //    if (col.CompareTag(tagToDetect))
    //    {
    //        onTriggerStayEvent.Invoke(col);
    //    }
    //}

    //private void OnTriggerExit(Collider col)
    //{
    //    if (col.CompareTag(tagToDetect))
    //    {
    //        onTriggerExitEvent.Invoke(col);
    //    }
    //}
}

