using UnityEngine;

public class WormholeExit : MonoBehaviour
{
    public Vector3 teleportTarget;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = teleportTarget;
        }
    }
}
