using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Misil : MonoBehaviour
{
    private float _damage = 20;
    public float damage { get { return _damage; } set { _damage = value; } }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
