using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Main.Audio;
using Main.Level;
using Main.UI;
using Main.Common.LevelDesign;

namespace Main.Common
{
    /// <summary>
    /// シーン管理クラス
    /// </summary>
    public class SceneInfoManager : MonoBehaviour
    {
        /// <summary>BGMのリソース管理プレハブ</summary>
        [SerializeField] private GameObject bgmPlay;
        /// <summary>BGMのリソース管理オブジェクト名</summary>
        private static readonly string OBJECT_NAME_BGMPLAY = "BgmPlay";
        /// <summary>最終ステージのフラグ</summary>
        public bool FinalStage { get; set; } = false;
        /// <summary>セーフゾーンのリソース管理プレハブ</summary>
        [SerializeField] private GameObject safeZone;
        /// <summary>セーフゾーンのリソース管理オブジェクト名</summary>
        private static readonly string OBJECT_NAME_SAFEZONE = "SafeZone";
        /// <summary>レベルデザイン</summary>
        [SerializeField] private GameObject levelDesign;
        /// <summary>レベルデザイン</summary>
        public GameObject LevelDesign => levelDesign;
        /// <summary>レベルデザインオブジェクト名</summary>
        private static readonly string OBJECT_NAME_LEVELDESIGN = "LevelDesign";
        /// <summary>空間操作</summary>
        [SerializeField] private GameObject spaceManager;
        /// <summary>空間操作オブジェクト名</summary>
        private static readonly string OBJECT_NAME_SPACEMANAGER = "SpaceManager";
        /// <summary>ステージごとの空間操作範囲</summary>
        [SerializeField] private Vector4[] stageScaleMaxDistances;
        /// <summary>プレイヤーのローカル位置</summary>
        [SerializeField] private Vector3[] playerTransformLocalPoses;
        /// <summary>プレイヤーのローカル角度</summary>
        [SerializeField] private Vector3[] playerTransformLocalAngles;
        /// <summary>プレイヤーのローカルスケール</summary>
        [SerializeField] private Vector3[] playerTransformLocalScales;
        /// <summary>カメラのローカル位置</summary>
        [SerializeField] private Vector3[] cameraTransformLocalPoses;
        /// <summary>カメラのローカル角度</summary>
        [SerializeField] private Vector3[] cameraTransformLocalAngles;
        /// <summary>カメラのローカルスケール</summary>
        [SerializeField] private Vector3[] cameraTransformLocalScales;
        /// <summary>カメラの視界範囲</summary>
        [SerializeField] private float[] fieldOfViews;
        /// <summary>BGMを鳴らす番号</summary>
        [SerializeField] private ClipToPlayBGM[] playBgmNames;
        /// <summary>最終ステージか否か</summary>
        [SerializeField] private bool[] finalStages;
        /// <summary>セーフゾーンの中心</summary>
        [SerializeField] private Vector3[] safeZoneBoxCenters;
        /// <summary>セーフゾーンのサイズ</summary>
        [SerializeField] private Vector3[] safeZoneBoxSizes;
        /// <summary>セーフゾーンの位置</summary>
        [SerializeField] private Vector3[] safeZoneInsPoses;
        /// <summary>最大ステージ数</summary>
        private static readonly int STAGE_COUNT_MAX = 30;
        /// <summary>メインシーンのシーン名</summary>
        private static readonly string SCENE_NAME_MAIN = "Main_Scene";
        /// <summary>セレクトシーンのシーン名</summary>
        private static readonly string SCENE_NAME_SELECT = "SelectScene";

        private static SceneInfoManager instance;
        public static SceneInfoManager Instance { get { return instance; } }
        /// <summary>シーンマップ</summary>
        private SceneIdCrumb _sceneIdCrumb;
        /// <summary>読み込ませるシーン</summary>
        private string _loadSceneName;
        /// <summary>読み込ませるステージID</summary>
        private int _loadSceneId;

