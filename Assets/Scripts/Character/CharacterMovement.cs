using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Windows;

[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(NetworkRigidbody))]
public class CharacterMovement : NetworkBehaviour, IReceiveInput
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float snapThreshold = .5f; // Distance after which to snap back to the server authoritative position

    private bool recentlyJumped = false;

    [SerializeField] private LayerMask groundMask;


    private Rigidbody rb;
    private InputHandler inputHandler;
    ///<summary>Used to snap the player to the server's position if the player gets too far away</summary>
    private NetworkVariable<Vector3> serverPosition = new NetworkVariable<Vector3>();
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Subscribe to accepting input events
        inputHandler = GetComponent<InputHandler>();
        inputHandler.AddTicks(this);

    }
    private void FixedUpdate()
    {
        if (IsServer)
        {
            serverPosition.Value = transform.position;
        }
    }

    public override void OnNetworkSpawn()
    {
        // Prevents the player from snapping to the server every frame
        if (IsOwner && !IsServer) 
        {
            gameObject.GetComponent<NetworkTransform>().enabled = false;
            gameObject.GetComponent<NetworkRigidbody>().enabled = false;

            serverPosition.OnValueChanged += ReconcilePosition;
        }

    }


    // --- CLIENT PREDICTION ---
    public void NetworkPredictionTick(CharacterInput receivedInput)
    {
        if (!IsOwner || IsServer) return;

        ApplyPhysicsMovement(receivedInput);

        if (recentlyJumped && !CheckGrounded()) recentlyJumped = false;
    }

    // --- SERVER AUTHORITY ---
    public void NetworkTick(CharacterInput input)
    {
        if (!IsServer) return;

        ApplyPhysicsMovement(input);

        if (recentlyJumped && !CheckGrounded()) recentlyJumped = false;
    }


    private void ApplyPhysicsMovement(CharacterInput input)
    {
        Move(input);

        if (input.Jump && !recentlyJumped) 
        { 
            Jump();
            input.Jump = false;
            recentlyJumped = true;
        }
    }

    /// <summary>
    /// Applies simple WASD movement
    /// </summary>
    /// <param name="input">Input by which to calculate movement</param>
    private void Move(CharacterInput input)
    {
        // Apply basic movement physics
        Vector3 moveDirection = new Vector3(input.Direction.x, 0f, input.Direction.y).normalized;

        // Modifying the velocity makes snappy movement
        Vector3 targetVelocity = moveDirection * moveSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    /// <summary>
    /// Calculate jump physics if the player can jump
    /// </summary>
    private void Jump() 
    {
        Debug.Log("Player is jumping");
        if (CheckGrounded())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool CheckGrounded()
    {
        return Physics.SphereCast(gameObject.transform.position + new Vector3(0f, .3f, 0f), .1f, Vector3.down, out RaycastHit hitInfo, .3f, groundMask);
    }

    // Reconcile the client position if it is too far from the server's calculated position
    private void ReconcilePosition(Vector3 oldPos, Vector3 newPos)
    {
        float distance = Vector3.Distance(transform.position, newPos);
        if (distance > snapThreshold * 4)
        {
            rb.position = newPos;
            rb.linearVelocity = Vector3.zero;
            Debug.LogWarning("Major client desync! Snapping to last known position");
        }
        else if (distance > snapThreshold)
        {
            // Instead of setting position, we 'nudge' the Rigidbody
            // This stops the "slideshow" and makes it feel like a tiny bit of lag
            Vector3 nudge = (newPos - transform.position) * 0.1f;
            rb.position += nudge;
        }
    }
}
