using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Threading.Tasks;

namespace Main.Audio
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
            // シングルトンのため複数生成禁止
            if (null != instance)
            {
                Destroy(gameObject);
                return;
            }

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
        /// <summary>メニューを開く</summary>
        se_menu = 0,
        /// <summary>メニューを閉じる</summary>
        se_close = 1,
        /// <summary>項目の決定</summary>
        se_decided = 2,
        /// <summary>ゲームクリア</summary>
        me_game_clear = 3,
        /// <summary>ステージセレクト</summary>
        se_select = 4,
        /// <summary>ジャンプ（候補１）</summary>
        se_player_jump_No1,
        /// <summary>ジャンプ（候補２）</summary>
        se_player_jump_No3,
        /// <summary>圧死音</summary>
        se_player_dead,
        /// <summary>遊び方表_開く音（候補１）</summary>
        se_play_open_No2,
        /// <summary>遊び方表_開く音（候補２）</summary>
        se_play_open_No3,
    }
}
