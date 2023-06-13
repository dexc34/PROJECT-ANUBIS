using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float cameraSensitivity = 5f;
    private GameObject playerBody;
    private float xRotation = 0f;

    private void Awake() 
    {
        playerBody = this.transform.parent.gameObject;   

        //Hides and locks cursor to the centre of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CameraLook(InputAction.CallbackContext context)
    {
        Vector2 mouseValues = context.ReadValue<Vector2>();

        //Rotate player on the X axis
        playerBody.transform.Rotate(Vector3.up * mouseValues.x * cameraSensitivity * Time.deltaTime);

        //Rotate camera on the Y axis
        xRotation -= mouseValues.y * cameraSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //Debug.Log(context.ReadValue<Vector2>());
    }
}
