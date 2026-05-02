using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    public Enemy_qu enemy;

    public void OnAttackHit()
    {
        if (enemy != null)
            enemy.OnAttackHit();
        else
            Debug.LogError("AnimationEventReceiver: enemy Œ¥…Ë÷√");
    }

    public void OnAttackEnd()
    {
        if (enemy != null)
            enemy.OnAttackEnd();
        else
            Debug.LogError("AnimationEventReceiver: enemy Œ¥…Ë÷√");
    }
}