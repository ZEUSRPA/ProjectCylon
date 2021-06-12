using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _damage=5;
    public float damage { get { return _damage; } set { _damage = value; } }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
