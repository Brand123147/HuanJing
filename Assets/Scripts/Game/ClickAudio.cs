using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAudio : MonoBehaviour {

    private AudioSource m_AudioSource;
    private void Awake()
    {
        EventCenter.AddListener(EventDefine.PlayButtonAudio, PlayButtonAudio);
        EventCenter.AddListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
        m_AudioSource = GetComponent<AudioSource>();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
        EventCenter.RemoveListener(EventDefine.PlayButtonAudio, PlayButtonAudio);
    }
    /// <summary>
    /// 播放按钮音效
    /// </summary>
    private void PlayButtonAudio()
    {
        m_AudioSource.PlayOneShot(ManagerVars.GetManagerVars().buttonClip);
    }

    /// <summary>
    /// 音效是否开启
    /// </summary>
    /// <param name="value"></param>
    private void IsMusicOn(bool value)
    {
        m_AudioSource.mute = !value;
    }
}
