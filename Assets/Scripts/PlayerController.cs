using System;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Inspector

    [Header("Movement")]
    
    [Min(0)]
    [Tooltip("The maximum movement speed in Unity units / sec (uu/s).")]
    [SerializeField] private float movementSpeed = 8f;
    
    [Tooltip("How fast the movement in-/ decreses.")]
    [SerializeField] private float speedChangeRate = 10f;

    [Min(0)]
    [Tooltip("How fast the character rotates around its y-axis")]
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Camera")]

    [Tooltip("The focus and rotation of the camera")]
    [SerializeField] private Transform cameraTarget;

    [Range(-89f, 0f)]
    [Tooltip("The minimum vertical camera angke. Lower half of the horizon")]
    [SerializeField] private float verticalCameraRotationMin = -30f;

    [Range(0f, 89f)]
    [Tooltip("The maximum vertical camera angle. Upper half of the horizon.")]
    [SerializeField] private float verticalCameraRotationMax = 70f;

    [Min(0)]
    [Tooltip("Sensitivity of the horizontal camera rotation. deg/s for controller.")]
    [SerializeField] private float cameraHorizontalSpeed = 200f;

    [Min(0)]
    [Tooltip("Sensitivity of the vertical camera rotation. deg/s for controller.")]
    [SerializeField] private float cameraVerticalSpeed = 130f;

    [Header("Mouse Settings")]

    // TODO Put in PlayerPrefs and put in settings.
    [Range(0f, 2f)]
    [Tooltip("Additional mouse rotation speed multiplier.")]
    [SerializeField] private float mouseCameraSensitivity = 1f;

    [Header("Controller Settings")]
    
    // TODO Put in PlayerPrefs and put in settings.
    [Range(0f, 2f)]
    [Tooltip("Additional controller rotation speed multiplier.")]
    [SerializeField] private float controllerCameraSensitivity = 1f;

    [Tooltip("Invert Y-axis for controller.")]
    [SerializeField] private bool invertY = true;
    
    #endregion

    private CharacterController characterController;
    
    private GameInput input;
    private InputAction moveAction;
    private InputAction lookAction;

    private Vector2 lookInput;
    private Vector2 moveInput;

    private Quaternion characterTargetRotation = Quaternion.identity;
    private Vector2 cameraRotation;
    private Vector3 lastMovement;

    #region Unity Event Funktions

    private void Awake()
    {

        characterController = GetComponent<CharacterController>();
        
        //Create new input
        input = new GameInput();
        lookAction = input.Player.Look;
        moveAction = input.Player.Move;

        // todo Subscribe to input events
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void Update()
    {
        ReadInput();

        Rotate(moveInput);
        Move(moveInput);
    }

    private void LateUpdate()
    {
        RotateCamera(lookInput);
    }

    private void OnDisable()
    {
        input.Disable(); 
    }

    #endregion

    private void OnDestroy()
    {
        //todo unsubscribe from input events
    }

    #region Input
    private void ReadInput()
    {
        lookInput = lookAction.ReadValue<Vector2>();
        moveInput = moveAction.ReadValue<Vector2>();
    }
    #endregion

    #region Movement

    private void Rotate(Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            Vector3 worldInputDirection = cameraTarget.TransformDirection(inputDirection);
            worldInputDirection.y = 0;

            characterTargetRotation = Quaternion.LookRotation(worldInputDirection);
        }
        if (Quaternion.Angle(transform.rotation, characterTargetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, characterTargetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = characterTargetRotation;
        }
    }

    private void Move(Vector2 moveInput)
    {
        float targetSpeed = moveInput == Vector2.zero ? 0f : movementSpeed * moveInput.magnitude;

        Vector3 currentVelocity = lastMovement;
        currentVelocity.y = 0f;
        float currentSpeed = currentVelocity.magnitude;

        // Increases the players speed based on their current speed
        if (Math.Abs(currentSpeed - targetSpeed) > 0.01f)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, speedChangeRate * Time.deltaTime);
        }
        else
        {
            currentSpeed = targetSpeed;
        }

        //Multiply the targetRotation Quaternion with Vector3.forward (not commutative!)
        // to get a direction vector in the direction of the targetRotation.
        // In a sense "vectorize the quaternion" 9loosing an axis of data: the roll).
        Vector3 targetDirection = characterTargetRotation * Vector3.forward;
        
        Vector3 movement = targetDirection * currentSpeed;

        characterController.SimpleMove(movement);

        lastMovement = movement;
    }

    #endregion

    #region Camera

    private void RotateCamera(Vector2 lookInput)
    {
        if (lookInput != Vector2.zero)
        {
            bool isMouseLook = IsMouseLook();

            // Variable zum multiplizieren mit deltaTime
            float deltaTimeMultiplier = isMouseLook ? 1f : Time.deltaTime;

            float sensitivity = isMouseLook ? mouseCameraSensitivity : controllerCameraSensitivity;

            lookInput *= deltaTimeMultiplier * sensitivity;

            // Multiply the input with the vertical camera speed in deg/s'
            // Vertical camera rotation around the X-axis of the player!
            //Additionally multiply with -1 if we are using the controller AND we want to invert the y-Input.
            cameraRotation.x += lookInput.y * cameraVerticalSpeed * (!isMouseLook && invertY ? -1 : 1);
            
            // Multiply the input with the vertical camera speed in deg/s'
            // Horizontal camera rotation around the Y-axis of the player!
            cameraRotation.y += lookInput.x * cameraVerticalSpeed;

            cameraRotation.x = NormalizeAngle(cameraRotation.x);
            cameraRotation.y = NormalizeAngle(cameraRotation.y);

            cameraRotation.x = Mathf.Clamp(cameraRotation.x, verticalCameraRotationMin, verticalCameraRotationMax);
        }
        
        //Important to always do even without input, so it is always steady and only moves if we give input
        // This prevents it from rotating with it's parent Player object.
        cameraTarget.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0f);
    }

    private float NormalizeAngle(float angle)
    {
        // Limit the angle to (-360, 360)
        angle %= 360;

        // Limits the agle to (0, 360)
        if (angle < 0)
        {
            angle += 360;
        }

        // Remaps the angle from (0,360) to (-180, 180)
        if (angle > 180)
        {
            angle -= 360;
        }

        return angle;
    }

    private bool IsMouseLook()
    {
        if (lookAction.activeControl == null)
        {
            return true;
        }

        return lookAction.activeControl.name == "delta";
    }

    #endregion
}
