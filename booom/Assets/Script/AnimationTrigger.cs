using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    public Player_X Player_X;

    public void Trigger()
    {
        Player_X.triggerCalled = true; 
    }

}
