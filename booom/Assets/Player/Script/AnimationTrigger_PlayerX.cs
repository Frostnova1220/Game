using UnityEngine;

public class AnimationTrigger_PlayerX : MonoBehaviour
{
    private Player_X playerX;

    void Awake()
    {
        playerX = GetComponentInParent<Player_X>();
    }

    public void Trigger()
    {
        if (playerX != null)
            playerX.triggerCalled = true;
    }
}