using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Audio
{
    /// <summary>
    /// BGM／ME・効果音を再生する親クラス
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MasterAudioPlay : MonoBehaviour
    {
        /// <summary>オーディオソース</summary>
        [SerializeField] protected AudioSource audioSource;
        /// <summary>効果音のクリップ</summary>
        [SerializeField] protected AudioClip[] clip;

        private void Reset()
        {
            Initialize();
        }
        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// 初期設定
        /// </summary>
        protected virtual void Initialize() { }

        /// <summary>
        /// 指定されたSEを再生する
        /// </summary>
        /// <param name="clipToPlay">SE</param>
        public virtual void PlaySFX(ClipToPlay clipToPlay) { }

        /// <summary>
        /// 指定されたBGMを再生する
        /// </summary>
        /// <param name="clipToPlay">BGM</param>
        public virtual void PlayBGM(ClipToPlayBGM clipToPlay) { }
    }
}