        private void Reset()
        {
            if (bgmPlay == null)
                bgmPlay = GameObject.Find(OBJECT_NAME_BGMPLAY);
            if (safeZone == null)
                safeZone = GameObject.Find(OBJECT_NAME_SAFEZONE);
            if (levelDesign == null)
                levelDesign = GameObject.Find(OBJECT_NAME_LEVELDESIGN);
            if (spaceManager == null)
                spaceManager = GameObject.Find(OBJECT_NAME_SPACEMANAGER);
        }

        private void Awake()
        {
            if (null != instance)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            instance = this;
        }

        private void Start()
        {
            if (!StartStage())
                Debug.LogError("ステージ開始処理の失敗");
        }

        [SerializeField] private float testTime;
        private void Update()
        {
            testTime = Time.time;
        }

        /// <summary>
        /// ステージ読み込みの設定
        /// </summary>
        public bool StartStage()
        {
            try
            {
                var stage = levelDesign.transform.GetChild(_sceneIdCrumb.Current).gameObject;
                stage.SetActive(true);
                spaceManager.transform.parent = stage.transform;
                if (!GameManager.Instance.SetStageScaleMaxDistanceFromSpaceManager(stageScaleMaxDistances[_sceneIdCrumb.Current]))
                    Debug.LogError("ステージの空間操作範囲設定の失敗");
                GameManager.Instance.Player.transform.parent = stage.transform;
                GameManager.Instance.Player.transform.localPosition = playerTransformLocalPoses[_sceneIdCrumb.Current];
                GameManager.Instance.Player.transform.localEulerAngles = playerTransformLocalAngles[_sceneIdCrumb.Current];
                GameManager.Instance.Player.transform.localScale = playerTransformLocalScales[_sceneIdCrumb.Current];
                GameManager.Instance.MainCamera.transform.parent = stage.transform;
                GameManager.Instance.MainCamera.transform.localPosition = cameraTransformLocalPoses[_sceneIdCrumb.Current];
                GameManager.Instance.MainCamera.transform.localEulerAngles = cameraTransformLocalAngles[_sceneIdCrumb.Current];
                GameManager.Instance.MainCamera.transform.localScale = cameraTransformLocalScales[_sceneIdCrumb.Current];
                GameManager.Instance.MainCamera.GetComponent<Camera>().fieldOfView = fieldOfViews[_sceneIdCrumb.Current];
                bgmPlay.GetComponent<BgmPlay>().PlayBGM(playBgmNames[_sceneIdCrumb.Current]);
                FinalStage = finalStages[_sceneIdCrumb.Current];
                safeZone.GetComponent<SafeZone>().BoxCenter = safeZoneBoxCenters[_sceneIdCrumb.Current];
                safeZone.GetComponent<SafeZone>().BoxSize = safeZoneBoxSizes[_sceneIdCrumb.Current];
                safeZone.GetComponent<SafeZone>().InsPosition = safeZoneInsPoses[_sceneIdCrumb.Current];
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 疑似スタートイベント発火
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool PlayManualStartFromSceneInfoManager()
        {
            if (!UIManager.Instance.PlayManualStartFadeScreenFromSceneInfoManager())
                Debug.Log("フェード演出開始処理の失敗");
            if (!spaceManager.GetComponent<SpaceManager>().PlayManualStartFromSceneInfoManager())
                Debug.Log("空間操作開始処理の失敗");
            if (!GameManager.Instance.PlayManualStartFromSceneInfoManager())
                Debug.Log("GameManager開始処理の失敗");
            return true;
        }

        /// <summary>
        /// ステージリセットの設定
        /// </summary>
        private bool EndStage()
        {
            // T.B.D 該当ステージプレハブ内の情報をリセットする
            var stage = levelDesign.transform.GetChild(_sceneIdCrumb.Current).gameObject;
            if (!LevelDesisionIsObjected.ResetObjectFromSceneInfoManager(stage, spaceManager.GetComponent<SpaceManager>().CubeOffsets))
                Debug.LogError("空間操作オブジェクトリセット処理の失敗");
            // T.B.D 敵ギミックの仮実装
            //if (!LevelDesisionIsObjected.ResetObjectFromSceneInfoManager(stage, GameManager.Instance.HumanEnemieOffsets))
            //    Debug.LogError("敵オブジェクトリセット処理の失敗");
            stage.SetActive(false);
            return true;
        }

        /// <summary>
        /// シーン遷移情報マップを更新する
        /// </summary>
        /// <param name="sceneID">現在のシーン名</param>
        public void UpdateScenesMap(int sceneID)
        {
            _sceneIdCrumb.Current = sceneID;
            // 次のシーン情報をシーン一覧から検索してセット
            if (_sceneIdCrumb.Current < STAGE_COUNT_MAX - 1)
            {
                // 次のシーンが存在する場合はセット
                _sceneIdCrumb.Next = _sceneIdCrumb.Current + 1;
            }
            else
            {
                // 最終ステージシーンならひとまず現在のシーンをセット
                _sceneIdCrumb.Next = _sceneIdCrumb.Current;
            }
        }

        /// <summary>
        /// 現在のシーンを次のシーンへセット
        /// </summary>
        public void SetSceneIdRedo()
        {
            _loadSceneId = _sceneIdCrumb.Current;
        }

        /// <summary>
        /// 次のシーンを次のシーンへセット
        /// </summary>
        public void SetSceneIdNext()
        {
            _loadSceneId = _sceneIdCrumb.Next;
        }

        /// <summary>
        /// メインシーンを次のシーンへセット
        /// SelectSceneから呼び出される想定の処理
        /// </summary>
        /// <param name="sceneId"></param>
        public void SetMainSceneNameIdFromSelect_Scene(int sceneId)
        {
            _loadSceneName = SCENE_NAME_MAIN;
            _loadSceneId = sceneId;
        }

        /// <summary>
        /// セレクトシーンを次のシーンへセット
        /// Main_Sceneから呼び出される想定の処理
        /// </summary>
        public void SetSelectSceneNameIdFromMain_Scene()
        {
            _loadSceneName = SCENE_NAME_SELECT;
            BrideScenes_SelectMain.Instance.LoadSceneId = _sceneIdCrumb.Current;
        }

        /// <summary>
        /// シーンロード開始
        /// </summary>
        public SceneLoadType PlayLoadScene()
        {
            if (!string.IsNullOrEmpty(_loadSceneName))
            {
                SceneManager.sceneLoaded += LoadedGameScene;
                SceneManager.LoadScene(_loadSceneName);
                return SceneLoadType.SceneLoad;
            }
            else
            {
                // メインシーン間でのステージ遷移
                return EndStage() ? SceneLoadType.PrefabLoad : SceneLoadType.Error;
            }
        }

        /// <summary>
        /// 次のシーンにあるオブジェクトへ値を渡す
        /// </summary>
        /// <param name="next"></param>
        /// <param name="mode"></param>
        private void LoadedGameScene(Scene next, LoadSceneMode mode)
        {
            UpdateScenesMap(_loadSceneId);
            // シーン移動の度に実行されないように消す
            SceneManager.sceneLoaded -= LoadedGameScene;
        }
    }

    /// <summary>
    /// シーン遷移管理のパンくず
    /// </summary>
    public struct SceneIdCrumb
    {
        /// <summary>現在のメインシーンのステージID</summary>
        public int Current { get; set; }
        /// <summary>次のメインシーンのステージID</summary>
        public int Next { get; set; }
    }

    /// <summary>
    /// シーン読み込みタイプ
    /// </summary>
    public enum SceneLoadType
    {
        /// <summary>シーン遷移</summary>
        SceneLoad
            /// <summary>プレハブ化された疑似シーン遷移（メインシーンのみ）</summary>
            , PrefabLoad
            /// <summary>エラー</summary>
            , Error
    }
}
