using UnityEngine;
using System.Collections.Generic; 

public class PoolManager : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private GameObject _bulletContainer;
    [SerializeField] private List<GameObject> _bulletPool;
    [SerializeField] private int _bullets;

    private static PoolManager _instance;

    public static PoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("PoolManager is Null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    private List<GameObject> GenerateBullets(int numOfBullets)
    {
        for (int i = 0; i < numOfBullets; i++)
        {
            // Creates bullets based on bullet count and adds them to the bullet pool
            GameObject bullet = Instantiate(_bulletPrefab);
            bullet.transform.parent = _bulletContainer.transform;
            bullet.SetActive(false);
            _bulletPool.Add(bullet);
        }
        return _bulletPool;
    }

    public GameObject RequestBullet()
    {
        // Looks for inactive bullet in bullet pool and returns it
        foreach(var bullet in _bulletPool)
        {
            if (bullet.activeInHierarchy == false)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }

        GameObject newBullet = Instantiate(_bulletPrefab);
        newBullet.transform.parent = _bulletContainer.transform;
        _bulletPool.Add(newBullet);
        return newBullet;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _bulletPool = GenerateBullets(_bullets); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
