using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Threading.Tasks;

namespace Main.Audio
{
    [RequireComponent(typeof(AudioSource))]
    /// <summary>
    /// 効果音を再生するクラス
    /// </summary>
    public class SfxPlay : MonoBehaviour
    {
        /// <summary>クラス自身</summary>
        private static SfxPlay instance;
        /// <summary>シングルトンのインスタンス</summary>
        public static SfxPlay Instance
        {
            get { return instance; }
        }

        /// <summary>オーディオソース</summary>
        [SerializeField] private AudioSource audioSource;
        /// <summary>効果音のクリップ</summary>
        [SerializeField] private AudioClip[] clip;
        //[SerializeField] private float limitTimeConnectSFX = 1.985f;
        //[SerializeField] private float _timeUp = 0f;

        private void Reset()
        {
            Initialize();
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

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// 初期設定
        /// </summary>
        private void Initialize()
        {
            if (!audioSource)
            {
                audioSource = GetComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }

        /// <summary>
        /// 指定されたSEを再生する
        /// </summary>
        /// <param name="clipToPlay">SE</param>
        public void PlaySFX(ClipToPlay clipToPlay)
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

        //private IEnumerator LoopReadClipsQue(ClipToPlay clipToPlay, float waitTime)
        //{
        //    yield return new WaitForSeconds(waitTime);
        //    PlaySFX(clipToPlay);
        //}

        //private IEnumerator _cntUpTimer;

        //private IEnumerator CntUpTimer()
        //{
        //    while (_timeUp < limitTimeConnectSFX)
        //    {
        //        _timeUp += Time.deltaTime;
        //        yield return null;
        //    }
        //    _timeUp = 0f;
        //    StopCoroutine(_cntUpTimer);
        //}

        //public void PlaySFXMetronomeAsync(ClipToPlay clipToPlay)
        //{

        //    // 音の再生時間をあらかじめ設定する
        //    // 音の再生処理からタイマーを起動
        //    // 二回目以降の再生処理は（再生時間-タイマー時間+0.5f）を待機時間として設定
        //    // コルーチンの処理へ引数を渡してStartCoroutine

        //    if (0f < _timeUp)
        //    {
        //        StopCoroutine(_cntUpTimer);
        //        _timeUp = .01f;
        //        _cntUpTimer = null;
        //        _cntUpTimer = CntUpTimer();
        //        StartCoroutine(_cntUpTimer);
        //        StartCoroutine(LoopReadClipsQue(clipToPlay, limitTimeConnectSFX - _timeUp/* + .5f*/));
        //    }
        //    else
        //    {
        //        _cntUpTimer = CntUpTimer();
        //        StartCoroutine(_cntUpTimer);
        //        StartCoroutine(LoopReadClipsQue(clipToPlay, 0f));
        //    }
        //}
    }

    /// <summary>
    /// オーディオクリップリストのインデックス
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
        se_select = 4
    }
}
