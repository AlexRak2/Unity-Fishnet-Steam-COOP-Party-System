using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.InputSystem
{
    public class InputManager: MonoBehaviour
    {
        public static InputManager Instance;

        public float MouseSensitivity = 1;

        private InputControl _inputControl;

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;
        private InputAction _sprintAction;

        private InputAction _aimAction;


        public static Vector2 MoveDir { get; private set; }
        public static Vector2 LookDir { get; private set; }

        public static bool Jumping;
        public static bool IsSprinting;
        public static bool IsAiming;

        public static event Action<bool> ProcessOnJump;


        private void Awake()
        {
            _inputControl = new InputControl();

            Instance = this;
        }

        private void OnEnable()
        {
            _moveAction = _inputControl.Player.Move;
            _lookAction = _inputControl.Player.Look;
            _jumpAction = _inputControl.Player.Jump;
            _sprintAction = _inputControl.Player.Sprint;
            _aimAction = _inputControl.Player.Aim;

            _moveAction.Enable();
            _lookAction.Enable();
            _jumpAction.Enable();
            _sprintAction.Enable();
            _aimAction.Enable();
        }

        private void OnDisable()
        {
            _moveAction.Disable();
            _lookAction.Disable();
            _jumpAction.Disable();
            _sprintAction.Disable();
            _aimAction.Disable();
        }

        private void Update()
        {
            MoveDir = _moveAction.ReadValue<Vector2>();
            LookDir = _lookAction.ReadValue<Vector2>();

            IsSprinting = _sprintAction.IsPressed();
            IsAiming = _aimAction.IsPressed();

            Jumping = _jumpAction.WasPressedThisFrame();

            if (Jumping)
                OnJump();
        }

        private void OnJump()
        {
            ProcessOnJump?.Invoke(true);
        }
    }
}