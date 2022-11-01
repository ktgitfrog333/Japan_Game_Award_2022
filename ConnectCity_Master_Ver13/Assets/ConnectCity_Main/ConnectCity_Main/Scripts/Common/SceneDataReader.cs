using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Main.Common
{
    /// <summary>
    /// シーンデータの読み込みとオブジェクトへ反映
    /// </summary>
    [RequireComponent(typeof(SceneOwner))]
    public class SceneDataReader : MonoBehaviour
    {
        private void Reset()
        {
            if (!LoadSceneConfiguration(GetComponent<SceneOwner>()))
                Debug.Log("読み込みエラー");
        }

        private bool LoadSceneConfiguration(SceneOwner sceneInfoManager)
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
                    if (i == 0)
                        // タイトル行はスキップ
                        continue;
                    if (!sceneInfoManager.SetSceneConfig(i - 1, csvDatas[i]))
                        Debug.LogError("シーン設定追加の失敗");
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
