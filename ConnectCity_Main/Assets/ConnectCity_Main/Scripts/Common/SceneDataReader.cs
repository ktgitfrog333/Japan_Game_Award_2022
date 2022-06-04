using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Main.Common
{
    /// <summary>
    /// シーンデータの読み込みとオブジェクトへ反映
    /// </summary>
    [RequireComponent(typeof(SceneInfoManager))]
    public class SceneDataReader : MonoBehaviour
    {
        private void Reset()
        {
            //if (!LoadSceneConfiguration(GetComponent<SceneInfoManager>()))
            //    Debug.Log("読み込みエラー");
        }

        private bool LoadSceneConfiguration(SceneInfoManager sceneInfoManager)
        {
            try
            {
                var csvDatas = new List<string[]>();
                var ta = Resources.Load("SceneConfigData") as TextAsset;
                var sr = new StringReader(ta.text);

                while (sr.Peek() != -1)
                {
                    var l = sr.ReadLine();
                    csvDatas.Add(l.Split(','));
                }

                for (var i = 0; i < csvDatas.Count; i++)
                {
                    Debug.Log(i);
                    if (i == 0)
                        // タイトル行はスキップ
                        continue;
                    if (!sceneInfoManager.SetSceneConfig(i, csvDatas[i]))
                        Debug.LogError("シーン設定追加の失敗");
                    //var data = csvDatas[i];
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
