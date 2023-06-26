using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class WeaponViewmodelAnimations : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputAction move;
    [SerializeField] CameraShake recoilShake;
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
        recoilShake = player.GetComponent<CameraShake>();
        initialGunPosition = transform.localPosition;
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

        ReturnToCenter();
        BackToOriginalPosition();

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

    //Initilizes starting position of weapon and target position for recoil
    [HideInInspector] public Vector3 currentRotation, targetRotation, targetPosition, currentPosition, initialGunPosition;

    [Header("Recoil Values")]
    [SerializeField] float recoilX;
    [SerializeField] float recoilY;
    [SerializeField] float recoilZ;

    [SerializeField] float kickBackZ;

    //How snappy should the recoil be
    public float snap;
    //How fast should it return to its original value
    public float returnAmount;

    [Header("Shake Values")]
    [SerializeField] float magnitude = 2f;
    float shakeNoiseX;
    float shakeNoiseY;
    float shakeNoiseZ;
    Vector3 shakeValue;

    //sets values needed for recoil
    void ReturnToCenter()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * returnAmount);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, Time.fixedDeltaTime * snap);
    }

    //performs recoil plus kickback 
    public void Recoil()
    {
        targetPosition -= new Vector3(0, 0, kickBackZ);
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(recoilZ, recoilZ));
        RecoilShake();
        //CameraShaker.Instance.ShakeOnce(4f, 4f, .1f, .1f);
    }

    void RecoilShake()
    {
        //shakeNoiseX = ((Mathf.PerlinNoise(0, Time.time) * 2f - 1f) * magnitude);
        shakeNoiseX = Random.Range(-0.2f, 0.2f);
        shakeNoiseY = Random.Range(0.1f, 0.2f);
        //shakeNoiseZ = ((Mathf.PerlinNoise(Time.time * 1, 0.0f) - 0.5f) * 2f) * magnitude;

        shakeValue = new Vector3(shakeNoiseX, shakeNoiseY, shakeNoiseZ);
        shakeValue *= magnitude;

        recoilShake.ScreenShake(shakeValue);
    }

    //puts the gun back to its original position
    void BackToOriginalPosition()
    {
        targetPosition = Vector3.Lerp(targetPosition, initialGunPosition, Time.deltaTime * returnAmount);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * snap);
    }

    float smooth = 10f; //used by both BobOffset and Sway
    float smoothRot = 12f; //ised by both BobSway and TiltSway

    public void CompositePositionRotation()
    {
        //position
        gunPos = Vector3.Lerp(transform.localPosition, swayPos + bobPosition, Time.deltaTime * smooth); ;
        transform.localPosition = gunPos + currentPosition;

        //rotation
        gunRot = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
        transform.localRotation = gunRot * Quaternion.Euler(currentRotation);
    }

}
