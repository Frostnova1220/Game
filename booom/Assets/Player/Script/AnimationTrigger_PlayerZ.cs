using UnityEngine;

public class AnimationTrigger_PlayerZ : MonoBehaviour
{
    private Player_Z playerZ;

    void Awake()
    {
        playerZ = GetComponentInParent<Player_Z>();
    }

    public void Trigger()
    {
        if (playerZ != null)
            playerZ.triggerCalled = true;
    }
}