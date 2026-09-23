using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles inputs calculated by a client
/// </summary>
public class ClientInputHandler : InputHandler
{
    [SerializeField] private LayerMask groundMask;

    InputAction Move;
    InputAction Jump;
    InputAction Sprint;
    InputAction Look;

    private void Awake()
    {
        Move = InputSystem.actions.FindAction("Move");
        Move.Enable();

        Jump = InputSystem.actions.FindAction("Jump");
        Jump.Enable();
        Jump.performed -= OnJump;
        Jump.performed += OnJump;

        Look = InputSystem.actions.FindAction("Look");
        // Look.Enable();
    }

    private void Update()
    {
        // Store accumulated inputs for sending to the server, as a client
        if (IsOwner)
        {
            accumulatedInput.LookDelta += Look.ReadValue<Vector2>();
            accumulatedInput.Direction += Move.ReadValue<Vector2>();
        }

        // As a server, run a network tick every frame and pass in the last accumulated input received by the client
        if (IsServer)
        {
            foreach (IReceiveInput subscriber in _subscribers)
            {
                subscriber.NetworkTick(accumulatedInput);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        // Client prediction for instant movement
        foreach (IReceiveInput subscriber in _subscribers)
        {
            subscriber.NetworkPredictionTick(accumulatedInput);
        }
        
        // Send the accumulatedInput to the server and begin accumulating again
        SendInputServerRpc(accumulatedInput);

        accumulatedInput.ResetInputs();
    }

    // Receive Jump input, and check if it can be used
    public void OnJump(InputAction.CallbackContext callbackContext)
    {
        if (!IsOwner) return;
        bool b_IsGrounded = Physics.SphereCast(gameObject.transform.position + new Vector3(0f, .3f, 0f), .1f, Vector3.down, out RaycastHit hitInfo, .3f, groundMask);
        Debug.DrawLine(gameObject.transform.position + new Vector3(0f, .3f, 0f), gameObject.transform.position + new Vector3(0f, -.1f, 0f), Color.red, 50);

        if (!accumulatedInput.Jump && b_IsGrounded) 
        {
            accumulatedInput.Jump = true;
        }
    }
}
