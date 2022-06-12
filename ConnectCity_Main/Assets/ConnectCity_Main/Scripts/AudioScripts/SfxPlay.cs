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
        public override bool Initialize()
        {
            try
            {
                if (!audioSource)
                {
                    audioSource = GetComponent<AudioSource>();
                    audioSource.playOnAwake = false;
                }
                return true;
            }
            catch
            {
                return false;
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
        /// <summary>キャンセル</summary>
        se_cancel = 1,
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
        /// <summary>リトライ（候補１）</summary>
        se_retry_No1,
        /// <summary>リトライ（候補２）</summary>
        se_retry_No3,
        /// <summary>レーザー（候補１）</summary>
        se_laser_No1,
        /// <summary>レーザー（候補２）</summary>
        se_laser_No3,
        /// <summary>浮遊音</summary>
        se_block_float,
        /// <summary>崩壊音（候補１）</summary>
        se_collapse_No1,
        /// <summary>崩壊音（候補２）</summary>
        se_collapse_No3,
        /// <summary>落下音（候補１）</summary>
        se_player_fall_No1,
        /// <summary>落下音（候補２）</summary>
        se_player_fall_No3,
        /// <summary>接続音（候補１）</summary>
        se_conect_No1,
        /// <summary>接続音（候補２）</summary>
        se_conect_No2,
        /// <summary>接続音（候補３）</summary>
        se_conect_No3,
    }
}
