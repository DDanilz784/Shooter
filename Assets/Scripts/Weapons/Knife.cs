using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float smooth;
    [SerializeField] private float multiplier;
    private Quaternion targetRotation;

    void Start()
    {
        
    }

    void Update()
    {
        Sway();
    }

    private void Sway()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * multiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * multiplier;

        // calculate target rotation
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        // rotate 
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}
