using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public Animator weaponAnimator;
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
    [SerializeField] public TextMeshProUGUI ammoText;
    private bool _canFire = true;
    private bool _reloading = false;

    private bool canShoot = true;
    private bool canMove = true;


    void Awake() 
    {
        _input = new PlayerInputActions();
        ammoText.text = "Ammo: " + _currentAmmoCount;
    }

    private void Update()
    {
        if (!_reloading)
        {
            ammoText.text = "Ammo: " + _currentAmmoCount;
        }
        else
        {
            ammoText.text = "Ammo: Reloading...";
        }
    }

    public void toggleShooting()
    {
        Debug.Log("Shooting Switched");
        canShoot = !canShoot;
    }

    public void toggleMovement()
    {
        canMove = !canMove;
    }

    // OnEnable is called if the player is active when the scene loads (or is reloaded in GameManager.Restart())
    private void OnEnable() 
    {    
        rb = GetComponent<Rigidbody2D>();

        _input.Player.Enable();

        _input.Player.Fire.performed += Fire_performed;
        _input.Player.Reload.performed += Reload_performed;
        _currentAmmoCount = _ammoCount;
    }

    // Disable input events in OnDisable to avoid callbacks on destroyed objects
    void OnDisable()
    {
        _input.Player.Fire.performed -= Fire_performed;
        _input.Player.Reload.performed -= Reload_performed;
        _input.Player.Disable();
    }

    private void FixedUpdate()
    {
        if(canMove)
        {
            Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            rb.MovePosition(rb.position + movement * _speed * Time.fixedDeltaTime);

            if (movement.sqrMagnitude > 0.01f)
            {
            // Player is moving: set "Speed" to 1
            animator.SetFloat("Speed", 1f);
            }
            else
            {
            // Player is idle: set "Speed" to 0
            animator.SetFloat("Speed", 0f);
            }
        }
    }

    




    private void Fire_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Fire();
        weaponAnimator.SetTrigger("Shoot");
    }

    private void Reload_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        StartCoroutine(Reload());
    }

    private void Fire()
    {
        if (_currentAmmoCount > 0 && _canFire && canShoot)
        {
            _currentAmmoCount--;
            // Gets a bullet from the bullet pool
            GameObject bullet = PoolManager.Instance.RequestBullet();
            // Sets the position of the bullet to infront of player
            bullet.transform.position = _bulletSpawn.transform.position;
            // Sets rotation of bullet moving forward from player
            bullet.transform.rotation = this.gameObject.transform.GetChild(0).rotation;
        }

        if (_currentAmmoCount == 0 && _reloading == false && canShoot)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        //Debug.Log("Reloading....");
        _canFire = false;
        _reloading = true;
        yield return new WaitForSeconds(_reloadTime);
        _currentAmmoCount = _ammoCount;
        _canFire = true;
        _reloading = false;
        //Debug.Log("Ready to fire....");
    }

    public void increaseSpeed() {
        _speed += 1f;
        Debug.Log("Current Speed: " + _speed);
    }
    
    public void reduceSpeed() {
        _speed -= 2f;
        Debug.Log("Current Speed: " + _speed);
    }

    public void increaseReloadTime() {
        _reloadTime += 1f;
        Debug.Log("Current reload time: " + _reloadTime);
    }

    public void decreaseReloadTime() {
        _reloadTime -= 0.5f;
        Debug.Log("Current reload time: " + _reloadTime);
    }

    public void increaseMaxAmmoCount() {
        _ammoCount += 1;
        Debug.Log("Current max ammo count:" + _ammoCount);
    }

    public void decreaseMaxAmmoCount() {
        _ammoCount -= 2;
        Debug.Log("Current max ammo count:" + _ammoCount);
    }
}
