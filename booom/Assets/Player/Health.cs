using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Health : MonoBehaviour
{
    public int health=5;
    public GameObject[] healthUI;
    public GameObject DeathUI;
    public AudioController audioController;
    void Update()
    {
        DetectUI();
        afterDamage();
    }

    public void DetectUI()
    {
        healthUI[0]?.SetActive(health >= 1);
        healthUI[1]?.SetActive(health >= 2);
        healthUI[2]?.SetActive(health >= 3);
        healthUI[3]?.SetActive(health >= 4);
        healthUI[4]?.SetActive(health >= 5);
    }



    public void afterDamage()
    {
        if (health <= 0)
        {
            health = 0;
            audioController.PlaySfx(audioController.Dead);
            gameObject.SetActive(false);
            DeathUI.SetActive(true);
        }
    }
}
