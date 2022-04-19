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
        /// <summary>ビデオクリップ（リソースからセットする）</summary>
        [SerializeField] private VideoClip[] channels;
        /// <summary>ビデオプレイヤー</summary>
        [SerializeField] private VideoPlayer videoPlayer;
        /// <summary>ビデオを再生させるトリガー</summary>
        [SerializeField] private GameObject[] triggers;
        /// <summary>出現演出の時間</summary>
        [SerializeField] float duration = .3f;

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

            foreach (var trigger in triggers)
            {
                trigger.OnTriggerEnterAsObservable()
                    .Where(x => x.CompareTag(TagConst.TAG_NAME_PLAYER))
                    .Subscribe(_ =>
                    {
                        player.GetComponent<RawImage>().color = new Vector4(255f, 255f, 255f, 0f);
                        player.gameObject.SetActive(true);
                        player.GetComponent<RawImage>().DOFade(endValue: 1f, duration: duration)
                            .OnComplete(() =>
                            {
                                var name = trigger.name;
                                if (!PlayVideoClip(System.Int32.Parse(name.Substring(name.IndexOf("_") + 1))))
                                    Debug.LogError("ビデオクリップ再生処理の失敗");
                            });
                    });
                trigger.OnTriggerExitAsObservable()
                    .Where(x => x.CompareTag(TagConst.TAG_NAME_PLAYER))
                    .Subscribe(_ =>
                    {
                        player.GetComponent<RawImage>().DOFade(endValue: 0f, duration: duration)
                            .OnComplete(() => player.gameObject.SetActive(false));
                    });
            }
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
    }
}
