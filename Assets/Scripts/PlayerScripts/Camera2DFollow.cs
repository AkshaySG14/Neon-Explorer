using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        public float damping = 1;

        private float m_OffsetZ;
        private Vector3 m_CurrentVelocity;

        // Use this for initialization
        private void Start()
        {
            m_OffsetZ = (transform.position - target.position).z;
        }


        // Update is called once per frame
        private void Update()
        {
            // only update lookahead pos if accelerating or changed direction
            Vector3 aheadTargetPos = target.position + Vector3.forward*m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);
            transform.position = newPos;
        }
    }
}
