using System;
using UnityEngine;

namespace Game
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] private Vector3 _boxSize;
        [SerializeField] private float _maxDistance;
        [SerializeField] private LayerMask _groundLayer;
        
        public bool IsGround()
        {
            return Physics.BoxCast(transform.position, _boxSize, -transform.up, transform.rotation, _maxDistance, _groundLayer);
        }
        
        // 절벽 끝인지..
        public bool IsLEdge()
        {
            return false;
        }
        
        // 벽에 충돌 했는지..
        public bool IsContactWall()
        {
            return false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(transform.position - transform.up * _maxDistance, _boxSize);
        }
#endif
    }
}
