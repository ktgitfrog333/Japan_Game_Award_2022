using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Trial.Common;

namespace Trial.Template
{
    /// <summary>
    /// コネクトシティテンプレート
    /// リソース管理用
    /// </summary>
    public class ConnectCityTemplateResourcesAccessory
    {
        /// <summary>
        /// マッピングデータの取得
        /// </summary>
        /// <param name="resourcesLoadName">リソースファイル名</param>
        /// <returns>成功／失敗</returns>
        public List<string[]> LoadMappingConfData(string resourcesLoadName)
        {
            return new ConnectCityResourcesAccessory().LoadMappingConfData(resourcesLoadName);
        }

        /// <summary>
        /// メインシーンのマッピングデータをオブジェクトへセット
        /// </summary>
        /// <param name="datas">１ステージのデータ分</param>
        /// <param name="stagesMap">格納先オブジェクト</param>
        /// <returns>成功／失敗</returns>
        public bool SetMainSceneMappingConfig(string[] datas, Dictionary<string, int> stagesMap)
        {
            return new ConnectCityResourcesAccessory().SetMainSceneMappingConfig(datas, stagesMap);
        }

        /// <summary>
        /// セレクトシーンのマッピングデータをオブジェクトへセット
        /// </summary>
        /// <param name="datas">１ステージのデータ分</param>
        /// <param name="stagesMap">格納先オブジェクト</param>
        /// <returns>成功／失敗</returns>
        public bool SetTitleSelectSceneMappingConfig(string[] datas, Dictionary<int, int> stagesMap)
        {
            return new ConnectCityResourcesAccessory().SetTitleSelectSceneMappingConfig(datas, stagesMap);
        }
    }
}
