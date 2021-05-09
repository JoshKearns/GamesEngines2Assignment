using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Rigidbody _rb;
    
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private const string MOUSE_X = "Mouse X";
    private const string MOUSE_Y = "Mouse Y";

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private LayerMask obstacleMask;

    private float horizontalInput;
    private float verticalInput;
    private float mouseInputX;
    private float mouseInputY;
    private float currentRotationY;
    private float currentRotationX;

    [SerializeField] private GameObject soundWater;
    [SerializeField] private GameObject soundAir;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        currentRotationY = transform.eulerAngles.y;
        currentRotationX = transform.eulerAngles.x;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        GetInput();
        HandleTranslation();
        HandleRotation();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        mouseInputX = Input.GetAxis(MOUSE_X);
        mouseInputY = Input.GetAxis(MOUSE_Y);
    }
    
    private void HandleTranslation()
    {
        var moveVector = new Vector3(horizontalInput, 0f, verticalInput);

        var MoveVelocity = Time.deltaTime * moveSpeed * transform.TransformDirection(moveVector);
        _rb.velocity = MoveVelocity;
    }
    
    private void HandleRotation()
    {
        float yaw = mouseInputX * Time.deltaTime * rotationSpeed;
        currentRotationY += yaw;

        float pitch = mouseInputY * Time.deltaTime * rotationSpeed;
        currentRotationX -= pitch;
        currentRotationX = Mathf.Clamp(currentRotationX, -90, 90);
        
        transform.localRotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Air"))
        {
            soundWater.SetActive(false);
            soundAir.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Air"))
        {
            soundWater.SetActive(true);
            soundAir.SetActive(false);
        }
    }
}
