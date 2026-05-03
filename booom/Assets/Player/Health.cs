using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health;
    public GameObject[] healthUI;

    void Update()
    {
        healthUI[0].SetActive(health >= 1);
        healthUI[1].SetActive(health >= 2);
        healthUI[2].SetActive(health >= 3);
        healthUI[3].SetActive(health >= 4);
        healthUI[4].SetActive(health >= 5);
    }

}
