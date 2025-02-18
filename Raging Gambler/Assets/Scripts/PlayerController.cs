using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;
    private Rigidbody2D rb;
    private Vector2 direction;

    public GameObject projectilePrefab;
    public Transform spawnPoint;

    private PlayerInputActions _input;

    [Header("Ammo")]
    [SerializeField] private GameObject _bulletSpawn;
    [SerializeField] private int _ammoCount = 10;
    private int _currentAmmoCount;
    [SerializeField] private float _reloadTime = 3.0f;
    private bool _canFire = true;
    private bool _reloading = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        _input = new PlayerInputActions();
        _input.Player.Enable();

        _input.Player.Fire.performed += Fire_performed;
        _input.Player.Reload.performed += Reload_performed;
        _currentAmmoCount = _ammoCount;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     // Defines player movement 
    //     //direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    //     // Movement();
    // }

    private void FixedUpdate()
    {
        Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        rb.MovePosition(rb.position + movement * _speed * Time.fixedDeltaTime);
        //rb.MovePosition(rb.position + (direction * speed * Time.deltaTime));
    }

    // private void Movement()
    // {
    //     var move = _input.Player.Movement.ReadValue<Vector2>();
    //     transform.Translate(move * Time.deltaTime * _speed);
    //     Debug.Log($"X: {move.x} Y: {move.y}");
    // }

    private void Fire_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Fire();
    }

    private void Reload_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        StartCoroutine(Reload());
    }

    private void Fire()
    {
        if (_currentAmmoCount > 0 && _canFire)
        {
            _currentAmmoCount--;
            // Gets a bullet from the bullet pool
            GameObject bullet = PoolManager.Instance.RequestBullet();
            // Sets the position of the bullet to infront of player
            bullet.transform.position = _bulletSpawn.transform.position;
            // Sets rotation of bullet moving forward from player
            bullet.transform.rotation = this.gameObject.transform.GetChild(0).rotation;
        }

        if (_currentAmmoCount == 0 && _reloading == false)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        Debug.Log("Reloading....");
        _canFire = false;
        _reloading = true;
        yield return new WaitForSeconds(_reloadTime);
        _currentAmmoCount = _ammoCount;
        _canFire = true;
        _reloading = false;
        Debug.Log("Ready to fire....");
    }
}
