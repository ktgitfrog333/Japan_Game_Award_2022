using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace Main.Common
{
    /// <summary>
    /// シーンデータの書き込みとファイル出力
    /// </summary>
    [RequireComponent(typeof(SceneInfoManager))]
    public class SceneDataWritter : MonoBehaviour
    {
        private void Reset()
        {
            var sceneInfoManager = GetComponent<SceneInfoManager>();
            if (!DeleteConfiguration())
                Debug.Log("削除失敗");
            if (!SaveSceneConfiguration(sceneInfoManager))
                Debug.Log("保存失敗");
        }

        /// <summary>
        /// 設定の削除
        /// </summary>
        /// <returns></returns>
        private bool DeleteConfiguration()
        {
            try
            {
                using (var fileStream = new FileStream(@".\Assets\Resources\SceneConfigData.csv", FileMode.Open))
                {
                    fileStream.SetLength(0);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 設定の保存
        /// </summary>
        /// <returns></returns>
        private bool SaveSceneConfiguration(SceneInfoManager sceneInfoManager)
        {
            try
            {
                var sw = new StreamWriter(@".\Assets\Resources\SceneConfigData.csv", true, Encoding.GetEncoding("UTF-8"));
                sw.WriteLine(string.Join(",", GetTitlesRecord()));
                for (var i = 0; i < sceneInfoManager.StageCountMax; i++)
                {
                    sw.WriteLine(string.Join(",", GetRecord(i, sceneInfoManager)));
                }
                sw.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// タイトル箇所のレコードを取得
        /// </summary>
        /// <returns>CSVのタイトル箇所</returns>
        private string[] GetTitlesRecord()
        {
            var scene = new List<string>();
            scene.Add("CameraTransformLocalPoses");
            scene.Add("CameraTransformLocalAngles");
            scene.Add("CameraTransformLocalScales");
            scene.Add("Field of Views");
            scene.Add("PlayBgnNames");
            scene.Add("FinalStages");
            scene.Add("Skyboxs");
            scene.Add("ClearConnectedCounters");
            return scene.ToArray();
        }

        /// <summary>
        /// レコードを取得
        /// </summary>
        /// <param name="index">行番号</param>
        /// <returns>一行分のレコード</returns>
        private string[] GetRecord(int index, SceneInfoManager sceneInfoManager)
        {
            var scene = new List<string>();
            scene.Add(ConvStringOfVector3(sceneInfoManager.CameraTransformLocalPoses[index]));
            scene.Add(ConvStringOfVector3(sceneInfoManager.CameraTransformLocalAngles[index]));
            scene.Add(ConvStringOfVector3(sceneInfoManager.CameraTransformLocalScales[index]));
            scene.Add(sceneInfoManager.FieldOfViews[index] + "");
            scene.Add((int)sceneInfoManager.PlayBgmNames[index] + "");
            scene.Add(sceneInfoManager.FinalStages[index] ? "1" : "0");
            scene.Add((int)sceneInfoManager.Skyboxs[index] + "");
            scene.Add(sceneInfoManager.ClearConnectedCounters[index] + "");
            return scene.ToArray();
        }

        /// <summary>
        /// ベクターを文字列に変換して取得
        /// カンマを消してfに変換
        /// </summary>
        /// <param name="value">3点ベクター</param>
        /// <returns>文字列変換データ</returns>
        private string ConvStringOfVector3(Vector3 value)
        {
            return new StringBuilder(value.x + "f")
                    .Append(value.y + "f")
                    .Append(value.z + "f")
                    .ToString();
        }
    }
}
