using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Trial.Template;

namespace Trial.TitleSelect
{
    /// <summary>
    /// 体験版
    /// セレクトシーンの制御
    /// </summary>
    public class InputKey_SelectTrial : MonoBehaviour
    {
        /// <summary>体験版フラグ</summary>
        [SerializeField] private bool trialMode = true;
        /// <summary>体験版フラグ</summary>
        public bool TrialMode => trialMode;
        /// <summary>ステージ番号の置換マップ</summary>
        private Dictionary<int, int> _stagesMap = new Dictionary<int, int>();

        /// <summary>
        /// スタートイベント
        /// </summary>
        public void SelfStart()
        {
            var temp = new ConnectCityTemplateResourcesAccessory();
            var csvDatas = temp.LoadMappingConfData("SelectSceneMappingData");
            if (csvDatas == null)
                Debug.LogError("読み込みエラー");
            for (var i = 0; i < csvDatas.Count; i++)
                if (!temp.SetTitleSelectSceneMappingConfig(csvDatas[i], _stagesMap))
                    Debug.LogError("シーン設定追加の失敗");
        }

        /// <summary>
        /// ステージ番号をマッピングから変換
        /// </summary>
        /// <param name="stage">ステージ番号</param>
        /// <returns>ステージ番号（置換後）</returns>
        public int GetStageNumberTrial(int stage)
        {
            try
            {
                if (_stagesMap != null && 0 < _stagesMap.Count)
                {
                    // マッピングデータ内に存在しないステージ番号は変換しない
                    return _stagesMap.ContainsKey(stage) ? _stagesMap[stage] : stage;
                }
                else
                    throw new System.Exception("ステージ番号マップにデータがありません");
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return -1;
            }
        }
    }
}
