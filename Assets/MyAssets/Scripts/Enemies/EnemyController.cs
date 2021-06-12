using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    private float _health;
    private float _maxHealth;
    private AudioManager _audioManager;
    private Canvas _lifeCanvas;

    private EnemiesManager _enemiesManager;

    private float _scoreByKill = 2f;
    private float _round=0;

    private GameObject _player;
    private PlayerController _playerController;
    private NavMeshAgent _enemyNav;
    private Animator _enemyAnim;
    private bool _isWalking=false;
    private float _fireRate=0.3f;
    private bool _destroyed = false;

    private GameObject _weapon;
    [SerializeField]
    private GameObject _bullet;
    private GameObject _bulletHolder;
    private float _bulletSpeed=3000;

    [SerializeField]
    private Image _life;

    // Start is called before the first frame update
    void Start()
    {
        
        _audioManager = GameObject.FindObjectOfType<AudioManager>();
        _enemyAnim = GetComponentInChildren<Animator>();
        _player = GameObject.Find("Player");
        _playerController = _player.GetComponent<PlayerController>();
        _enemyNav = transform.GetComponent<NavMeshAgent>();
        _weapon = transform.GetChild(0).gameObject;
        _bulletHolder = _weapon.transform.GetChild(0).gameObject;
        _enemiesManager = GameObject.FindObjectOfType<EnemiesManager>();
        _lifeCanvas = GetComponentInChildren<Canvas>();
        _life.fillAmount = _health / _maxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        if (_player != null) 
        {
            _lifeCanvas.transform.LookAt(_player.transform);
            _enemyNav.SetDestination(_player.transform.position);
            if (!_isWalking)
            {
                _isWalking = true;
                _enemyAnim.SetBool("isWalking", true);
            }
            if (_fireRate < 0)
            {
                _fireRate = 0.2f;
                Fire();

            }
            _fireRate -= Time.deltaTime;
        
            if (_enemyNav.velocity.magnitude/_enemyNav.speed==0 && _isWalking)
            {
                _isWalking = false;
                _enemyAnim.SetBool("isWalking", false);
            }
        }
        
    }

    public void setHealth(float value)
    {
        _health = value;
        _maxHealth = value;
    }

    void Fire()
    {
        GameObject tempBullet = Instantiate(_bullet, _bulletHolder.transform.position, _bulletHolder.transform.rotation) as GameObject;
        Rigidbody tempRigidBodyBullet = tempBullet.GetComponent<Rigidbody>();
        tempRigidBodyBullet.AddForce(tempRigidBodyBullet.transform.forward * _bulletSpeed);
        Destroy(tempBullet, 1f);
    }

    public void HealthUpdate(float damage)
    {
        _health -= damage;
        _life.fillAmount = _health / _maxHealth;
        if (_health <= 0 && !_destroyed)
        {
            _destroyed = true;
            _playerController.UpdateScore(_scoreByKill * _round);
            _enemiesManager.UpdateEnemies();
            _enemyAnim.SetTrigger("Destroyed");
            Destroy(gameObject, 1f);

        }
    }

    public void SetRound(float value)
    {
        _round = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bullet")
        {
            Bullet aux = other.GetComponent<Bullet>();
            HealthUpdate(aux.damage);
        }else if(other.tag == "Misil")
        {
            _audioManager.PlayAudio("Explosion");
            Misil aux = other.GetComponent<Misil>();
            HealthUpdate(aux.damage);
            
        }
        Destroy(other.gameObject);
    }
}
