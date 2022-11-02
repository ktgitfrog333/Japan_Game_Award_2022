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

        /// <summary>
        /// 最終ステージフラグを更新
        /// ステージ単位で変更
        /// </summary>
        /// <param name="index">ステージ番号</param>
        /// <param name="value">値</param>
        /// <returns>成功／失敗</returns>
        public bool UpdateFinalStage(int index, bool value)
        {
            try
            {
                finalStages[index] = value;
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// コネクトクリア回数を更新
        /// </summary>
        /// <param name="chgCounters">コネクトクリア回数（変更配列）</param>
        /// <returns>成功／失敗</returns>
        public bool UpdateClearConnectedCounters(int[] chgCounters)
        {
            try
            {
                if (chgCounters != null && 0 < chgCounters.Length)
                {
                    for (var i = 0; i < chgCounters.Length; i++)
                    {
                        clearConnectedCounters[i] = chgCounters[i];
                    }
                }
                else
                    throw new System.Exception("データが空");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
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
                return GameManager.Instance.EndStage() ? SceneLoadType.PrefabLoad : SceneLoadType.Error;
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
