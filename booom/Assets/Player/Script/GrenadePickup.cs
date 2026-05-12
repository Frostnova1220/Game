using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadePickup : MonoBehaviour
{
    [Header("╠Мож")]
    public float rotateSpeed = 90f;
    public float bobHeight = 0.2f;
    public float bobSpeed = 2f;

    private Vector3 startPos;
    private float bobOffset;

    void Start()
    {
        startPos = transform.position;
        bobOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        float newY = startPos.y + Mathf.Sin((Time.time + bobOffset) * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GrenadeLauncher.instance != null)
            {
                GrenadeLauncher.instance.PickupGrenade();
                Destroy(gameObject);
            }
        }
    }
}