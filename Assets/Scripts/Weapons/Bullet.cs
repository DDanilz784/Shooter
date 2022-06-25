using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTimeBullet;
    [SerializeField] private GameObject bulletHole;
    void Start()
    {
        Destroy(gameObject, lifeTimeBullet);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Instantiate(bulletHole, transform.position, Quaternion.identity);
        if (collision.gameObject.tag == "Target")
        {
            collision.gameObject.GetComponentInParent<Target>().OnShotTarget();
        }
        Destroy(gameObject);
    }
}
