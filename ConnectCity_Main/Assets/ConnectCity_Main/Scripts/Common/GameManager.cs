using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Audio;
using Main.UI;
using Main.Level;
using Gimmick;
using Main.Common.LevelDesign;
using Main.InputSystem;

namespace Main.Common
{
    /// <summary>
    /// ゲームマネージャー
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>シーンのオーナー</summary>
        [SerializeField] private GameObject sceneOwner;
        /// <summary>シーンのオーナー</summary>
        public GameObject SceneOwner => sceneOwner;
        /// <summary>オーディオのオーナー</summary>
        [SerializeField] private GameObject audioOwner;
        /// <summary>オーディオのオーナー</summary>
        public GameObject AudioOwner => audioOwner;
        /// <summary>UIのオーナー</summary>
        [SerializeField] private GameObject uIOwner;
        /// <summary>UIのオーナー</summary>
        public GameObject UIOwner => uIOwner;
        /// <summary>レベルデザインのオーナー</summary>
        [SerializeField] private GameObject levelOwner;
        /// <summary>レベルデザインのオーナー</summary>
        public GameObject LevelOwner => levelOwner;
        /// <summary>InputSystemのオーナー</summary>
        [SerializeField] private GameObject inputSystemsOwner;
        /// <summary>InputSystemのオーナー</summary>
        public GameObject InputSystemsOwner => inputSystemsOwner;
        /// <summary>チュートリアルのオーナー</summary>
        [SerializeField] private GameObject tutorialOwner;
        /// <summary>チュートリアルのオーナー</summary>
        public GameObject TutorialOwner => tutorialOwner;

        /// <summary>クラス自身</summary>
        private static GameManager instance;
        /// <summary>シングルトンのインスタンス</summary>
        public static GameManager Instance => instance;

        private void Awake()
        {
            instance = this;
            if (!audioOwner.GetComponent<AudioOwner>().Initialize())
                Debug.LogError("オーディオ初期処理の失敗");
        }

        private void Reset()
        {
            if (sceneOwner == null)
                sceneOwner = GameObject.Find("SceneOwner");
            if (audioOwner == null)
                audioOwner = GameObject.Find("AudioOwner");
            if (uIOwner == null)
                uIOwner = GameObject.Find("UIOwner");
            if (levelOwner == null)
                levelOwner = GameObject.Find("LevelOwner");
            if (inputSystemsOwner == null)
                inputSystemsOwner = GameObject.Find("InputSystemsOwner");
            if (tutorialOwner == null)
                tutorialOwner = GameObject.Find("TutorialOwner");
        }

        private void Start()
        {
            if (!StartStage())
                Debug.LogError("ステージ読み込みの設定の失敗");
            if (!uIOwner.GetComponent<UIOwner>().Initialize())
                Debug.LogError("UI初期処理の失敗");
            if (!uIOwner.GetComponent<UIOwner>().FadeScreenPlayFadeInAndStartCutScene())
                Debug.LogError("フェードイン処理の失敗");
            if (!levelOwner.GetComponent<LevelOwner>().Initialize())
                Debug.LogError("レベルデザイン初期処理の失敗");
            if (!levelOwner.GetComponent<LevelOwner>().ManualStart())
                Debug.LogError("レベルデザイン疑似スタートの失敗");
            if (!inputSystemsOwner.GetComponent<InputSystemsOwner>().Initialize())
                Debug.LogError("インプット初期処理の失敗");
            if (!tutorialOwner.GetComponent<TutorialOwner>().Initialize())
                Debug.LogError("チュートリアル初期処理の失敗");
            if (!tutorialOwner.GetComponent<TutorialOwner>().ManualStart())
                Debug.LogError("チュートリアル疑似スタートの失敗");
        }

