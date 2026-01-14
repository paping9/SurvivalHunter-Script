using System;
using System.Collections.Generic;

using UnityEngine;

namespace Game
{
    public class UnitMove : IUnitComponent
    {
        #region Const Value
        private const float     DirectionMoveTime   = 0.5f;
        #endregion
    
        private BaseUnit                _owner;
        private Transform               _ownerTransform;
        private Vector3                 _nextPosition;
        private Queue<Vector3>          _positions;
        private Vector3                 _direction;
        private float                   _speed;
        private bool                    _move = false;
        private CharacterController     _characterController = null;
        private GroundChecker           _groundChecker = null;
        
        public void Init(BaseUnit unit)
        {
            _owner = unit;
            _ownerTransform = _owner.transform;
            
            if (_characterController == null) _characterController = _owner.GetComponent<CharacterController>();
            if (_groundChecker == null) _groundChecker = _owner.GetComponent<GroundChecker>();

        }

        public void Release()
        {
        }

        public void GameUpdate(float elapsedTime)
        {
            if (_owner == null || _ownerTransform == null) return;
            
            UpdateMove(elapsedTime);
        }

        public void FixedUpdate(float elapsedTime)
        {

        }

        private void UpdateMove(float elapsedTime)
        {
            if (_move == false) return;
            
            Move(elapsedTime);
        }

        private void Move(float elapsedTime)
        {
            var v = _speed * elapsedTime;

            var position = _ownerTransform.position;
            position += _direction * v;
            _ownerTransform.position = position;

            var dist = Vector3.Distance(_nextPosition, position);
            var dot = Vector3.Dot(_direction, position - _nextPosition);

            // 지나 치거나 가까워지면 멈춘다
            if (!(dist <= v) && !(dot < 0)) return;

            if (_positions.Count == 0)
            {
                Stop();
            }
            else
            {
                _nextPosition = _positions.Dequeue();
                _direction = _nextPosition - position;
            }
        }

        public void Warp(Vector3 position)
        {
            Stop();

            _owner.transform.position = position;
        }

        /// <summary>
        /// 해당 방향으로 정해진 시간동안만 이동
        /// </summary>
        /// <param name="direction"></param>
        public void SetDirection(Vector3 direction)
        {
            _positions.Clear();

            var v = _speed * DirectionMoveTime;
            _nextPosition = _owner.transform.position + direction * v;
            _direction = direction;

            _move = true;
        }

        /// <summary>
        /// 목적지만 설정해서 이동
        /// </summary>
        /// <param name="nextPosition"></param>
        public void SetMovePosition(Vector3 nextPosition)
        {
            _positions.Clear();

            _nextPosition = nextPosition;
            _direction = _nextPosition - _owner.transform.position;

            _move = true;
        }

        /// <summary>
        /// 경로를 따라 이동
        /// </summary>
        /// <param name="positions"></param>
        public void SetMovePositions(Queue<Vector3> positions)
        {
            if (positions.Count == 0) return;

            _positions = positions;
            _nextPosition = _positions.Dequeue();
            _direction = _nextPosition - _owner.transform.position;

            _move = true;
        }

        public void Stop()
        {
            _move = false;
            _positions.Clear();
        }
    }
}
