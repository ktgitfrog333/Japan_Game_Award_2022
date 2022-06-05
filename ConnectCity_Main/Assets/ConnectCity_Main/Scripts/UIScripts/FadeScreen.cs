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
        void Start()
        {
            ManualStart();
        }

        /// <summary>
        /// 疑似スタートイベント
        /// </summary>
        public void ManualStart()
        {
            GameManager.Instance.UIOwner.GetComponent<UIOwner>().enabled = false;
            DrawLoadNowFadeIn();
        }

        /// <summary>
        /// フェードイン処理
        /// </summary>
        private void DrawLoadNowFadeIn()
        {
            GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().Player.SetActive(false);
            transform.GetChild(0).GetComponent<Image>().DOFade(endValue: 0f, duration: 1f)
                .OnComplete(() =>
                {
                    GameManager.Instance.UIOwner.GetComponent<UIOwner>().enabled = true;
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
                            // ゴール演出の後処理
                            if (!GameManager.Instance.UIOwner.GetComponent<UIOwner>().DestroyParticleFromFadeScreen())
                                Debug.LogError("ゴール演出の後処理の失敗");
                            // 同じステージをリロードする場合はスタート演出を短くする
                            if (!GameManager.Instance.UIOwner.GetComponent<UIOwner>().SetStartCutsceneContinueFromFadeScreen(GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().LoadSceneId == GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().SceneIdCrumb.Current))
                                Debug.LogError("リスタートフラグセットの失敗");
                            GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().UpdateScenesMap(GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().LoadSceneId);
                            if (!GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().StartStage())
                                Debug.LogError("ステージ開始処理の失敗");
                            if (!GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().PlayManualStartFromSceneOwner())
                                Debug.LogError("疑似スタートイベント発火処理の失敗");
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
