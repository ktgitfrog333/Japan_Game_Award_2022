using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Main.Common.Const;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using DG.Tweening;

namespace Main.UI
{
    /// <summary>
    /// チュートリアル画面
    /// トリガーとなるオブジェクト（TutorialTrigger_0）
    /// ※上記オブジェクトを「*_0...99」のように一意の番号で付けることでビデオクリップの配列番号「channels」と連動する
    /// </summary>
    public class TutorialScreen : MonoBehaviour
    {
        /// <summary>
        /// ビデオクリップ（リソースからセットする）
        /// チャンネル0：基本操作　移動について
        /// チャンネル1：基本操作　ジャンプについて
        /// チャンネル2：基本操作　コネクトシステムについて
        /// T.B.D チャンネルx：基本操作　死亡条件について
        /// T.B.D チャンネルx：基本操作　クリア条件について
        /// T.B.D チャンネルx：ギミック　敵について
        /// T.B.D チャンネルx：ギミック　重力について
        /// T.B.D チャンネルx：ギミック　レーザー砲について
        /// T.B.D チャンネルx：ギミック　ぼろいブロック・天井について
        /// </summary>
        [SerializeField] private VideoClip[] channels;
        /// <summary>ビデオプレイヤー</summary>
        [SerializeField] private VideoPlayer videoPlayer;
        /// <summary>ビデオを再生させるトリガー</summary>
        [SerializeField] private GameObject[] triggers;
        /// <summary>出現演出の時間</summary>
        [SerializeField] float durationFade = .3f;
        /// <summary>点滅演出の時間</summary>
        [SerializeField] float durationLoopFlash = .3f;
        /// <summary>簡易タイプ</summary>
        [SerializeField] Ease easeType = Ease.InCubic;
        /// <summary>入力操作アイコンの切替時間</summary>
        [SerializeField, Range(3, 7)] private double switchInterval = 5d;

        private void Reset()
        {
            if (videoPlayer == null)
                videoPlayer = transform.GetChild(0).GetComponent<VideoPlayer>();
            if (triggers == null || (triggers != null && triggers.Length == 0))
            {
                triggers = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_TUTORIALTRIGGER);
            }
        }

        private void Start()
        {
            // プレイヤー初期状態は表示させない
            var player = transform.GetChild(0);
            player.gameObject.SetActive(false);
            var anims = transform.GetChild(1);
            foreach (Transform anim in anims)
                anim.gameObject.SetActive(false);

            foreach (var trigger in triggers)
            {
                trigger.OnTriggerEnterAsObservable()
                    .Where(x => x.CompareTag(TagConst.TAG_NAME_PLAYER))
                    .Subscribe(_ =>
                    {
                        var compCnt = new IntReactiveProperty(0);
                        if (!PlayFadeAndSetCompleteCount<RawImage>(player, compCnt, player.GetComponent<RawImage>()))
                            Debug.LogError("RawImage:フェード処理の失敗");
                        var anim = transform.GetChild(1).GetChild(GetIdx(trigger.name));
                        if (!PlayFadeAndSetCompleteCount(anim, compCnt))
                            Debug.LogError("CanvasGroup:フェード処理の失敗");

                        compCnt.Where(x => 1 < x)
                            .Subscribe(_ =>
                            {
                                if (!PlayVideoClip(GetIdx(trigger.name)))
                                    Debug.LogError("ビデオクリップ再生処理の失敗");
                                var subChaIdx = new IntReactiveProperty(0);
                                Observable.Interval(System.TimeSpan.FromSeconds(switchInterval))
                                    .Subscribe(_ => subChaIdx.Value = (subChaIdx.Value == 0) ? 1 : 0)
                                    .AddTo(this);
                                subChaIdx.Where(x => x == 0 | x == 1)
                                    .Subscribe(x =>
                                    {
                                        // 時間差で切り替える
                                        if (!ChangeSubChannel(anim, x))
                                            Debug.LogError("コントローラー／キーボード表示切替の失敗");
                                    });
                                // コントローラーとキーボードの点滅部分は常時開始させておく
                                var s = 0;
                                if (!PlayFlash(anim, GetIdx(trigger.name), s++))
                                    Debug.LogError("点滅再生処理の失敗");
                                if (!PlayFlash(anim, GetIdx(trigger.name), s++))
                                    Debug.LogError("点滅再生処理の失敗");
                            });
                    });
                trigger.OnTriggerExitAsObservable()
                    .Where(x => x.CompareTag(TagConst.TAG_NAME_PLAYER))
                    .Subscribe(_ =>
                    {
                        player.gameObject.SetActive(false);
                        var anim = transform.GetChild(1).GetChild(GetIdx(trigger.name));
                        anim.gameObject.SetActive(false);
                    });
            }
        }

