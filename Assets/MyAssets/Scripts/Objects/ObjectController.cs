using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Garlic" || other.tag == "EnemyBat" || other.tag == "Garlic2")
        {
            Destroy(other.gameObject);
        }
    }
}
