using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Main.Audio;
using Main.Level;
using Main.UI;
using Main.Common.LevelDesign;
using Gimmick;

namespace Main.Common
{
    /// <summary>
    /// シーン管理クラス
    /// </summary>
    public class SceneOwner : MonoBehaviour
    {
        /// <summary>最終ステージのフラグ</summary>
        public bool FinalStage { get; set; } = false;
        /// <summary>カメラのローカル位置</summary>
        [SerializeField] private Vector3[] cameraTransformLocalPoses;
        /// <summary>カメラのローカル位置</summary>
        public Vector3[] CameraTransformLocalPoses => cameraTransformLocalPoses;
        /// <summary>カメラのローカル角度</summary>
        [SerializeField] private Vector3[] cameraTransformLocalAngles;
        /// <summary>カメラのローカル角度</summary>
        public Vector3[] CameraTransformLocalAngles => cameraTransformLocalAngles;
        /// <summary>カメラのローカルスケール</summary>
        [SerializeField] private Vector3[] cameraTransformLocalScales;
        /// <summary>カメラのローカルスケール</summary>
        public Vector3[] CameraTransformLocalScales => cameraTransformLocalScales;
        /// <summary>カメラの視界範囲</summary>
        [SerializeField] private float[] fieldOfViews;
        /// <summary>カメラの視界範囲</summary>
        public float[] FieldOfViews => fieldOfViews;
        /// <summary>BGMを鳴らす番号</summary>
        [SerializeField] private ClipToPlayBGM[] playBgmNames;
        /// <summary>BGMを鳴らす番号</summary>
        public ClipToPlayBGM[] PlayBgmNames => playBgmNames;
        /// <summary>最終ステージか否か</summary>
        [SerializeField] private bool[] finalStages;
        /// <summary>最終ステージか否か</summary>
        public bool[] FinalStages => finalStages;
        /// <summary>ステージごとのSkybox</summary>
        [SerializeField] private RenderSettingsSkybox[] skyboxs;
        /// <summary>ステージごとのSkybox</summary>
        public RenderSettingsSkybox[] Skyboxs => skyboxs;
        /// <summary>ゴールポイント解放となるコネクト回数</summary>
        [SerializeField] private int[] clearConnectedCounters;
        /// <summary>ゴールポイント解放となるコネクト回数</summary>
        public int[] ClearConnectedCounters => clearConnectedCounters;
        /// <summary>ゴールポイント解放となるコネクト回数</summary>
        public int ClearConnectedCounter { get; set; }
        /// <summary>最大ステージ数</summary>
        [SerializeField] private int stageCountMax = 30;
        /// <summary>最大ステージ数</summary>
        public int StageCountMax => stageCountMax;
        /// <summary>メインシーンのシーン名</summary>
        private static readonly string SCENE_NAME_MAIN = "Main_Scene";
        /// <summary>セレクトシーンのシーン名</summary>
        private static readonly string SCENE_NAME_SELECT = "SelectScene";

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

        private void Start()
        {
            if (!StartStage())
                Debug.LogError("ステージ開始処理の失敗");
        }

