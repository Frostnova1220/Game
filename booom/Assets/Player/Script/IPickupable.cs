using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickupable
{
    void OnPickup(GameObject player);
    string GetPickupName();
    int GetMaxUses();
}
