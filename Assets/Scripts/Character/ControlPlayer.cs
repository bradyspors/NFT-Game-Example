using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPlayer : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private CharacterMovement _characterMovement;
    [SerializeField] private CharacterCollision _characterCollision;
 
    public GameObject weaponHolder;
    public static ControlPlayer Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float XMOVE = Input.GetAxis("Horizontal");
        float YMOVE = Input.GetAxis("Vertical");

        if (_characterMovement.currentState == CharacterMovement.PhysicsStates.grounded)
        {
            // Check for jumping
            if (Input.GetButtonDown("Jump"))
                _characterMovement.JumpUp();

            // check for crouching
            if (Input.GetButton("Crouch"))
            {
                _characterMovement.StartCrouching();
            }
            else // Stand up
            {
                bool checkRoof = _characterCollision.CheckRoof(transform.up);

                if (!checkRoof)
                    _characterMovement.StopCrouching();
            }

            // check for ground
            bool checkGround = _characterCollision.CheckFloor(-transform.up);
            if (!checkGround)
            {
                _characterMovement.InAir();
            }
        }
        else if (_characterMovement.currentState == CharacterMovement.PhysicsStates.inAir)
        {
            // check for ground
            bool checkGround = _characterCollision.CheckFloor(-transform.up);
            if (checkGround)
                _characterMovement.OnGround();
        }
    }

    private void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;

        float XMOVE = Input.GetAxis("Horizontal");
        float YMOVE = Input.GetAxis("Vertical");

        float camX = Input.GetAxis("Mouse X");
        float camY = Input.GetAxis("Mouse Y");

        _characterMovement.LookUpDown(camY, deltaTime);

        if (_characterMovement.currentState == CharacterMovement.PhysicsStates.grounded)
        {
            // increase grounded timer
            _characterMovement.CheckGroundedTimer(deltaTime);

            // get magnitutude of inputs
            float inputMagnitude = new Vector2(XMOVE, YMOVE).normalized.magnitude;

            // get which speed to apply to player (forwards or backwards)
            float targetSpeed = Mathf.Lerp(_characterMovement.backwardsMovementSpeed, _characterMovement.maxSpeed, YMOVE);

            _characterMovement.LerpSpeed(inputMagnitude, deltaTime, targetSpeed);

            _characterMovement.Move(XMOVE, YMOVE, _characterMovement.deceleration, 1);
            _characterMovement.Turn(camX, _characterMovement.deceleration, _characterMovement.turnSpeed);
        }
        else if (_characterMovement.currentState == CharacterMovement.PhysicsStates.inAir)
        {
            // increase in air timer timer
            _characterMovement.CheckInAirTimer(deltaTime);

            // Move player in air with less control
            _characterMovement.Move(XMOVE, YMOVE, deltaTime, _characterMovement.inAirControl);

            _characterMovement.Turn(camX, deltaTime, _characterMovement.turnSpeedInAir);
        }
    }
}
