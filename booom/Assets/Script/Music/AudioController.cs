using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] AudioSource BgmAudio;
    [SerializeField] AudioSource SfxAudio;

    public AudioClip Bgm;
    public AudioClip Walk;
    public AudioClip Jump;
    public AudioClip Shoot;
    public AudioClip Dead;

    private void Start()
    {
/*        BgmAudio.clip = Bgm;
        BgmAudio.Play();*/
    }

    public void PlaySfx(AudioClip clip)
    {
        SfxAudio.PlayOneShot(clip);
    }
}
