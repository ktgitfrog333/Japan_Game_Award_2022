using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Threading.Tasks;

namespace TitleSelect
{
    /// <summary>
    /// 効果音を再生するクラス
    /// </summary>
    public class SfxPlay : MasterAudioPlay
    {
        /// <summary>クラス自身</summary>
        private static SfxPlay instance;
        /// <summary>シングルトンのインスタンス</summary>
        public static SfxPlay Instance
        {
            get { return instance; }
        }

        private void Awake()
        {
            //// シングルトンのため複数生成禁止
            //if (null != instance)
            //{
            //    Destroy(gameObject);
            //    return;
            //}

            instance = this;
        }

        protected override void Initialize()
        {
            if (!audioSource)
            {
                audioSource = GetComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }

        public override void PlaySFX(ClipToPlay clipToPlay)
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
    /// SFX・MEオーディオクリップリストのインデックス
    /// </summary>
    public enum ClipToPlay
    {
        /// <summary>キャンセル</summary>
        se_cancel = 0,
        /// <summary>項目の決定</summary>
        se_decided = 1,
        /// <summary>ゲームスタート（候補１）</summary>
        se_game_start_No1 = 2,
        /// <summary>ゲームスタート（候補２）</summary>
        se_game_start_No2 = 3,
        /// <summary>ゲームスタート（候補３）</summary>
        se_game_start_No3 = 4,
        /// <summary>ステージセレクト</summary>
        se_select = 5,
    }
}
