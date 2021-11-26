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
    private bool _killed = false;
    private List<GameObject> _garlicHolders=new List<GameObject>();
    private float _fireRate=0.6f;
    private float _recharge;
    private float _garlic2Rate = 4f;
    private float _garlic2Recharge;
    private float _regenerationTime = 2f;
    private float _damageControlTime = 2f;
    private static float _score = 0;
    public float score { get { return _score; } set { _score = value; } }
    private float _upgrades = 0;
    private Animator _playerAnim;
    private float _updating = 2f;

    private GameObject _body;

    [SerializeField]
    private GameObject _garlic;
    [SerializeField]
    private GameObject _garlic2;
    private float _garlicSpeed=3000;
    private float _garlic2Speed = 1500;
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
        _body = transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).gameObject;
        
        _audioManager = GameObject.FindObjectOfType<AudioManager>();
        _recharge = _fireRate;
        _garlic2Recharge = _garlic2Rate;

        for(int i = 0; i <= 1; i++)
        {
            _garlicHolders.Add(_body.transform.GetChild(i).transform.GetChild(0).gameObject);
        }
        Cursor.lockState = CursorLockMode.Locked;
        
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
        if (!_killed)
        {

            _mouseX = Input.GetAxis("Mouse X");
            _mouseY -= Input.GetAxis("Mouse Y")*_sensitivity;
            _mouseY = Mathf.Clamp(_mouseY, -45, 45);
            _body.transform.localRotation = Quaternion.Euler(_mouseY, 0, 0);
            transform.Rotate(0, _mouseX * _sensitivity, 0);

            _moveFB = Input.GetAxis("Vertical");
            _moveLR = Input.GetAxis("Horizontal");
        
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
                if (_garlic2Recharge < 0)
                {
                    _garlic2Recharge = _garlic2Rate;
                    ThrowGarlic2();
                }
            }
            if(_damageControlTime < 0 && _health<100)
            {
                _health++;
                _damageControlTime = _regenerationTime;
                UpdateLifeBar();
            }
            _recharge -= Time.deltaTime;
            _garlic2Recharge -= Time.deltaTime;
            _damageControlTime -= Time.deltaTime;
            _updating -= Time.deltaTime;
        }
    }

    private void UpdateLifeBar()
    {
        _uIController.UpdateLifeBar(_health);
    } 

    public void UpdateRecharge()
    {
        _fireRate /= 2;
        _garlic2Rate /= 2;
        _uIController.ShowMessage("Brazo X2");
    }

    public void UpdateRegeneration()
    {
        _regenerationTime /= 2;
        _uIController.ShowMessage("Recuperacion X2");
    }

    public void UpdateDamage()
    {
        _garlic.GetComponent<Garlic>().damage *= 2;
        _garlic2.GetComponent<Garlic2>().damage *= 2;
        _uIController.ShowMessage("Ajinez X2");
    }
    private void MovePlayer()
    {
        Vector3 movement = new Vector3(_moveLR, 0, _moveFB).normalized * _playerSpeed;
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

    private void ThrowGarlic2()
    {
        _playerAnim.SetTrigger("attack2");
        _audioManager.PlayAudio("Garlic2");
        GameObject tempMisil = Instantiate(_garlic2, _garlicHolders[1].transform.position, _garlicHolders[1].transform.rotation) as GameObject;
        Rigidbody tempRigidBodyBullet = tempMisil.GetComponent<Rigidbody>();
        tempRigidBodyBullet.AddForce(tempRigidBodyBullet.transform.forward * _garlic2Speed);
        Destroy(tempMisil, 5f);
    }

    private void Fire()
    {
        _playerAnim.SetTrigger("attack");
        _audioManager.PlayAudio("Garlic");
        GameObject tempBullet = Instantiate(_garlic, _garlicHolders[0].transform.position, _garlicHolders[0].transform.rotation) as GameObject;
        Rigidbody tempRigidBodyBullet = tempBullet.GetComponent<Rigidbody>();
        tempRigidBodyBullet.AddForce(tempRigidBodyBullet.transform.forward * _garlicSpeed);
        Destroy(tempBullet, 1f);

    }

    void HealthUpdate(float value)
    {
        _health += value;
        UpdateLifeBar();
        if (_health <= 0)
        {
            _killed = true;
            _scene.LoadGameOver();
        }
        
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyBat")
        {
            _audioManager.PlayAudio("Impact");
            _audioManager.PlayAudio("Hurt");
            HealthUpdate(-2);
        }else if(other.tag == "Upgrade" && _updating<=0)
        {
            Destroy(other.gameObject,0f);
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
            _updating = 2f;
        }
    }
}
