using UnityEngine;

namespace Unity.ArrowIndicator
{
    public class DirectionIndicator : MonoBehaviour
    {

        public bool updatedByAgent; //should this be updated by the agent? If not, it will use local settings
        public Transform transformToFollow; //ex: hips or body
        public Transform targetToLookAt; //target in the scene the indicator will point to
        public float heightOffset;
        //private float m_StartingYPos;

        //void OnEnable()
        //{
        //    //m_StartingYPos = transform.position.y;
        //}

        void Update()
        {
            transform.position = new Vector3(transformToFollow.position.x, transformToFollow.position.y + heightOffset,
                transformToFollow.position.z);
            Vector3 walkDir = targetToLookAt.position - transformToFollow.position;
            //Debug.Log("walkDir");
            //Debug.Log(walkDir);
            transform.rotation = Quaternion.LookRotation(walkDir);  
        }

        //Public method to allow an agent to directly update this component
        public void MatchOrientation(Transform t)
        {
            //Debug.Log("matchorientation");
            //transform.position = new Vector3(t.position.x, t.position.y, t.position.z);
            //transform.rotation = t.rotation;
        }
    }
}