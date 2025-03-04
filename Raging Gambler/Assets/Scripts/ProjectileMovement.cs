using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    private float bulletTime = 1f;

    private void OnEnable()
    {
        Invoke("Hide", bulletTime);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _speed = 10f;
        bulletTime = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    private void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    // Interface allowing projectiles to interact with enemies
    public interface IDamagable
    {
        int Health { get; set; }
        void Damage();
    }

    // Will call Damage() and hide the projectile
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Hit: " + other.name);

        IDamagable hit = other.GetComponent<IDamagable>();

        if (!other.gameObject.CompareTag("Player") && hit != null)
        {
            hit.Damage();
            Hide();
        }

        else if (other.CompareTag("Obstacle")) {
            Hide(); 
        }
    }

    public void reduceBulletTime() {
        // bulletTime -= 0.25f;
        // _speed -= 2.5f;
        Debug.Log("Bullet time: " + _speed);
        Debug.Log("Bullet time: " + bulletTime);
    }

}
