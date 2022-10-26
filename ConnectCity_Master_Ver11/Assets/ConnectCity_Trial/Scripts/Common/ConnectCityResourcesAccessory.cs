using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Trial.Common
{
    /// <summary>
    /// リソース管理用
    /// </summary>
    public class ConnectCityResourcesAccessory
    {
        /// <summary>
        /// マッピングデータの取得
        /// </summary>
        /// <param name="resourcesLoadName">リソースファイル名</param>
        /// <returns>成功／失敗</returns>
        public List<string[]> LoadMappingConfData(string resourcesLoadName)
        {
            try
            {
                var csvDatas = new List<string[]>();
                var ta = Resources.Load(resourcesLoadName) as TextAsset;
                var sr = new StringReader(ta.text);

                while (sr.Peek() != -1)
                {
                    var l = sr.ReadLine();
                    csvDatas.Add(l.Split(','));
                }

                return csvDatas;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        /// <summary>
        /// メインシーンのマッピングデータをオブジェクトへセット
        /// </summary>
        /// <param name="datas">１ステージのデータ分</param>
        /// <param name="stagesMap">格納先オブジェクト</param>
        /// <returns>成功／失敗</returns>
        public bool SetMainSceneMappingConfig(string[] datas, Dictionary<string, int> stagesMap)
        {
            try
            {
                for (var j = 0; j < datas.Length; j++)
                {
                    var child = datas[j];
                    switch (j)
                    {
                        case (int)MainSceneMappingDataColumns.StageIndex:
                            var valsStr = ConvStringArrayOfString(child);
                            stagesMap[valsStr[0]] = int.Parse(valsStr[1]);
                            break;
                        default:
                            Debug.LogError("カラム不備");
                            break;
                    }
                }

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// セレクトシーンのマッピングデータをオブジェクトへセット
        /// </summary>
        /// <param name="datas">１ステージのデータ分</param>
        /// <param name="stagesMap">格納先オブジェクト</param>
        /// <returns>成功／失敗</returns>
        public bool SetTitleSelectSceneMappingConfig(string[] datas, Dictionary<int, int> stagesMap)
        {
            try
            {
                for (var j = 0; j < datas.Length; j++)
                {
                    var child = datas[j];
                    switch (j)
                    {
                        case (int)SelectSceneMappingDataColumns.StageIndex:
                            var valsStr = ConvStringArrayOfString(child);
                            stagesMap[int.Parse(valsStr[0])] = int.Parse(valsStr[1]);
                            break;
                        default:
                            Debug.LogError("カラム不備");
                            break;
                    }
                }

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// 文字列を文字列の配列に変換して取得
        /// :を消して配列に変換
        /// </summary>
        /// <param name="value">文字列変換データ</param>
        /// <returns>文字列配列</returns>
        private string[] ConvStringArrayOfString(string value)
        {
            if (value.IndexOf(":") < 0)
                Debug.LogError("フォーマット不正");
            var array = value.Split(':');
            if (array.Length != 2)
                Debug.LogError("フォーマット不正");
            return array;
        }
    }

    /// <summary>
    /// メインシーンのマッピングカラム
    /// </summary>
    public enum MainSceneMappingDataColumns
    {
        /// <summary>ステージ番号</summary>
        StageIndex,
    }

    /// <summary>
    /// セレクトシーンのマッピングカラム
    /// </summary>
    public enum SelectSceneMappingDataColumns
    {
        /// <summary>ステージ番号</summary>
        StageIndex,
    }
}
