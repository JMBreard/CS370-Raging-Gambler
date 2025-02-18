using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;

    private void OnEnable()
    {
        Invoke("Hide", 1f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        Debug.Log("Hit: " + other.name);

        IDamagable hit = other.GetComponent<IDamagable>();

        if (!other.gameObject.CompareTag("Player") && hit != null)
        {
            hit.Damage();
            Hide();
        }
    }
}
