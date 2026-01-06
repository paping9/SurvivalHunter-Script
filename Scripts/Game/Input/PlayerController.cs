using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class PlayerController : IDisposable
    {
        private BaseUnit _unit;
        private InputActions _inputActions = null;
        
        public PlayerController(InputActions inputActions)
        {
            _inputActions = inputActions;
            _inputActions.Enable();
            
            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled  += OnMoveCanceled;
        }

        public void Dispose()
        {
            _inputActions.Disable();
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled  -= OnMoveCanceled;

            _inputActions = null;
        }
        
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            Debug.Log($"Move: {moveInput}");
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            Debug.Log("Move Stopped");
        }
        
        
        public void Possess(BaseUnit unit)
        {
            if (_unit != null)
            {
                UnPossess();
            }

            _unit = unit;
        }

        public void UnPossess()
        {
            if (_unit == null) return;
        }

        public void Move(Vector2 input)
        {
            
        }

        public void PressSkillButton(int index)
        {
            
        }

        public void PressSkillRollButton()
        {
            
        }
    }
}