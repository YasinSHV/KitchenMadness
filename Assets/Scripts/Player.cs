using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    //For Fields we start with camelCase
    [SerializeField] private float speed = 1f, rotationSpeed = 10f;
    [SerializeField] private InputManager inputManager;

    private bool isWalking;

    private void Update()
    {
        //Moved vector logic to the InputManager Class
        Vector2 inputVector = inputManager.GetMovementVectorNormalized();

        Vector3 movementDirection = new Vector3(inputVector.x, 0,inputVector.y);
        transform.position += movementDirection * speed * Time.deltaTime;

        //if the move direction isnt zero the player is walking
        isWalking = movementDirection != Vector3.zero;

        transform.forward = Vector3.Slerp(transform.forward, movementDirection, Time.deltaTime * rotationSpeed);
    }

    //For functions we have PascalCase
    public bool IsWalking() 
    {
        return isWalking;
    }
}
