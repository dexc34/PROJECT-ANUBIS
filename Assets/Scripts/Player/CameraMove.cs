using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private InputAction cameraMove;
    [SerializeField] private float cameraSensitivity = 5f;
    private GameObject playerBody;
    private float xRotation = 0f;

    private void Start() 
    {
        playerBody = GameObject.Find("Player");   

        //Hides and locks cursor to the centre of the screen
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cameraMove.Enable();
    }

    private void Update() 
    {
        Vector2 mouseValues = cameraMove.ReadValue<Vector2>();

        //Rotate player on the X axis
        playerBody.transform.Rotate(Vector3.up * mouseValues.x * cameraSensitivity * Time.deltaTime);

        //Rotate camera on the Y axis
        xRotation -= mouseValues.y * cameraSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    public void CameraLook(InputAction.CallbackContext context)
    {
        
    }
}
