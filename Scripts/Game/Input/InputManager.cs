using Game;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Game
{
    public class InputManager : SingletonMB<InputManager>
    {
        [SerializeField] private PlayerInput _playerInput;
    
        public PlayerController PlayerController { get; private set; }

        private InputActions _inputActions = null;
        
        private void Start()
        {
            PlayerController = new PlayerController(new InputActions());
            // _inputActions = new InputActions();
            // _inputActions.Enable();
            //
            // _inputActions.Player.Move.performed += OnMovePerformed;
            // _inputActions.Player.Move.canceled  += OnMoveCanceled;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            // _inputActions.Disable();
            //
            // _inputActions.Player.Move.performed -= OnMovePerformed;
            // _inputActions.Player.Move.canceled  -= OnMoveCanceled;
        }

        private void OnEnable()
        {
            UnityEngine.InputSystem.InputSystem.onDeviceChange += OnDeviceChange;
        }
    
        private void OnDisable()
        {
            UnityEngine.InputSystem.InputSystem.onDeviceChange -= OnDeviceChange;
        }
        
        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
            {
                SwitchToAppropriateInput();
            }
        }
        
        private void SwitchToAppropriateInput()
        {
            bool hasGamepad = Gamepad.all.Count > 0;
            bool hasKeyboard = Keyboard.current != null;
    
            if (hasGamepad)
            {
                Debug.Log("Switching to Gamepad Input");
                _playerInput.actions.FindActionMap("Player").Enable();
                
            }
            else if (hasKeyboard)
            {
                Debug.Log("Switching to Keyboard Input");
                _playerInput.actions.FindActionMap("Player").Enable();
                //virtualPad.enabled = false;
            }
            else
            {
                Debug.Log("Switching to Virtual Pad Input");
                _playerInput.actions.FindActionMap("Player").Disable();
                //virtualPad.enabled = true;
            }
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
    }
}
