using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputAction move;
    private GameObject player;
    private CharacterController characterContoller;

    //for other scripts to use
    [HideInInspector] public Vector3 gunPos; //stores current position of gun for recoil script to use
    [HideInInspector] public Quaternion gunRot; //stores current rotation of gun for recoil script to use


    //Gets player controller 
    private void Start()
    {
        player = GameObject.Find("Player");
        characterContoller = player.GetComponent<CharacterController>();
    }

    /*[Header("Settings")]
    //Activate these for accessablity reasons (in case bobing or sway causes motion sickness for some players)
    public bool sway = true;
    */

    // Update is called once per frame
    void Update()
    {
        //get player input
        GetInput();

        //get each movement and rotation component
        Sway();
        SwayRotation();

        BobOffset();
        BobRotation();

        //applay all movement and rotation components
        CompositePositionRotation();

    }

    //Stores input "WASD" and Mouse position 
    Vector2 walkInput; //from keyboard
    Vector2 lookInput; //from mouse
    void GetInput()
    {
        walkInput = move.ReadValue<Vector2>();

        lookInput.x = Input.GetAxis("Mouse X");
        lookInput.y = Input.GetAxis("Mouse Y");
    }

    [Header("Sway")]
    public float step = 0.01f; //multipled by the value from the mouse for 1 frame
    public float maxStepDistance = 0.06f; //max distance from the local origin 
    Vector3 swayPos;//Stores our value for later 

    void Sway() //x,y,z position change as a result of movingg the mouse
    {
        Vector3 invertLook = lookInput * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    [Header("Sway Rotation")]
    public float rotationStep = 4f; //multipled by the value from the mouse for 1 frame 
    public float maxRotationStep = 5f; //max rotation from the local identity rotation 
    Vector3 swayEulerRot; //stores our value 

    void SwayRotation() //roll, pitch, yaw change as a result of moving the mouse 
    {
        Vector2 invertLook = lookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);

        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    [Header("Bobbing")]
    public float speedCurve; //used by both bobbing methods 
    float curveSin { get => Mathf.Sin(speedCurve); } //gets sin for the curve
    float curveCos { get => Mathf.Cos(speedCurve); } //gets cos for the curve

    public Vector3 travelLimit = Vector3.one * 0.025f; //the maximum limits of travel from move input
    public Vector3 bobLimit = Vector3.one * 0.01f; //limits of travel from bobbing over time

    Vector3 bobPosition;
    void BobOffset() //x,y,z position change as a result of walking
    {
        //used to generate the sin and cos waves
        speedCurve += Time.deltaTime * (characterContoller.isGrounded ? characterContoller.velocity.magnitude : 1f) + 0.01f;

        bobPosition.x = (curveCos * bobLimit.x * (characterContoller.isGrounded ? 1 : 0)) - (walkInput.x * travelLimit.x);
        bobPosition.y = (curveSin * bobLimit.y) - (characterContoller.velocity.y * travelLimit.y);
        bobPosition.z = -(walkInput.y * travelLimit.z);
    }

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    Vector3 bobEulerRotation;

    void BobRotation() //roll, pitch, yaw change as a result of walking
    {
        bobEulerRotation.x = (walkInput != Vector2.zero ? multiplier.x * (Mathf.Sin(2 * speedCurve)) : multiplier.x * (Mathf.Sin(2 * speedCurve) / 2)); //pitch
        bobEulerRotation.y = (walkInput != Vector2.zero ? multiplier.y * curveCos : 0); //yaw
        bobEulerRotation.z = (walkInput != Vector2.zero ? multiplier.z * curveCos * walkInput.x : 0); //roll
    }

    float smooth = 10f; //used by both BobOffset and Sway
    float smoothRot = 12f; //ised by both BobSway and TiltSway

    public void CompositePositionRotation()
    {
        //position
        gunPos = Vector3.Lerp(this.transform.position, swayPos + bobPosition, Time.deltaTime * smooth); ;
        this.transform.position = gunPos;

        //rotation
        gunRot = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
        this.transform.rotation = gunRot;
    }

}
