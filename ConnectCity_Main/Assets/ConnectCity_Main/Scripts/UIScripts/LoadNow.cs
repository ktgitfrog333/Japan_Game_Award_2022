using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Main.UI
{
    /// <summary>
    /// UIへフェード演出を入れるスクリプトクラス
    /// </summary>
    public class LoadNow : MonoBehaviour
    {
        private static LoadNow instance;
        public static LoadNow Instance { get { return instance; } }
        /// <summary>位置情報の最低値</summary>
        private static readonly float LOAD_NOW_MIN_POSITION = 0f;
        /// <summary>位置情報の最高値</summary>
        private static readonly float LOAD_NOW_MAX_POSITION = -1920.0f;
        /// <summary>UIの位置情報をキャッシュ</summary>
        private RectTransform _loadNowRect;
        /// <summary>ロード画面を画面外へ移動させる挙動のトリガー</summary>
        public bool DrawLoadNowFadeInTrigger { get; set; }
        /// <summary>ロード画面を画面内へ移動させる挙動のトリガー</summary>
        public bool DrawLoadNowFadeOutTrigger { get; set; }

        private void Awake()
        {
            if (null != instance)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        void Start()
        {
            _loadNowRect = transform as RectTransform;
            _loadNowRect.anchoredPosition = new Vector2(LOAD_NOW_MIN_POSITION, 0f);
            DrawLoadNowFadeInTrigger = true;
            UIManager.Instance.enabled = false;

            this.UpdateAsObservable()
                .Where(_ => DrawLoadNowFadeInTrigger == true)
                .Subscribe(_ => DrawLoadNowFadeIn());
            this.UpdateAsObservable()
                .Where(_ => DrawLoadNowFadeOutTrigger == true)
                .Subscribe(_ => DrawLoadNowFadeOut());
        }

        /// <summary>
        /// フェードイン処理
        /// </summary>
        private void DrawLoadNowFadeIn()
        {
            if (_loadNowRect.anchoredPosition.x >= LOAD_NOW_MAX_POSITION)
            {
                _loadNowRect.anchoredPosition += new Vector2(-1500 * Time.deltaTime, 0);
            }
            else
            {
                _loadNowRect.anchoredPosition = new Vector2(LOAD_NOW_MAX_POSITION, 0);
                DrawLoadNowFadeInTrigger = false;
                ActiveObject();
            }
        }

        /// <summary>
        /// フェードアウト処理
        /// </summary>
        private void DrawLoadNowFadeOut()
        {
            if (_loadNowRect.anchoredPosition.x <= LOAD_NOW_MIN_POSITION)
            {
                _loadNowRect.anchoredPosition += new Vector2(1500 * Time.deltaTime, 0);
            }
            else
            {
                _loadNowRect.anchoredPosition = new Vector2(LOAD_NOW_MIN_POSITION, 0);
                DrawLoadNowFadeOutTrigger = false;
                SceneInfoManager.Instance.PlayLoadScene();
            }
        }

        /// <summary>
        /// タイミングごとにオブジェクトを有効にする
        /// </summary>
        private void ActiveObject()
        {
            UIManager.Instance.enabled = true;
        }
    }
}
