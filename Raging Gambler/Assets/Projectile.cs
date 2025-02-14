using UnityEngine;

public class ProjectileProjectile : MonoBehaviour
{
    [SerializeField]
    private GameObject ProjectilePrefabs;
    private float speed = 5.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Make projectile appear if lmb presed
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Instantiate(ProjectilePrefabs, transform.position, Quaternion.identity);
        }

        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    //private void FixedUpdate()
    //{

    //    //if (Input.GetAxisRaw("Horizontal") == -1)
    //    //{
    //    //    // Shoot left
    //    //    transform.Rotate(Vector3.forward * speed * Time.deltaTime);
    //    //}
    //    //else if (Input.GetAxisRaw("Horizontal") == 1)
    //    //{
    //    //    // Shoot right
    //    //    transform.Rotate(Vector3.forward * speed * Time.deltaTime);
    //    //}
    //    //if (Input.GetAxisRaw("Vertical") == -1)
    //    //{
    //    //    // Shoot down
    //    //}
    //    //else if (Input.GetAxisRaw("Vertical") == 1)
    //    //{
    //    //    // Shoot up
    //    //}

    //}

}
