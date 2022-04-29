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
        /// <summary>レベルデザイン</summary>
        [SerializeField] private GameObject levelDesign;
        /// <summary>レベルデザイン</summary>
        public GameObject LevelDesign => levelDesign;
        /// <summary>レベルデザインオブジェクト名</summary>
        private static readonly string OBJECT_NAME_LEVELDESIGN = "LevelDesign";
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
        /// <summary>シーンマップ</summary>
        public SceneIdCrumb SceneIdCrumb => _sceneIdCrumb;
        /// <summary>読み込ませるシーン</summary>
        private string _loadSceneName;
        /// <summary>読み込ませるステージID</summary>
        private int _loadSceneId;
        /// <summary>読み込ませるステージID</summary>
        public int LoadSceneId => _loadSceneId;

        private void Reset()
        {
            if (bgmPlay == null)
                bgmPlay = GameObject.Find(OBJECT_NAME_BGMPLAY);
            if (levelDesign == null)
                levelDesign = GameObject.Find(OBJECT_NAME_LEVELDESIGN);
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

        // ▼▼▼テスト用 結合時には消す▼▼▼
        [SerializeField, Range(0, 29)] private int DemoUpdateScenesMap = 0;
        // ▲▲▲テスト用 結合時には消す▲▲▲

        private void Start()
        {
            // ▼▼▼テスト用 結合時には消す▼▼▼
            UpdateScenesMap(DemoUpdateScenesMap);
            // ▲▲▲テスト用 結合時には消す▲▲▲
            if (!StartStage())
                Debug.LogError("ステージ開始処理の失敗");
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
                GameManager.Instance.SpaceManager.transform.parent = stage.transform;
                GameManager.Instance.SpaceManager.transform.localPosition = Vector3.zero;
                GameManager.Instance.MainCamera.transform.parent = stage.transform;
                GameManager.Instance.MainCamera.transform.localPosition = cameraTransformLocalPoses[_sceneIdCrumb.Current];
                GameManager.Instance.MainCamera.transform.localEulerAngles = cameraTransformLocalAngles[_sceneIdCrumb.Current];
                GameManager.Instance.MainCamera.transform.localScale = cameraTransformLocalScales[_sceneIdCrumb.Current];
                GameManager.Instance.MainCamera.GetComponent<Camera>().fieldOfView = fieldOfViews[_sceneIdCrumb.Current];
                bgmPlay.GetComponent<BgmPlay>().PlayBGM(playBgmNames[_sceneIdCrumb.Current]);
                FinalStage = finalStages[_sceneIdCrumb.Current];
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
            if (!GameManager.Instance.SpaceManager.GetComponent<SpaceManager>().PlayManualStartFromSceneInfoManager())
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
            if (!LevelDesisionIsObjected.LoadObjectOffset(stage, GameManager.Instance.PlayerOffsets))
                Debug.LogError("プレイヤーリセット処理の失敗");
            if (!LevelDesisionIsObjected.LoadObjectOffset(stage, GameManager.Instance.SpaceManager.GetComponent<SpaceManager>().CubeOffsets))
                Debug.LogError("空間操作オブジェクトリセット処理の失敗");
            // T.B.D 敵ギミックの仮実装
            //if (!LevelDesisionIsObjected.ResetObjectFromSceneInfoManager(stage, GameManager.Instance.HumanEnemieOffsets))
            //    Debug.LogError("敵オブジェクトリセット処理の失敗");
            // T.B.D ぼろいブロックの仮実装
            //if (!LevelDesisionIsObjected.ResetObjectFromSceneInfoManager(stage, GameManager.Instance.BrokenCubeOffsets))
            //    Debug.LogError("ぼろいブロックリセット処理の失敗");
            stage.SetActive(false);
            return true;
        }

        /// <summary>
        /// シーン遷移情報マップを更新する
        /// </summary>
        /// <param name="sceneID">現在のシーン名</param>
        public void UpdateScenesMap(int sceneID)
        {
            Debug.Log("シーンIDの更新:[" + sceneID + "]");
            _sceneIdCrumb.Current = sceneID;
            // 次のシーン情報をシーン一覧から検索してセット
            if (_sceneIdCrumb.Current < STAGE_COUNT_MAX/* - 1*/)
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
        public void SetSceneIdUndo()
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
