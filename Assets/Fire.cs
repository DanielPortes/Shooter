using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public Transform pfBullet;

    public void shoot()
    {
        Transform pos = transform.Find("ponto");
        Transform bullet = Instantiate(pfBullet, pos.transform.position, Quaternion.identity);
        Vector3 shootDir = transform.forward;
        bullet.GetComponent<bullet>().Setup(shootDir);
    }
}
