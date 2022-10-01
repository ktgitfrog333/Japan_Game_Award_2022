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
        public override bool Initialize()
        {
            try
            {
                if (!audioSource)
                {
                    audioSource = GetComponent<AudioSource>();
                    audioSource.playOnAwake = true;
                    audioSource.loop = true;
                }
                return true;
            }
            catch
            {
                return false;
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
        /// <summary>ステージ1～10のBGM</summary>
        bgm_stage_vol1,
        /// <summary>ステージ11～20のBGM</summary>
        bgm_stage_vol2,
        /// <summary>ステージ21～30のBGM</summary>
        bgm_stage_vol3,
        /// <summary>ステージ31～40のBGM</summary>
        bgm_stage_vol4,
        /// <summary>ステージ41～50のBGM</summary>
        bgm_stage_vol5,
    }
}
