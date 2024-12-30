using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// The FreeFlyCamera class allows for free-flying camera movement in a Unity scene.
/// It handles movement and looking around using input from the player.
/// </summary>
public class FreeFlyCamera : MonoBehaviour
{
    /// <summary>
    /// Speed at which the camera moves horizontally.
    /// </summary>
    [SerializeField] private float moveSpeed = 10f;
    
    /// <summary>
    /// Speed at which the camera rotates.
    /// </summary>
    [SerializeField] private float lookSpeed = 0.5f;
    
    /// <summary>
    /// Speed at which the camera ascends or descends.
    /// </summary>
    [SerializeField] private float ascendSpeed = 5f;
    
    /// <summary>
    /// The camera to be controlled. Can be assigned in the Inspector.
    /// </summary>
    [SerializeField] private Camera flyCam; // Allow assigning a specific camera in the Inspector

    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private float _ascendInput;
    private float _pitch = 0f;
    private float _yaw = 0f;

    private PlayerInputActions _playerInputActions;

    
    /// <summary>
    /// Initializes the input actions and sets up the input callbacks.
    /// </summary>
    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();

        _playerInputActions.FlightControls.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _playerInputActions.FlightControls.Move.canceled += ctx => _moveInput = Vector2.zero;

        _playerInputActions.FlightControls.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _playerInputActions.FlightControls.Look.canceled += ctx => _lookInput = Vector2.zero;

        _playerInputActions.FlightControls.Ascend.performed += ctx => _ascendInput = 1f;
        _playerInputActions.FlightControls.Ascend.canceled += ctx => _ascendInput = 0f;

        _playerInputActions.FlightControls.Descend.performed += ctx => _ascendInput = -1f;
        _playerInputActions.FlightControls.Descend.canceled += ctx => _ascendInput = 0f;
    }

    
    /// <summary>
    /// Enables the input actions.
    /// </summary>
    private void OnEnable()
    {
        _playerInputActions.FlightControls.Enable();
    }

    
    /// <summary>
    /// Disables the input actions.
    /// </summary>
    private void OnDisable()
    {
        _playerInputActions.FlightControls.Disable();
    }

    
    /// <summary>
    /// Calls HandleMovement and HandleMouseLook every frame to update the camera's position and rotation.
    /// </summary>
    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    
    /// <summary>
    /// Handles the movement of the camera based on player input.
    /// </summary>
    private void HandleMovement()
    {
        Transform cameraTransform = Camera.main.transform;

        Vector3 moveDirection = (cameraTransform.right * _moveInput.x) + (cameraTransform.forward * _moveInput.y);
        transform.position += moveDirection * (moveSpeed * Time.deltaTime);
        transform.position += Vector3.up * (_ascendInput * ascendSpeed * Time.deltaTime);
    }
    
    
    /// <summary>
    /// Handles the rotation of the camera based on player input.
    /// </summary>
    private void HandleMouseLook()
    {
        _yaw += _lookInput.x * lookSpeed;
        _pitch -= _lookInput.y * lookSpeed;
        _pitch = Mathf.Clamp(_pitch, -90f, 90f);

        transform.rotation = Quaternion.Euler(0f, _yaw, 0f);

        if (flyCam)
        {
            flyCam.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }
    }

   
}
