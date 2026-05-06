using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damage : MonoBehaviour
{
    [Header("ÌØÐ§")]
    public GameObject sfx;
    public SpriteRenderer characterRenderer;

    public UnityEvent onTriggerEnter;
    public float cooldown;
    public float backDistance;
    public float upDistance;
    private bool canDamage = true;

    void Start()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        if (!canDamage) return;
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            playerHealth.health -= 1;
            other.GetComponent<Rigidbody>().velocity =new Vector3(backDistance,upDistance,0);
            PlayBulletSfx(other.transform.position);
            HitStopController.Instance.HitStop(0.05f, 0f);
            StartCoroutine(DamageCooldown());
        }
    }
    public void PlayBulletSfx(Vector3 position)
    {
        GameObject sfx1 = Instantiate(sfx, position, Quaternion.identity);
        Destroy(sfx1, 5f);
    }

    IEnumerator DamageCooldown()
    {
        canDamage = false;
        characterRenderer.color = Color.red;

        yield return new WaitForSeconds(cooldown);

        characterRenderer.color = Color.white;
        canDamage = true;
    }
}
