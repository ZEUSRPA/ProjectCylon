using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Scene_Manager _scene;
    private UIController _uIController;
    private AudioManager _audioManager;
    private float _health = 100;
    private Rigidbody _playerRigidbody;
    //private Camera _camera;
    private List<GameObject> _bulletHolders=new List<GameObject>();
    private float _fireRate=0.3f;
    private float _recharge;
    private bool _firstRound = true;
    private float _misilRate = 4f;
    private float _misilRecharge;
    private float _regenerationTime = 2f;
    private float _damageControlTime = 2f;
    private static float _score = 0;
    public float score { get { return _score; } set { _score = value; } }
    private float _upgrades = 0;
    private Animator _playerAnim;

    private GameObject _body;

    [SerializeField]
    private GameObject _bullet;
    [SerializeField]
    private GameObject _misil;
    private float _bulletSpeed=3000;
    private float _misilSpeed = 1500;
    private float _playerSpeed = 15f;
    private float _jumpForce = 10f;
    private float _mouseX;
    private float _mouseY=90;
    private float _moveFB;
    private float _moveLR;
    private float _moveUp;
    private float _sensitivity=5f;
    // Start is called before the first frame update
    void Start()
    {
        _scene = GameObject.FindObjectOfType<Scene_Manager>();
        _uIController = GameObject.Find("UI").GetComponentInChildren<UIController>();
        _playerAnim = GetComponentInChildren<Animator>();
        _body = transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        
        _audioManager = GameObject.FindObjectOfType<AudioManager>();
        _recharge = _fireRate;
        _misilRecharge = _misilRate;

        for(int i = 13; i <= 17; i++)
        {
            _bulletHolders.Add(_body.transform.GetChild(i).transform.GetChild(0).gameObject);
        }
        Cursor.lockState = CursorLockMode.Locked;
        //_camera = GetComponentInChildren<Camera>();
        
        _moveLR = 0;
        _moveFB = 0;
        _moveUp = 0;
        _playerRigidbody = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        _mouseX = Input.GetAxis("Mouse X");
        _mouseY -= Input.GetAxis("Mouse Y")*_sensitivity;
        _mouseY = Mathf.Clamp(_mouseY, -45, 45);
        //_camera.transform.localRotation = Quaternion.Euler(_mouseY, 0, 0);
        _body.transform.localRotation = Quaternion.Euler(0, 180+_mouseY, 0);
        //_weapon.transform.localRotation = Quaternion.Euler(_mouseY, 0, 0);
        //_camera.transform.Rotate(-_mouseY * _sensitivity, 0, 0);
        //_weapon.transform.Rotate(-_mouseY * _sensitivity, 0, 0);
        transform.Rotate(0, _mouseX * _sensitivity, 0);

        _moveFB = Input.GetAxis("Vertical");
        _moveLR = Input.GetAxis("Horizontal");
        //Jump
        //if (Input.GetKeyDown("space"))
        //{
        //    _moveUp = 1;
        //}

        MovePlayer();

        if (Input.GetMouseButton(0))
        {
            if (_recharge < 0)
            {
                _recharge = _fireRate;
                Fire();
            }

        }
        if (Input.GetMouseButton(1))
        {
            if (_misilRecharge < 0)
            {
                _misilRecharge = _misilRate;
                ThrowMisil();
            }
        }
        if(_damageControlTime < 0 && _health<100)
        {
            _health++;
            _damageControlTime = _regenerationTime;
            UpdateLifeBar();
        }
        _recharge -= Time.deltaTime;
        _misilRecharge -= Time.deltaTime;
        _damageControlTime -= Time.deltaTime;
    }

    private void UpdateLifeBar()
    {
        _uIController.UpdateLifeBar(_health);
    } 

    public void UpdateRecharge()
    {
        _fireRate /= 2;
        _misilRate /= 2;
        _uIController.ShowMessage("Recarga X2");
    }

    public void UpdateRegeneration()
    {
        _regenerationTime /= 2;
        _uIController.ShowMessage("Regeneracion X2");
    }

    public void UpdateDamage()
    {
        _bullet.GetComponent<Bullet>().damage *= 2;
        _misil.GetComponent<Misil>().damage *= 2;
        _uIController.ShowMessage("Daño X2");
    }
    private void MovePlayer()
    {
        Vector3 movement = new Vector3(_moveLR, 0, _moveFB).normalized * _playerSpeed;
        movement.y = _playerRigidbody.velocity.y + _moveUp * _jumpForce;
        movement = transform.rotation * movement;
        _playerRigidbody.velocity = movement;
        _moveFB = 0;
        _moveLR = 0;
        _moveUp = 0;
    }

    public void UpdateScore(float value)
    {
        _score += value;
        _uIController.UpdateScore(_score);
    }

    private void ThrowMisil()
    {
        _audioManager.PlayAudio("Misil");
        GameObject tempMisil = Instantiate(_misil, _bulletHolders[4].transform.position, _bulletHolders[4].transform.rotation) as GameObject;
        Rigidbody tempRigidBodyBullet = tempMisil.GetComponent<Rigidbody>();
        tempRigidBodyBullet.AddForce(tempRigidBodyBullet.transform.forward * _misilSpeed);
        Destroy(tempMisil, 5f);
    }

    private void Fire()
    {
        int weapons;
        _audioManager.PlayAudio("Shoot");
        if (_firstRound)
        {
            _playerAnim.SetTrigger("ShootingA");
            weapons = 0;
            _firstRound = false;
        }
        else
        {
            _playerAnim.SetTrigger("ShootingB");
            weapons = 2;
            _firstRound = true;
        }

        for(int i = 0; i < 2; i++)
        {
            GameObject tempBullet = Instantiate(_bullet, _bulletHolders[weapons+i].transform.position, _bulletHolders[weapons+i].transform.rotation) as GameObject;
            Rigidbody tempRigidBodyBullet = tempBullet.GetComponent<Rigidbody>();
            tempRigidBodyBullet.AddForce(tempRigidBodyBullet.transform.forward * _bulletSpeed);
            Destroy(tempBullet, 1f);
        }
    }

    void HealthUpdate(float value)
    {
        _health += value;
        UpdateLifeBar();
        if (_health <= 0)
        {
            _scene.LoadGameOver();
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyBullet")
        {
            _audioManager.PlayAudio("Impact");
            HealthUpdate(-2);
        }else if(other.tag == "Upgrade")
        {
            _audioManager.PlayAudio("Upgrade");
            if (_upgrades == 0)
            {
                _upgrades++;
                UpdateRecharge();
            }else if (_upgrades == 1)
            {
                _upgrades++;
                UpdateDamage();
            }
            else
            {
                _upgrades++;
                UpdateRegeneration();
            }
            Destroy(other.gameObject);
        }
    }
}
