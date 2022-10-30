using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Audio
{
    /// <summary>
    /// オーディオのオーナー
    /// </summary>
    public class AudioOwner : MonoBehaviour
    {
        /// <summary>SEを再生</summary>
        [SerializeField] private GameObject sfxPlay;
        /// <summary>BGMを再生</summary>
        [SerializeField] private GameObject bgmPlay;

        private void Reset()
        {
            if (sfxPlay == null)
                sfxPlay = GameObject.Find("SfxPlay");
            if (bgmPlay == null)
                bgmPlay = GameObject.Find("BgmPlay");
        }

        /// <summary>
        /// 初期処理
        /// </summary>
        public bool Initialize()
        {
            try
            {
                if (!sfxPlay.GetComponent<SfxPlay>().Initialize())
                    Debug.LogError("Sfx初期処理の失敗");
                if (!bgmPlay.GetComponent<BgmPlay>().Initialize())
                    Debug.LogError("BGM初期処理の失敗");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// SEを再生
        /// </summary>
        /// <param name="clipToPlay">SE番号</param>
        public void PlaySFX(ClipToPlay clipToPlay)
        {
            sfxPlay.GetComponent<SfxPlay>().PlaySFX(clipToPlay);
        }

        /// <summary>
        /// BGMを再生
        /// </summary>
        /// <param name="clipToPlay">BGM番号</param>
        public void PlayBGM(ClipToPlayBGM clipToPlay)
        {
            bgmPlay.GetComponent<BgmPlay>().PlayBGM(clipToPlay);
        }
    }
}