        /// <summary>
        /// シーン設定をセット
        /// </summary>
        /// <param name="index">シーン番号</param>
        /// <param name="datas">１ステージのデータ分</param>
        /// <returns></returns>
        public bool SetSceneConfig(int index, string[] datas)
        {
            try
            {
                for (var j = 0; j < datas.Length; j++)
                {
                    var child = datas[j];
                    switch (j)
                    {
                        case (int)SceneConfigColumns.CameraTransformLocalPoses:
                            cameraTransformLocalPoses[index] = ConvVector3OfString(child);
                            break;
                        case (int)SceneConfigColumns.CameraTransformLocalAngles:
                            cameraTransformLocalAngles[index] = ConvVector3OfString(child);
                            break;
                        case (int)SceneConfigColumns.CameraTransformLocalScales:
                            cameraTransformLocalScales[index] = ConvVector3OfString(child);
                            break;
                        case (int)SceneConfigColumns.FieldOfViews:
                            fieldOfViews[index] = float.Parse(child);
                            break;
                        case (int)SceneConfigColumns.PlayBgnNames:
                            playBgmNames[index] = (ClipToPlayBGM)int.Parse(child);
                            break;
                        case (int)SceneConfigColumns.FinalStages:
                            // 0はfalse　1はtrue
                            finalStages[index] = int.Parse(child) == 1;
                            break;
                        case (int)SceneConfigColumns.Skyboxs:
                            skyboxs[index] = (RenderSettingsSkybox)int.Parse(child);
                            break;
                        case (int)SceneConfigColumns.ClearConnectedCounters:
                            clearConnectedCounters[index] = int.Parse(child);
                            break;
                        default:
                            Debug.LogError("カラム不備");
                            break;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 文字列をベクターに変換して取得
        /// fを消して配列に変換
        /// </summary>
        /// <param name="value">文字列変換データ</param>
        /// <returns>3点ベクター</returns>
        private Vector3 ConvVector3OfString(string value)
        {
            if (value.IndexOf("f") < 0)
                Debug.LogError("フォーマット不正");
            var array = value.Split('f');
            if (array.Length != 4)
                Debug.LogError("フォーマット不正");
            return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
        }

        /// <summary>
        /// ステージ読み込みの設定
        /// </summary>
        public bool StartStage()
        {
            try
            {
                // Skyboxの設定
                if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SetRenderSkyboxFromSceneOwner(skyboxs[_sceneIdCrumb.Current]))
                    Debug.LogError("Skybox設定処理の失敗");
                // スタート演出の間は空間操作は無効
                if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SpaceOwner.GetComponent<SpaceOwner>().InputBan)
                    GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SpaceOwner.GetComponent<SpaceOwner>().InputBan = true;
                // スタート演出の間はショートカット入力は無効
                GameManager.Instance.UIOwner.GetComponent<UIOwner>().ShortcuGuideScreen.GetComponent<ShortcuGuideScreen>().InputBan = true;
                // 読み込むステージのみ有効
                var stage = GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().LevelDesign.transform.GetChild(_sceneIdCrumb.Current).gameObject;
                stage.SetActive(true);
                // コネクトシステムの初期設定
                GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SpaceOwner.transform.parent = stage.transform;
                GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SpaceOwner.transform.localPosition = Vector3.zero;
                if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SpaceOwner.GetComponent<SpaceOwner>().PlayManualStartFromSceneOwner())
                    Debug.Log("空間操作開始処理の失敗");
                if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().TurretEnemiesOwner.GetComponent<TurretEnemiesOwner>().Initialize())
                    Debug.Log("レーザー砲起動処理の失敗");
                if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().RobotEnemiesOwner.GetComponent<RobotEnemiesOwner>().Initialize())
                    Debug.Log("敵起動処理の失敗");
                if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().BreakBlookOwner.GetComponent<BreakBlookOwner>().Initialize())
                    Debug.Log("ぼろいブロック・天井復活処理の失敗");
                // カメラの初期設定
                GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().MainCamera.transform.parent = stage.transform;
                GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().MainCamera.transform.localPosition = cameraTransformLocalPoses[_sceneIdCrumb.Current];
                GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().MainCamera.transform.localEulerAngles = cameraTransformLocalAngles[_sceneIdCrumb.Current];
                GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().MainCamera.transform.localScale = cameraTransformLocalScales[_sceneIdCrumb.Current];
                GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().MainCamera.GetComponent<Camera>().fieldOfView = fieldOfViews[_sceneIdCrumb.Current];
                // BGMの初期設定
                GameManager.Instance.AudioOwner.GetComponent<AudioOwner>().PlayBGM(playBgmNames[_sceneIdCrumb.Current]);
                // 最終ステージか否かの判断（クリア画面のUIに影響）
                FinalStage = finalStages[_sceneIdCrumb.Current];
                // コネクト回数
                ClearConnectedCounter = clearConnectedCounters[_sceneIdCrumb.Current];
                if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().InitializeGoalPoint())
                    Debug.LogError("ゴールポイント初期化の失敗");
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
        public bool PlayManualStartFromSceneOwner()
        {
            if (!GameManager.Instance.UIOwner.GetComponent<UIOwner>().PlayManualStartFadeScreenFromSceneOwner())
                Debug.Log("フェード演出開始処理の失敗");
            if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().PlayManualStartFromSceneOwner())
                Debug.Log("LevelOwner開始処理の失敗");
            return true;
        }

        /// <summary>
        /// ステージリセットの設定
        /// </summary>
        private bool EndStage()
        {
            // 該当ステージプレハブ内の情報をリセットする
            var stage = GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().LevelDesign.transform.GetChild(_sceneIdCrumb.Current).gameObject;
            if (!LevelDesisionIsObjected.LoadObjectOffset(stage, GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().PlayerOffsets))
                Debug.LogError("プレイヤーリセット処理の失敗");
            if (!LevelDesisionIsObjected.LoadObjectOffset(stage, GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SpaceOwner.GetComponent<SpaceOwner>().CubeOffsets))
                Debug.LogError("空間操作オブジェクトリセット処理の失敗");
            GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().SpaceOwner.GetComponent<SpaceOwner>().DisposeAllFromSceneOwner();
            if (!LevelDesisionIsObjected.LoadObjectOffset(stage, GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().RobotEnemiesOwner.GetComponent<RobotEnemiesOwner>().RobotEmemOffsets))
                Debug.Log("敵オブジェクトリセット処理の失敗");
            if (!GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().TurretEnemiesOwner.GetComponent<TurretEnemiesOwner>().OnImitationDestroy())
                Debug.Log("レーザー砲終了処理の失敗");
            // ぼろいブロック・天井の監視を終了
            GameManager.Instance.LevelOwner.GetComponent<LevelOwner>().DisposeAllBreakBlooksFromSceneOwner();
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
            if (_sceneIdCrumb.Current < stageCountMax/* - 1*/)
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
