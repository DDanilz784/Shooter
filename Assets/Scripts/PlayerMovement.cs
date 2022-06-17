using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    [Header("For Movement")]
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float slopeForce = 5f;
    [SerializeField] private float slopeForceRayLength = 1.5f;
    private float moveSpeed;
    private Vector3 moveInput;
    private Vector3 velocity;
    private CharacterController characterController;
    [SerializeField] private bool isJumping;
    private float targetFov;

    [Header("For Look")]
    public float mouseSensitivity;
    float xRotation = 0f;
    [SerializeField] private Camera camera;

    [Header("For Footsteps")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepsSFX;
    private float footstepsSpeed;
    private bool onGroundLastFrame = false;
    private bool readyForNextFootstep = true;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Movement();
        MouseLook();
        LandSound();
    }
    
    private void Movement()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            footstepsSpeed = 0.25f;
            moveSpeed = Mathf.Lerp(moveSpeed, runSpeed, 5f * Time.deltaTime);
        }
        else
        {
            footstepsSpeed = 0.4f;
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, 5f * Time.deltaTime);
            targetFov = 80f;
        }
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFov, 2.5f * Time.deltaTime);

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        moveInput = transform.right * x + transform.forward * z;
        Vector3 direction = Vector3.ClampMagnitude(moveInput, moveSpeed);
        characterController.Move(direction * Time.deltaTime * moveSpeed);

        if (IsGrounded() && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
            isJumping = true;
        }
        else if(IsGrounded())
        {
            isJumping = false;
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        if(x != 0 || z != 0 )
        {
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                targetFov = 90f;
            }
            if (IsGrounded())
            {
                StartCoroutine(Footsteps());
            }
            if(OnSlope())
            {
                characterController.Move(Vector3.down * characterController.height / 2 * slopeForce * Time.deltaTime);
            }
        }
    }

    private void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, characterController.height / 2 + 0.2f);
    }

    private bool OnSlope()
    {
        if (isJumping)
            return false;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, characterController.height / 2 * slopeForceRayLength))
            if (hit.normal != Vector3.up)
                return true;
        return false;
    }

    private IEnumerator Footsteps()
    {
        if(readyForNextFootstep)
        {
            readyForNextFootstep = false;
            audioSource.PlayOneShot(footstepsSFX[Random.Range(0, footstepsSFX.Length)]);
            yield return new WaitForSeconds(footstepsSpeed);
            readyForNextFootstep = true;
        }
    }
    private void LandSound()
    {
        if (IsGrounded() && !onGroundLastFrame)
        {
            audioSource.PlayOneShot(footstepsSFX[Random.Range(0, footstepsSFX.Length)]);
        }
        onGroundLastFrame = IsGrounded();
    }
}