using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Audio
{
    /// <summary>
    /// BGMのリソース管理
    /// </summary>
    public class BgmPlay : MasterAudioPlay
    {
        protected override void Initialize()
        {
            if (!audioSource)
            {
                audioSource = GetComponent<AudioSource>();
                audioSource.playOnAwake = true;
                audioSource.loop = true;
            }
        }

        public override void PlayBGM(ClipToPlayBGM clipToPlay)
        {
            try
            {
                if ((int)clipToPlay <= (clip.Length - 1))
                {
                    audioSource.clip = clip[(int)clipToPlay];

                    // SEを再生
                    audioSource.Play();
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("対象のファイルが見つかりません:[" + clipToPlay + "]");
                Debug.Log(e);
            }
        }
    }

    /// <summary>
    /// BGMオーディオクリップリストのインデックス
    /// </summary>
    public enum ClipToPlayBGM
    {
        /// <summary>ステージ1～10のBGM？</summary>
        bgm_section_01
    }
}
