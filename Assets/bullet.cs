using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    private Vector3 shootDir;
    void Start()
    {
        
    }

    public void Setup(Vector3 shootDir)
    {
        this.shootDir = shootDir;
    }

    void Update()
    {
        float moveSpeed = 100f;
        transform.position += shootDir * moveSpeed* Time.deltaTime;
        
    }

    
}
