using UnityEngine;

public class Target : MonoBehaviour
{
    private bool isDowned;
    private Animator animator;
    [SerializeField] private AudioClip hitSFX;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    public void OnShotTarget()
    {
        if(!isDowned)
        {
            isDowned = true;
            audioSource.PlayOneShot(hitSFX, Random.Range(0.5f,1f));
            Invoke("TargetUp", 2f);
            animator.SetBool("isDowned", isDowned);
        }
    }

    private void TargetUp()
    {
        isDowned = false;
        animator.SetBool("isDowned", isDowned);
    }
}
