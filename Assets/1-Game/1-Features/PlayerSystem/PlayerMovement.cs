using System;
using FishNet.Object.Synchronizing;
using Game.InputSystem;
using Game.PlayerSystem;
using Game.Utils;
using UnityEngine;

namespace _Game.PlayerSystem
{
    public class PlayerMovement : BaseNetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkingSpeed = 7.5f;
        [SerializeField] private float runningSpeed = 11.5f;
        [SerializeField] private float jumpHeight = 8.0f;
        [SerializeField] private float gravity = 20.0f;
        public Camera playerCamera;

        [Header("Grounding")]
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private Transform groundedCheck;
        [SerializeField] private float groundedDistance = 2;
        
        [Header("Camera")]
        [SerializeField] private Transform cameraHolder;
        [SerializeField] private float lookSpeed = 2.0f;
        [SerializeField] private float lookXLimit = 45.0f;
        float rotationX = 0;
        Vector2 inputDir;
        
        public CharacterController characterController { get; private set; }

        Vector3 velocity;
        public bool isGrounded;


        protected override void RegisterEvents()
        {
            characterController = GetComponent<CharacterController>();
        }

        protected override void UnregisterEvents()
        {
            
        }
        
        

        void FixedUpdate()
        {

            CheckIfGrounded();
            HandleMovement(); 
        }

        private void LateUpdate()
        {
            if (!IsOwner) return;
            HandleCamera();
        }

        private void HandleMovement()
        {
            if (isGrounded && velocity.y < 0)
                velocity.y = 0f;
            
            bool isRunning = InputManager.IsSprinting;

            Vector3 moveDir = new Vector3(InputManager.MoveDir.x, 0, InputManager.MoveDir.y).normalized;
            Vector3 fixedMoveDir;
            

            moveDir *= (isRunning ? runningSpeed : walkingSpeed);
            moveDir = transform.TransformDirection(moveDir);

            if (Input.GetButtonDown("Jump") && isGrounded)
                velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * -gravity);

            velocity.y -= gravity * Time.deltaTime;

            fixedMoveDir = moveDir;
            fixedMoveDir.y = velocity.y;
            
            characterController.Move(fixedMoveDir * Time.deltaTime);
        }

        public void HandleCamera()
        {
            inputDir = InputManager.LookDir;
            rotationX += -inputDir.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            cameraHolder.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, inputDir.x * lookSpeed, 0);
        }

        public void CheckIfGrounded()
        {
            isGrounded = Physics.Raycast(groundedCheck.position, Vector3.down, groundedDistance, groundMask);
        }
    }
}