using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Player : MonoBehaviour
{
    //proprty is written in PascalCase
    public static Player Instance { get; private set; }

    public event EventHandler<OnCounterSelectedEventArgs> OnCounterSelected;
    public class OnCounterSelectedEventArgs : EventArgs 
    {
        public ClearCounter selectedCounter;
    }

    //For Fields we name with camelCase
    [SerializeField] private float speed = 1f, rotationSpeed = 10f;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private LayerMask countersLayer;

    private bool isWalking;
    private Vector3 lastMovementDirection;
    private Vector3 lastMovement;
    private ClearCounter selectedCounter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        inputManager.OnInteractAction += InputManager_OnInteractAction;
    }

    private void InputManager_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter)
            selectedCounter.Interact();
    }

    private void Update()
    {
        HandleMovement();
        HandleIntractions();
    }

    //For functions we have PascalCase
    public bool IsWalking() 
    {
        return isWalking;
    }

    private void HandleMovement() 
    {
        //Moved vector logic to the InputManager Class
        Vector2 inputVector = inputManager.GetMovementVectorNormalized();

        //convert vector2 to vector3
        Vector3 movementDirection = new Vector3(inputVector.x, 0, inputVector.y);

        //Collision handling
        float moveDistance = speed * Time.deltaTime;
        float playerHeight = 2f;
        float playerRadius = 0.7f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movementDirection, moveDistance);

        //Handling diagonal collison detection
        if (!canMove)
        {
            //Try moving on X
            Vector3 moveOnX = new Vector3(movementDirection.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveOnX, moveDistance);
            if (canMove)
            {
                movementDirection = moveOnX;
            }
            else
            {
                //Try moving on z
                Vector3 moveOnZ = new Vector3(0, 0, movementDirection.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveOnZ, moveDistance);
                if (canMove)
                {
                    movementDirection = moveOnZ;
                }
            }
        }

        if (canMove)
        {
            transform.position += movementDirection * speed * Time.deltaTime;
        }


        //if the move direction isnt zero the player is walking
        isWalking = movementDirection != Vector3.zero;
        if (lastMovement != movementDirection)
            lastMovement = movementDirection;

        transform.forward = Vector3.Slerp(transform.forward, lastMovement, Time.deltaTime * rotationSpeed);
    }

    private void HandleIntractions()
    {
        Vector2 inputVector = inputManager.GetMovementVectorNormalized();
        Vector3 movementDirection = new Vector3(inputVector.x, 0, inputVector.y);

        if (movementDirection != Vector3.zero) 
        {
            lastMovementDirection = movementDirection;
        }

        float interactDistance = 1f;
        if (Physics.Raycast(transform.position, lastMovementDirection, out RaycastHit raycastHit, interactDistance, countersLayer))
        {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                if (clearCounter != selectedCounter)
                    SetSelectedCounter(clearCounter);
                //Look at Counter
                transform.forward = Vector3.Slerp(transform.forward, lastMovementDirection, Time.deltaTime * rotationSpeed);
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else 
        {
            SetSelectedCounter(null);
        }
    }
    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnCounterSelected?.Invoke(this, new OnCounterSelectedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }
}