        /// <summary>
        /// サブチャンネル
        /// 0と1を切り替える
        /// </summary>
        /// <param name="tran">コンテンツ</param>
        /// <param name="subChannelIdx">サブチャンネルのインデックス</param>
        /// <returns>成功／失敗</returns>
        private bool ChangeSubChannel(Transform tran, int subChannelIdx)
        {
            try
            {
                // 全部のアルファ値をゼロにする
                for (var i = 0; i < tran.childCount; i++)
                    tran.GetChild(i).GetComponent<CanvasGroup>().alpha = 0f;
                tran.GetChild(subChannelIdx).GetComponent<CanvasGroup>().alpha = 1f;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// フェード処理の再生
        /// 終了した際に完了カウントをカウントアップ
        /// </summary>
        /// <param name="tran">子オブジェクト</param>
        /// <param name="count">完了カウント</param>
        /// <returns></returns>
        private bool PlayFadeAndSetCompleteCount(Transform tran, IntReactiveProperty count)
        {
            tran.gameObject.SetActive(true);
            for (var i = 0; i < tran.childCount; i++){
                tran.GetChild(i).GetComponent<CanvasGroup>().alpha = 0f;
                tran.GetChild(i).gameObject.SetActive(true);
            }

            tran.GetChild(0).GetComponent<CanvasGroup>().DOFade(endValue: 1f, duration: durationFade)
                .OnComplete(() => count.Value++);

            return true;
        }

        /// <summary>
        /// フェード処理の再生
        /// 終了した際に完了カウントをカウントアップ
        /// </summary>
        /// <param name="tran">子オブジェクト</param>
        /// <param name="count">完了カウント</param>
        /// <returns></returns>
        private bool PlayFadeAndSetCompleteCount<T>(Transform tran, IntReactiveProperty count, T type)
        {
            if (typeof(RawImage) == type.GetType())
            {
                tran.GetComponent<RawImage>().color = new Vector4(255f, 255f, 255f, 0f);
                tran.gameObject.SetActive(true);
                tran.GetComponent<RawImage>().DOFade(endValue: 1f, duration: durationFade)
                    .OnComplete(() => count.Value++);
            }
            else if (typeof(CanvasGroup) == type.GetType())
            {
                tran.GetComponent<CanvasGroup>().alpha = 0f;
                tran.gameObject.SetActive(true);
                tran.GetComponent<CanvasGroup>().DOFade(endValue: 1f, duration: durationFade)
                    .OnComplete(() => count.Value++)
                    .SetLink(gameObject);
            }
            else
                return false;
            return true;
        }

        /// <summary>
        /// UIのImageを点滅させる
        /// コンポーネントが参照できないオブジェクトもある可能性があるためチャンネルで判定する
        /// </summary>
        /// <param name="content">TutorialInputKeyAnimGroupの子要素</param>
        /// <param name="channelIdx">再生させるビデオチャンネル</param>
        /// <returns></returns>
        private bool PlayFlash(Transform content, int channelIdx, int subChannelIdx)
        {
            // 点滅が発生するチャンネルのみ実行
            if (channelIdx == 0 ||
                channelIdx == 1 ||
                channelIdx == 2 ||
                channelIdx == 3)
            {
                // 点滅させる
                content.GetChild(subChannelIdx).GetChild(1).GetComponent<CanvasGroup>().DOFade(0f, durationLoopFlash)
                    .SetEase(easeType)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetLink(gameObject);
            }
            return true;
        }

        /// <summary>
        /// ビデオクリップの再生
        /// </summary>
        /// <param name="channelIdx">再生させるビデオチャンネル</param>
        /// <returns>成功／失敗</returns>
        private bool PlayVideoClip(int channelIdx)
        {
            try
            {
                videoPlayer.clip = channels[channelIdx];
                videoPlayer.Play();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 何番目から取得
        /// </summary>
        /// <param name="name">名前（.*_nからnを取得）</param>
        /// <returns>インデックス</returns>
        private int GetIdx(string name)
        {
            return System.Int32.Parse(name.Substring(name.IndexOf("_") + 1));
        }
    }
}
