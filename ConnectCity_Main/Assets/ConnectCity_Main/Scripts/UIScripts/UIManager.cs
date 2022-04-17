using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using Main.Common.Const;
using Main.Audio;
using Main.Common;

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
        [SerializeField] private GameObject fadeScreen;
        /// <summary>ロード演出のオブジェクト名</summary>
        private static readonly string OBJECT_NAME_FADESCREEN = "FadeScreen";
        /// <summary>ポーズ画面</summary>
        [SerializeField] private GameObject pauseScreen;
        /// <summary>ポーズ画面のオブジェクト名</summary>
        private static readonly string OBJECT_NAME_PAUSESCREEN = "PauseScreen";
        /// <summary>遊び方の確認</summary>
        [SerializeField] private GameObject gameManualScrollView;
        /// <summary>遊び方の確認のオブジェクト名</summary>
        private static readonly string OBJECT_NAME_GAMEMANUALSCROLLVIEW = "GameManualScrollView";
        /// <summary>遊び方の確認</summary>
        [SerializeField] private GameObject clearScreen;
        /// <summary>遊び方の確認のオブジェクト名</summary>
        private static readonly string OBJECT_NAME_CLEARSCREEN = "ClearScreen";
        /// <summary>ショートカット入力</summary>
        [SerializeField] private GameObject shortcuGuideScreen;
        /// <summary>ショートカット入力</summary>
        private static readonly string OBJECT_NAME_SHORTCUGUIDESCREEN = "ShortcuGuideScreen";

        private void Reset()
        {
            if (spaceScreen == null)
                spaceScreen = GameObject.Find(OBJECT_NAME_SPACESCREEN);
            if (fadeScreen == null)
                fadeScreen = GameObject.Find(OBJECT_NAME_FADESCREEN);
            if (pauseScreen == null)
                pauseScreen = GameObject.Find(OBJECT_NAME_PAUSESCREEN);
            if (gameManualScrollView == null)
                gameManualScrollView = GameObject.Find(OBJECT_NAME_GAMEMANUALSCROLLVIEW);
            if (clearScreen == null)
                clearScreen = GameObject.Find(OBJECT_NAME_CLEARSCREEN);
            if (shortcuGuideScreen == null)
                shortcuGuideScreen = GameObject.Find(OBJECT_NAME_SHORTCUGUIDESCREEN);
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
                    !clearScreen.activeSelf &&
                    !pauseScreen.activeSelf)
                .Subscribe(_ =>
                {
                    pauseScreen.SetActive(true);
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
            pauseScreen.SetActive(false);
            if (Time.timeScale == 0f)
                Time.timeScale = 1f;
        }

        /// <summary>
        /// 遊び方確認を閉じる
        /// </summary>
        public async void CloseManual()
        {
            await Task.Delay(500);
            GameManualScrollViewResetFromUIManager();
            GameManualScrollViewSetActiveFromUIManager(false);
            if (Time.timeScale == 0f)
                Time.timeScale = 1f;
        }

        /// <summary>
        /// 遊び方の確認を有効にする
        /// </summary>
        /// <param name="active">有効／無効</param>
        public void GameManualScrollViewSetActiveFromUIManager(bool active)
        {
            gameManualScrollView.SetActive(active);
        }

        /// <summary>
        /// 遊び方の確認を選択した際に表示される
        /// 1ページ目にリセット
        /// </summary>
        public void GameManualScrollViewResetFromUIManager()
        {
            gameManualScrollView.GetComponent<GameManualScrollView>().ResetPage();
        }

        /// <summary>
        /// 遊び方の確認を選択した際に表示される
        /// 1ページ目にリセット
        /// </summary>
        public void GameManualScrollViewScrollPageFromUIManager(int pageIndex)
        {
            gameManualScrollView.GetComponent<GameManualScrollView>().ScrollPage(pageIndex);
        }

        /// <summary>
        /// クリア画面を開く
        /// </summary>
        public async void OpenClearScreen()
        {
            clearScreen.SetActive(true);

            // 子オブジェクトは一度非表示にする
            for (int i = 1; i < clearScreen.transform.GetChild(0).childCount; i++)
                clearScreen.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
            await Task.Delay(3000);
            // 子オブジェクトは一度非表示にする
            for (int i = 1; i < clearScreen.transform.GetChild(0).childCount - (SceneInfoManager.Instance.FinalStage ? 1 : 0); i++)
                clearScreen.transform.GetChild(0).GetChild(i).gameObject.SetActive(true);

            clearScreen.GetComponent<ClearScreen>().AutoSelectContent();
        }

        public async void CloseClearScreen()
        {
            await Task.Delay(500);
            clearScreen.SetActive(false);
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
            fadeScreen.SetActive(true);
            fadeScreen.GetComponent<FadeScreen>().DrawLoadNowFadeOut();

            return true;
        }

        /// <summary>
        /// フェード演出UIのスタートイベント内の処理を疑似発火
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool PlayManualStartFadeScreenFromSceneInfoManager()
        {
            fadeScreen.GetComponent<FadeScreen>().ManualStart();
            return true;
        }
    }
}
