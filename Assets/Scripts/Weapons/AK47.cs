using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK47 : MonoBehaviour
{
    [Header("Gun Ñharacteristics")]
    [SerializeField] private float shootInterval;
    [SerializeField, Range(1, 100)] private int maxDamage;
    [SerializeField, Range(1, 100)] private int minDamage;
    [SerializeField] private float spread;

    [Header("Sway Settings")]
    [SerializeField] private float smooth;
    [SerializeField] private float multiplier;
    private Quaternion targetRotation;

    [Header("For Audio")]
    [SerializeField] private AudioClip ReloadSFX;
    [SerializeField] private AudioClip ShotSFX;
    private AudioSource audioSource;

    [Header("Other")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float animationInspectTime;
    [SerializeField] private ParticleSystem muzzleFlash;
    private Animator animator;
    private bool readyForShoot;
    private bool onAim;
    private bool inspect;

    [Header("Aim Settings")]
    [SerializeField] private Transform gunObject;
    [SerializeField] private Vector3 aimPosition;
    private Vector3 targetPosition;



    void Start()
    {
        readyForShoot = true;
        audioSource = GetComponent<AudioSource>();
        targetRotation = transform.rotation;
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Sway();
        Weapon();
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

    private void Weapon()
    {
        if(Input.GetKey(KeyCode.Mouse0) && readyForShoot)
        {
            StartCoroutine(Attack());
        }

        if(Input.GetKey(KeyCode.Mouse1) && !inspect)
        {
            targetPosition = aimPosition;
            onAim = true;
        }
        else
        {
            targetPosition = new Vector3(0,0,0);
            onAim = false;
        }

        if (Input.GetKey(KeyCode.F) && !inspect)
        {
            inspect = true;
            animator.SetTrigger("Inspect");
            Invoke("InspectEnd", animationInspectTime);
        }
        gunObject.localPosition = Vector3.Lerp(gunObject.localPosition, targetPosition, 10 * Time.deltaTime);

    }

    private IEnumerator Attack()
    {
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        if(onAim)
        {
            x = x / 2.5f;
            y = y / 2.5f;
        }
        muzzleFlash.Play();
        InspectEnd();
        readyForShoot = false;
        audioSource.PlayOneShot(ShotSFX, Random.Range(0.5f, 0.7f));
        GameObject bulleClone = Instantiate(bulletPrefab, muzzle.position, Quaternion.identity);
        animator.SetTrigger("Shot");
        bulleClone.GetComponent<Rigidbody>().AddForce((-muzzle.right + new Vector3(x,y,0)) * 10000, ForceMode.Force);
        yield return new WaitForSeconds(shootInterval);
        readyForShoot = true;
    }

    private void InspectEnd()
    {
        inspect = false;
    }
}
