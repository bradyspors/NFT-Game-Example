using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public enum PhysicsStates
    {
        grounded,
        inAir,
    }

    // Control and see which state character is in
    public PhysicsStates currentState;

    [Header("Physics")]
    public float maxSpeed;
    public float backwardsMovementSpeed; // for when move left, right, or backwards
    [Range(0, 1)]
    public float inAirControl; // how much control over movements in air

    private float _actualSpeed; // actual speed of character

    public float acceleration;
    public float deceleration;
    public float directionControl; // how fast can change direction of movement
    private float _inAirTimer;
    private float _groundedTimer;
    private float _adjustmentAmount; // for how much player can adjust to any movement

    [Header("Jumping")]
    public float jumpAmount;

    [Header("Turning")]
    public float turnSpeed;
    public float turnSpeedInAir;

    public float lookUpSpeed; // how fast look upwards

    private float _yTurn;
    private float _xTurn;
    public float maxLookAngle;
    public float minLookAngle;

    [Header("Sprinting")]
    [Range(1, 3)]
    public float sprintMultiplier;
    private float _actualSpeedMultiplier;
    public bool sprinting;

    [Header("Crouching")]
    public float crouchSpeedMultiplier;
    public float crouchHeight;
    private float _standingHeight;
    private bool _crouching;

    [Header("Component References")]
    [SerializeField] private Camera _head; // reference to head to move
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CapsuleCollider _capsuleCollider;
    [SerializeField] private CharacterCollision _playerCollision;

    private void Start()
    {
        _standingHeight = _capsuleCollider.height;

        _adjustmentAmount = 1; //reset any adjustment amounts
    }

    public void JumpUp()
    {
        Vector3 velocity = _rigidbody.velocity;
        velocity.y = 0;

        _rigidbody.velocity = velocity;

        _rigidbody.AddForce(transform.up * jumpAmount, ForceMode.Impulse);

        InAir();
    }

    public void LerpSpeed(float magnitude, float deltaTime, float speed)
    {
        // current speed timed by the magnitude of inputs
        float LaMT = (speed * _actualSpeedMultiplier) * magnitude;

        // if moving or stopping
        float targetAcceleration = acceleration;
        if (magnitude == 0)
            targetAcceleration = deceleration;

        // lerp actual speed
        _actualSpeed = Mathf.Lerp(_actualSpeed, LaMT, deltaTime * targetAcceleration);
    }

    public void Move(float horizontal, float vertical, float deltaTime, float control)
    {
        // find direction to move
        Vector3 moveDirection = (transform.forward * vertical) + (transform.right * horizontal);
        moveDirection = moveDirection.normalized;

        // if not pressing an input, continue in direction of velocity
        if (horizontal == 0 && vertical == 0)
            moveDirection = _rigidbody.velocity.normalized;

        // multiply direction by speed
        moveDirection *= _actualSpeed;
        moveDirection.y = _rigidbody.velocity.y;

        // apply acceleration
        float acceleration = (directionControl * _adjustmentAmount) * control; // how much control over movement
        Vector3 lerpVelocity = Vector3.Lerp(_rigidbody.velocity, moveDirection, acceleration * deltaTime);
        _rigidbody.velocity = lerpVelocity;
    }

    public void Turn(float XAmount, float direction, float speed)
    {
        _yTurn += (XAmount * direction) * speed;

        transform.rotation = Quaternion.Euler(0, _yTurn, 0);
    }

    public void LookUpDown(float YAmount, float deltaTime)
    {
        _xTurn -= (YAmount * deltaTime) * lookUpSpeed;
        _xTurn = Mathf.Clamp(_xTurn, minLookAngle, maxLookAngle);

        _head.transform.localRotation = Quaternion.Euler(_xTurn, 0, 0);
    }

    public void InAir()
    {
        if (_crouching)
            StopCrouching();

        _inAirTimer = 0f;
        currentState = PhysicsStates.inAir;
    }

    public void CheckInAirTimer(float deltaTime)
    {
        if (_inAirTimer < 10)
            _inAirTimer += deltaTime;
    }

    public void OnGround()
    {
        if (_inAirTimer <= 0.2f)
            return;

        _groundedTimer = 0;
        currentState = PhysicsStates.grounded;
    }

    public void CheckGroundedTimer(float deltaTime)
    {
        if (_groundedTimer < 10)
            _groundedTimer += deltaTime;
    }

    public void StartSprinting(float xInput, float yInput)
    {
        if (xInput == 0 && yInput == 0)
            return;

        sprinting = true;
        _actualSpeedMultiplier = sprintMultiplier;
    }

    public void StopSprinting()
    {
        sprinting = false;
        _actualSpeedMultiplier = 1;
    }

    public void StartCrouching()
    {
        StopSprinting();

        _capsuleCollider.height = crouchHeight;
        _crouching = true;
        _actualSpeedMultiplier = crouchSpeedMultiplier;
    }

    public void StopCrouching()
    {
        _crouching = false;
        _capsuleCollider.height = _standingHeight;
        _actualSpeedMultiplier = 1;
    }
}
