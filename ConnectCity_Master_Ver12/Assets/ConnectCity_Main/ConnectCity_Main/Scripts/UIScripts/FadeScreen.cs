using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

namespace Main.UI
{
    /// <summary>
    /// UIへフェード演出を入れるスクリプトクラス
    /// </summary>
    public class FadeScreen : MonoBehaviour
    {
        /// <summary>フェードアウト演出の実行中</summary>
        private bool _playingFadeOut;
        /// <summary>フェードアウト演出の実行中</summary>
        public bool PlayingFadeOut => _playingFadeOut;

        /// <summary>
        /// フェードイン処理＆ステージ開始演出
        /// </summary>
        public void PlayFadeInAndStartCutScene()
        {
            if (_playingFadeOut)
                _playingFadeOut = false;
            GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().Player.SetActive(false);
            transform.GetChild(0).GetComponent<Image>().DOFade(endValue: 0f, duration: 1f)
                .OnComplete(() =>
                {
                    //GameManager.Instance.UIOwner.GetComponent<UIOwner>().enabled = true;
                    // スタート演出の中でプレイヤーを有効にする
                    if (!GameManager.Instance.UIOwner.GetComponent<UIOwner>().PlayStartCutsceneFromSceneOwner())
                        Debug.LogError("スタート演出の失敗");
                });
        }

        /// <summary>
        /// フェードアウト処理
        /// </summary>
        public void DrawLoadNowFadeOut()
        {
            if (!_playingFadeOut)
            {
                _playingFadeOut = true;
                transform.GetChild(0).GetComponent<Image>().DOFade(endValue: 1f, duration: 1f)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                    // ロード処理の終了通知を受け取ったら一時停止を解除
                    switch (GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().PlayLoadScene())
                        {
                            case SceneLoadType.SceneLoad:
                                if (Time.timeScale == 0f)
                                    Time.timeScale = 1f;
                                break;
                            case SceneLoadType.PrefabLoad:
                                GameManager.Instance.ReStart();
                                if (Time.timeScale == 0f)
                                    Time.timeScale = 1f;
                                break;
                            case SceneLoadType.Error:
                                Debug.LogError("ロード処理の失敗");
                                break;
                            default:
                                Debug.LogError("ロード処理の例外エラー");
                                break;
                        }
                    });
            }
        }
    }
}
