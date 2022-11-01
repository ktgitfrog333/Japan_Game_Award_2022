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
        /// <summary>オーディオソース用のプレハブ</summary>
        [SerializeField] private GameObject sFXChannelPrefab;
        /// <summary>プール用</summary>
        private Transform _transform;
        /// <summary>プール済みのオーディオ情報マップ</summary>
        private Dictionary<ClipToPlay, int> _sfxIdxDictionary = new Dictionary<ClipToPlay, int>();

        public override bool Initialize()
        {
            try
            {
                if (_transform == null)
                    _transform = transform;
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
                    var audio = GetSFXSource(clipToPlay);
                    audio.clip = clip[(int)clipToPlay];

                    // SEを再生
                    audio.Play();
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("対象のファイルが見つかりません:[" + clipToPlay + "]");
                Debug.Log(e);
            }
        }

        /// <summary>
        /// SFXのキーから対象のオーディオソースを取得する
        /// </summary>
        /// <param name="key">ClipToPlayのキー</param>
        /// <returns>オーディオソース</returns>
        private AudioSource GetSFXSource(ClipToPlay key)
        {
            if (!_sfxIdxDictionary.ContainsKey(key))
            {
                var sfx = Instantiate(sFXChannelPrefab);
                sfx.transform.parent = _transform;
                _sfxIdxDictionary.Add(key, _transform.childCount - 1);
                return sfx.GetComponent<AudioSource>();
            }
            return _transform.GetChild(_sfxIdxDictionary[key]).GetComponent<AudioSource>();
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