        /// <summary>
        /// リスタート処理
        /// </summary>
        public void ReStart()
        {
            var compSceneOwner = sceneOwner.GetComponent<SceneOwner>();
            var compLevelOwner = levelOwner.GetComponent<LevelOwner>();
            var compUIOwner = uIOwner.GetComponent<UIOwner>();
            var compTutorialOwner = tutorialOwner.GetComponent<TutorialOwner>();
            var current = compSceneOwner.SceneIdCrumb.Current;

            // ゴール演出の後処理
            if (!compUIOwner.DestroyParticleFromFadeScreen())
                Debug.LogError("ゴール演出の後処理の失敗");
            // 同じステージをリロードする場合はスタート演出を短くする
            if (!compUIOwner.SetStartCutsceneContinueFromFadeScreen(compSceneOwner.LoadSceneId == current))
                Debug.LogError("リスタートフラグセットの失敗");
            compSceneOwner.UpdateScenesMap(compSceneOwner.LoadSceneId);
            if (!StartStage())
                Debug.LogError("ステージ読み込みの設定の失敗");
            if (!compUIOwner.FadeScreenPlayFadeInAndStartCutScene())
                Debug.Log("フェード演出開始処理の失敗");
            if (!compLevelOwner.ManualStart())
                Debug.Log("レベルデザイン疑似スタートの失敗");
            if (!inputSystemsOwner.GetComponent<InputSystemsOwner>().Initialize())
                Debug.LogError("インプット初期処理の失敗");
            if (!compTutorialOwner.ManualStart())
                Debug.LogWarning("チュートリアル疑似スタートの失敗");
        }

