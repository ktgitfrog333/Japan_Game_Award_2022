using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using Main.Common.Const;
using Main.Audio;

namespace Main.UI
{
    /// <summary>
    /// ポーズ画面などのUIを制御する
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;
        public static UIManager Instance { get { return instance; } }

        /// <summary>空間操作可能な境界</summary>
        [SerializeField] private GameObject spaceScreen;
        /// <summary>空間操作可能な境界のオブジェクト名</summary>
        private static readonly string OBJECT_NAME_SPACESCREEN = "SpaceScreen";
        /// <summary>ロード演出</summary>
        [SerializeField] private GameObject loadNow;
        /// <summary>ロード演出のオブジェクト名</summary>
        private static readonly string OBJECT_NAME_LOADNOW = "LoadNow";

        private void Reset()
        {
            if (spaceScreen == null)
                spaceScreen = GameObject.Find(OBJECT_NAME_SPACESCREEN);
            if (loadNow == null)
                loadNow = GameObject.Find(OBJECT_NAME_LOADNOW);
        }

        private void Awake()
        {
            if (null != instance)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        private void Start()
        {
            // ポーズ画面表示の入力（クリア画面の表示中はポーズ画面を有効にしない）
            this.UpdateAsObservable()
                .Where(_ => Input.GetButtonDown(InputConst.INPUT_CONST_MENU) &&
                    !ClearScreen.Instance.gameObject.activeSelf &&
                    !PauseScreen.Instance.gameObject.activeSelf)
                .Subscribe(_ =>
                {
                    PauseScreen.Instance.gameObject.SetActive(true);
                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_menu);
                });
            // 空間操作可能な境界を表示切り替え操作の入力
            this.UpdateAsObservable()
                .Where(_ => Input.GetButtonDown(InputConst.INPUT_CONSTSPACE) &&
                    !spaceScreen.activeSelf)
                .Subscribe(_ =>
                {
                    spaceScreen.SetActive(true);
                    SfxPlay.Instance.PlaySFX(ClipToPlay.se_menu);
                });
        }

        /// <summary>
        /// メニューを閉じる
        /// </summary>
        public async void CloseMenu()
        {
            await Task.Delay(500);
            // T.B.D プレイヤー/ギミックその他のオブジェクトを無効にする
            PauseScreen.Instance.gameObject.SetActive(false);
        }

        /// <summary>
        /// 遊び方確認を閉じる
        /// </summary>
        public async void CloseManual()
        {
            await Task.Delay(500);
            GameManualScrollView.Instance.ResetPage();
            GameManualScrollView.Instance.gameObject.SetActive(false);
            PauseScreen.Instance.AutoSelectContent(PauseActionMode.CheckAction);
        }

        /// <summary>
        /// クリア画面を開く
        /// </summary>
        public async void OpenClearScreen()
        {
            ClearScreen.Instance.gameObject.SetActive(true);

            // 子オブジェクトは一度非表示にする
            for (int i = 1; i < ClearScreen.Instance.transform.GetChild(0).childCount; i++)
                ClearScreen.Instance.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
            await Task.Delay(3000);
            // 子オブジェクトは一度非表示にする
            for (int i = 1; i < ClearScreen.Instance.transform.GetChild(0).childCount; i++)
                ClearScreen.Instance.transform.GetChild(0).GetChild(i).gameObject.SetActive(true);

            ClearScreen.Instance.AutoSelectContent();
        }

        /// <summary>
        /// 空間操作可能な境界を表示／非表示
        /// </summary>
        public async void CloseSpaceScreen()
        {
            await Task.Delay(500);
            spaceScreen.SetActive(false);
        }

        /// <summary>
        /// フェード演出オブジェクトを有効にする
        /// </summary>
        /// <returns></returns>
        public bool EnableDrawLoadNowFadeOutTrigger()
        {
            loadNow.SetActive(true);
            loadNow.GetComponent<LoadNow>().DrawLoadNowFadeOutTrigger = true;

            return true;
        }
    }
}
