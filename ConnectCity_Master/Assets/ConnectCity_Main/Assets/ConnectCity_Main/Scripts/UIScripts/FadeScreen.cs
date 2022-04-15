using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;
using UnityEngine.UI;
using DG.Tweening;

namespace Main.UI
{
    /// <summary>
    /// UIへフェード演出を入れるスクリプトクラス
    /// </summary>
    public class FadeScreen : MonoBehaviour
    {
        void Start()
        {
            UIManager.Instance.enabled = false;
            DrawLoadNowFadeIn();
        }

        /// <summary>
        /// フェードイン処理
        /// </summary>
        private void DrawLoadNowFadeIn()
        {
            GameManager.Instance.Player.SetActive(false);
            transform.GetChild(0).GetComponent<Image>().DOFade(endValue: 0f, duration: 1f)
                .OnComplete(() =>
                {
                    UIManager.Instance.enabled = true;
                    GameManager.Instance.Player.SetActive(true);
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
                    SceneInfoManager.Instance.PlayLoadScene();
                    if (Time.timeScale == 0f)
                        Time.timeScale = 1f;
                });
        }
    }
}
