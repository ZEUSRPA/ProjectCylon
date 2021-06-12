using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _enemyBases;

    [SerializeField]
    private GameObject _enemy;
    private UIController _uIController;
    private float _enemies=0;
    private float _round = 0;

    private float _enemiesHealth = 20;

    private static EnemiesManager _instance;

    private float _waitTime = 5f;



    private void Awake()
    {
        if (_instance != null && _instance != this) 
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        _uIController = GameObject.Find("UI").GetComponentInChildren<UIController>();
        _uIController.ShowMessage("Supervivencia");
    }

    void CreateEnemies()
    {
        _round++;
        _uIController.ShowMessage("Ronda " + _round);
        foreach (GameObject x in _enemyBases)
        {
            for (int i = 0; i < 3; i++)
            {
                CreateEnemy(x);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_enemies == 0 && _waitTime<0)
        {
            _enemiesHealth += 4;
            CreateEnemies();
        }
        _waitTime -= Time.deltaTime;
    }

    public void UpdateEnemies()
    {
        _enemies--;
        _waitTime = 5f;
    }
    void CreateEnemy(GameObject baseLocation)
    {
        GameObject tempEnemy = Instantiate(_enemy, baseLocation.transform.position, _enemy.transform.rotation) as GameObject;
        tempEnemy.GetComponent<EnemyController>().setHealth(_enemiesHealth);
        tempEnemy.GetComponent<EnemyController>().SetRound(_round);
        _enemies++;
    }
}
