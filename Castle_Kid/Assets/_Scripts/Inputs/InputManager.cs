using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Inputs
{
    public class InputManager : MonoBehaviour
    {

        public static PlayerInput PlayerInput; // static ~= that creates only one variable, because it is an unique object that is not changing 
        // (when other script use that variable it will refer to that one and not copy it to another)

        public static Vector2 Movement;
        public static bool JumpWasPressed;
        public static bool JumpIsHeld;
        public static bool JumpWasReleased;
        public static bool RunIsHeld;
        public static bool DashWasPressed;
        
        public static bool PauseMenuWasPressed;
        
        public static bool InteractWasPressed;
        
        public static bool AttackWasPressed;
        public static bool AttackControllerWasPressed;

        public static bool PowerUp1WasPressed;
        public static bool PowerUp2WasPressed;
        public static bool PowerUp3WasPressed;
        
        public static bool PowerUp1ControllerWasPressed;
        public static bool PowerUp2ControllerWasPressed;
        public static bool PowerUp3ControllerWasPressed;
        
        public static Vector2 AimController;

        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _runAction;
        private InputAction _dashAction;
        
        private InputAction _pauseMenuAction;
        
        private InputAction _interactAction;
        
        private InputAction _attackAction;
        private InputAction _attackControllerAction;

        private InputAction _power1Action;
        private InputAction _power2Action;
        private InputAction _power3Action;
        
        private InputAction _power1ControllerAction;
        private InputAction _power2ControllerAction;
        private InputAction _power3ControllerAction;
        
        private InputAction _aimControllerAction;
        
        void Awake()
        {
            PlayerInput = GetComponent<PlayerInput>();

            _moveAction = PlayerInput.actions["Move"];
            _jumpAction = PlayerInput.actions["Jump"];
            _runAction = PlayerInput.actions["Run"];
            _dashAction = PlayerInput.actions["Dash"];
            
            _pauseMenuAction = PlayerInput.actions["PauseMenu"];
            
            _interactAction = PlayerInput.actions["Interact"];
            
            _attackAction = PlayerInput.actions["Attack"];
            _attackControllerAction = PlayerInput.actions["AttackController"];

            _power1Action = PlayerInput.actions["PowerUp1"];
            _power2Action = PlayerInput.actions["PowerUp2"];
            _power3Action = PlayerInput.actions["PowerUp3"];

            _power1ControllerAction = PlayerInput.actions["PowerUp1Controller"];
            _power2ControllerAction = PlayerInput.actions["PowerUp2Controller"];
            _power3ControllerAction = PlayerInput.actions["PowerUp3Controller"];

            _aimControllerAction = PlayerInput.actions["AimController"];
        }

        void Update()
        {
            PauseMenuWasPressed = _pauseMenuAction.WasPressedThisFrame();

            if (!PauseMenu.isPaused)
            {
                Movement = _moveAction.ReadValue<Vector2>();

                JumpWasPressed = _jumpAction.WasPressedThisFrame();
                JumpIsHeld = _jumpAction.IsPressed();
                JumpWasReleased = _jumpAction.WasReleasedThisFrame();

                RunIsHeld = _runAction.IsPressed();

                DashWasPressed = _dashAction.WasPressedThisFrame();
                
                InteractWasPressed = _interactAction.WasPressedThisFrame();
                
                AttackWasPressed = _attackAction.WasPressedThisFrame();
                AttackControllerWasPressed = _attackControllerAction.WasPressedThisFrame();

                PowerUp1WasPressed = _power1Action.WasPressedThisFrame();
                PowerUp2WasPressed = _power2Action.WasPressedThisFrame();
                PowerUp3WasPressed = _power3Action.WasPressedThisFrame();

                PowerUp1ControllerWasPressed = _power1ControllerAction.WasPressedThisFrame();
                PowerUp2ControllerWasPressed = _power2ControllerAction.WasPressedThisFrame();
                PowerUp3ControllerWasPressed = _power3ControllerAction.WasPressedThisFrame();

                AimController = _aimControllerAction.ReadValue<Vector2>();
            }
        }
    }
}
