using System.Collections;
using UnityEngine;

public class HitStopController : MonoBehaviour
{
    public static HitStopController Instance;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    public void HitStop(float duration, float timeScale = 0f)
    {
        StartCoroutine(FreezeFrame(duration, timeScale));
    }

    IEnumerator FreezeFrame(float duration, float timeScale)
    {
        float original = Time.timeScale;
        Time.timeScale = timeScale;

        yield return new WaitForSecondsRealtime(duration); 

        Time.timeScale = original;
    }
}