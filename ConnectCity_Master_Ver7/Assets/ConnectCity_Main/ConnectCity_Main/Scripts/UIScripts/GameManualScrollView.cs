using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using Main.Audio;

namespace Main.UI
{
    /// <summary>
    /// 操作説明
    /// 遊び方の確認を選択した際に表示される
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class GameManualScrollView : MonoBehaviour
    {
        /// <summary>ScrollRectコンポーネント</summary>
        [SerializeField] private ScrollRect scrollRect;
        /// <summary>選択項目のUIスクリプト</summary>
        [SerializeField] private GameManualViewPageUIController firstElement;
        /// <summary>選択項目のUIオブジェクト</summary>
        [SerializeField] private GameObject firstObject;
        /// <summary>イベントシステム</summary>
        [SerializeField] private EventSystem @event;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (!@event)
                @event = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            @event.SetSelectedGameObject(firstObject);
            firstElement.SelectSEMute = true;
            firstElement.Selected();
            Time.timeScale = 0f;
        }

        private void Reset()
        {
            Initialize();
        }

        private void Start()
        {
            Initialize();
            // マウスボタン・キャンセルボタンが押されたら画面を閉じる
            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButtonDown(0) ||
                    Input.GetMouseButtonDown(1) ||
                    Input.GetMouseButtonDown(2))
                .Subscribe(_ =>
                {
                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_cancel);
                    UIManager.Instance.CloseManual();
                });
        }

        private void Initialize()
        {
            if (!scrollRect)
                scrollRect = GetComponent<ScrollRect>();
            var g = transform.GetChild(0).GetChild(0).GetChild(0);
            if (!firstElement)
                firstElement = g.GetComponent<GameManualViewPageUIController>();
            if (!firstObject)
                firstObject = g.gameObject;
        }

        /// <summary>
        /// ページをスクロールする
        /// </summary>
        /// <param name="pageIndex">ページ番号</param>
        public void ScrollPage(int pageIndex)
        {
            switch (pageIndex)
            {
                case 0:
                    if (!scrollRect.horizontalNormalizedPosition.Equals(0f))
                        _ = PlayScrollMotion(0f);
                    break;
                case 1:
                    if (!scrollRect.horizontalNormalizedPosition.Equals(.3325f))
                        _ = PlayScrollMotion(.3325f);
                    break;
                case 2:
                    if (!scrollRect.horizontalNormalizedPosition.Equals(.666f))
                        _ = PlayScrollMotion(.666f);
                    break;
                case 3:
                    if (!scrollRect.horizontalNormalizedPosition.Equals(1f))
                        _ = PlayScrollMotion(1f);
                    break;
                default:
                    if (!scrollRect.horizontalNormalizedPosition.Equals(0f))
                        _ = PlayScrollMotion(0f);
                    break;
            }
        }

        /// <summary>
        /// 1ページ目にリセット
        /// </summary>
        public void ResetPage()
        {
            scrollRect.horizontalNormalizedPosition = 0f;
        }

        /// <summary>
        /// 流れるようにスクロールさせる
        /// </summary>
        /// <param name="target">ターゲットのスクロール位置</param>
        /// <returns>処理完了待機</returns>
        public async Task<bool> PlayScrollMotion(float target)
        {
            var s = scrollRect.horizontalNormalizedPosition;
            var add = (target - s) / 10;
            for (var i = 0; i < 10; i++)
            {
                await Task.Delay(10);
                scrollRect.horizontalNormalizedPosition += add;
            }
            scrollRect.horizontalNormalizedPosition = target;
            return true;
        }
    }
}