        /// <summary>
        /// ステージ読み込みの設定
        /// </summary>
        /// <returns>成功／失敗</returns>
        private bool StartStage()
        {
            try
            {
                var compSceneOwner = sceneOwner.GetComponent<SceneOwner>();
                var compLevelOwner = levelOwner.GetComponent<LevelOwner>();
                var compUIOwner = uIOwner.GetComponent<UIOwner>();
                var current = compSceneOwner.SceneIdCrumb.Current;

                // スタート演出の間はショートカット入力は無効
                compUIOwner.SetShortcuGuideScreenInputBan(true);
                // Skyboxの設定
                if (!compLevelOwner.SetRenderSkybox(compSceneOwner.Skyboxs[current]))
                    Debug.LogError("Skybox設定処理の失敗");
                // スタート演出の間は空間操作は無効
                if (!compLevelOwner.SpaceOwner.GetComponent<SpaceOwner>().InputBan)
                    compLevelOwner.SetSpaceOwnerInputBan(true);
                // 読み込むステージのみ有効
                var stage = compLevelOwner.LevelDesign.transform.GetChild(current).gameObject;
                stage.SetActive(true);
                // コネクトシステムの初期設定
                compLevelOwner.SpaceOwner.transform.parent = stage.transform;
                compLevelOwner.SpaceOwner.transform.localPosition = Vector3.zero;
                if (!compLevelOwner.SpaceOwner.GetComponent<SpaceOwner>().Initialize())
                    Debug.LogError("空間操作開始処理の失敗");
                if (!compLevelOwner.TurretEnemiesOwner.GetComponent<TurretEnemiesOwner>().Initialize())
                    Debug.Log("レーザー砲起動処理の失敗");
                if (!compLevelOwner.RobotEnemiesOwner.GetComponent<RobotEnemiesOwner>().Initialize())
                    Debug.Log("敵起動処理の失敗");
                if (!compLevelOwner.BreakBlookOwner.GetComponent<BreakBlookOwner>().Initialize())
                    Debug.Log("ぼろいブロック・天井復活処理の失敗");
                // カメラの初期設定
                compLevelOwner.MainCamera.transform.parent = stage.transform;
                compLevelOwner.MainCamera.transform.localPosition = compSceneOwner.CameraTransformLocalPoses[current];
                compLevelOwner.MainCamera.transform.localEulerAngles = compSceneOwner.CameraTransformLocalAngles[current];
                compLevelOwner.MainCamera.transform.localScale = compSceneOwner.CameraTransformLocalScales[current];
                compLevelOwner.MainCamera.GetComponent<Camera>().fieldOfView = compSceneOwner.FieldOfViews[current];
                // BGMの初期設定
                audioOwner.GetComponent<AudioOwner>().PlayBGM(compSceneOwner.PlayBgmNames[current]);
                // 最終ステージか否かの判断（クリア画面のUIに影響）
                compSceneOwner.FinalStage = compSceneOwner.FinalStages[current];
                // コネクト回数
                compSceneOwner.ClearConnectedCounter = compSceneOwner.ClearConnectedCounters[current];
                if (!compLevelOwner.GoalPointInitialize())
                    Debug.LogError("ゴールポイント初期化の失敗");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ステージリセットの設定
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool EndStage()
        {
            try
            {
                var compLevelOwner = levelOwner.GetComponent<LevelOwner>();
                var current = sceneOwner.GetComponent<SceneOwner>().SceneIdCrumb.Current;
                var compTutorialOwner = tutorialOwner.GetComponent<TutorialOwner>();

                // 該当ステージプレハブ内の情報をリセットする
                var stage = compLevelOwner.LevelDesign.transform.GetChild(current).gameObject;
                if (!LevelDesisionIsObjected.LoadObjectOffset(stage, compLevelOwner.PlayerOffsets))
                    Debug.LogError("プレイヤーリセット処理の失敗");
                if (!LevelDesisionIsObjected.LoadObjectOffset(stage, compLevelOwner.GoalPointOffsets))
                    Debug.LogError("ゴールリセット処理の失敗");
                if (!compLevelOwner.SpaceOwner.GetComponent<SpaceOwner>().DestroyNewMoveCube())
                    Debug.LogError("後から生成された空間操作オブジェクト削除の失敗");
                if (!LevelDesisionIsObjected.LoadObjectOffset(stage, compLevelOwner.SpaceOwner.GetComponent<SpaceOwner>().CubeOffsets))
                    Debug.LogError("空間操作オブジェクトリセット処理の失敗");
                compLevelOwner.SpaceOwner.GetComponent<SpaceOwner>().DisposeAllFromSceneOwner();
                if (!LevelDesisionIsObjected.LoadObjectOffset(stage, compLevelOwner.RobotEnemiesOwner.GetComponent<RobotEnemiesOwner>().RobotEmemOffsets))
                    Debug.LogWarning("敵オブジェクトリセット処理の失敗");
                if (!LevelDesisionIsObjected.LoadObjectOffset(stage, compLevelOwner.AutoDroneOwner.GetComponent<AutoDroneOwner>().AutoDroneOffsets))
                    Debug.LogWarning("自動追尾ドローンリセット処理の失敗");
                if (!compLevelOwner.TurretEnemiesOwner.GetComponent<TurretEnemiesOwner>().OnImitationDestroy())
                    Debug.LogWarning("レーザー砲終了処理の失敗");
                if (!LevelDesisionIsObjected.LoadObjectOffset(stage, compTutorialOwner.TutorialEnvironment.GetComponent<TutorialEnvironment>().CubeOffsets))
                    Debug.LogWarning("チュートリアルエンバイロメントリセット処理の失敗");
                // ぼろいブロック・天井の監視を終了
                compLevelOwner.DisposeAll();
                if (!levelOwner.GetComponent<LevelOwner>().Exit())
                    Debug.LogError("レベルデザイン終了処理の失敗");
                stage.SetActive(false);
                if (!inputSystemsOwner.GetComponent<InputSystemsOwner>().Exit())
                    Debug.LogError("インプット終了処理の失敗");
                if (!compTutorialOwner.Exit())
                    Debug.LogError("チュートリアル終了処理の失敗");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }

    /// <summary>
    /// GameMangaerのインターフェース
    /// </summary>
    public interface IGameManager
    {
        /// <summary>
        /// 初期処理
        /// </summary>
        public bool Initialize();
        /// <summary>
        /// 終了
        /// </summary>
        public bool Exit();
    }

    /// <summary>
    /// Owner系のインターフェース
    /// </summary>
    public interface IOwner : IGameManager
    {
        /// <summary>
        /// 疑似スタート
        /// </summary>
        public bool ManualStart();
    }
}
