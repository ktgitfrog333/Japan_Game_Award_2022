using Main.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Trial.Template;

namespace Trial.Main
{
    /// <summary>
    /// 体験版ゲームマネージャー
    /// </summary>
    public class GameManagerTrial : MonoBehaviour
    {
        /// <summary>体験版フラグ</summary>
        [SerializeField] private bool trialMode = true;
        /// <summary>体験版フラグ</summary>
        public bool TrialMode => trialMode;

        /// <summary>
        /// アウェイクイベント
        /// </summary>
        public void SelfAwake()
        {
            var temp = new ConnectCityTemplateResourcesAccessory();
            var csvDatas = temp.LoadMappingConfData("MainSceneMappingData");
            if (csvDatas == null)
                Debug.LogError("読み込みエラー");
            // ステージ番号の置換マップ
            var stagesMap = new Dictionary<string, int>();
            for (var i = 0; i < csvDatas.Count; i++)
                if (!temp.SetMainSceneMappingConfig(csvDatas[i], stagesMap))
                    Debug.LogError("シーン設定追加の失敗");
            if (!SortStageTrial(stagesMap))
                Debug.LogError("並び替え処理の失敗");
            if (!AllApply())
                Debug.LogError("登録の失敗");
            if (!UpdateSceneInfo(stagesMap))
                Debug.LogError("シーン情報更新の失敗");
        }

        /// <summary>
        /// シーン情報を更新する
        /// </summary>
        /// <returns>成功／失敗</returns>
        private bool UpdateSceneInfo(Dictionary<string, int> stagesMap)
        {
            try
            {
                var sceneOwner = GameObject.Find("SceneOwner").GetComponent<SceneOwner>();
                if (!sceneOwner.UpdateFinalStage(9, true))
                    throw new System.Exception("最終ステージフラグ更新呼び出しの失敗");
                var counters = sceneOwner.ClearConnectedCounters;
                var chgCounters = new List<int>();
                foreach (var m in stagesMap)
                    chgCounters.Add(counters[int.Parse(m.Key.Replace("Stage_", ""))]);
                if (!sceneOwner.UpdateClearConnectedCounters(chgCounters.ToArray()))
                    throw new System.Exception("コネクトクリア回数更新呼び出しの失敗");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// 体験版ステージ用にレベルデザインを変更する
        /// </summary>
        /// <returns>成功／失敗</returns>
        private bool SortStageTrial(Dictionary<string, int> stagesMap)
        {
            try
            {
                if (stagesMap.Count < 1)
                    throw new System.Exception("データ未登録エラー");
                var levelDesign = GameObject.Find("LevelDesign");
                // まず各ステージをリスト化する
                var levelList = new List<Transform>();
                foreach (Transform c in levelDesign.transform)
                    levelList.Add(c);
                // 並び替えに該当するステージは一度、昇順で親の末端に移動させる（前準備ソート）
                foreach (var stage in stagesMap.OrderByDescending(q => q.Value))
                {
                    var target = levelList.Where(q => q.name.Equals(stage.Key)).Select(q => q).ToArray()[0];
                    target.SetAsFirstSibling();
                }

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 全登録
        /// </summary>
        /// <returns>成功／失敗</returns>
        private bool AllApply()
        {
            try
            {
                var levelOwner = GameObject.Find("LevelOwner");
                var levelDesign = GameObject.Find("LevelDesign");

                // プレイヤーを登録
                var players = levelOwner.GetComponent<LevelOwner>().Players;
                for (var i = 0; i < players.Length; i++)
                    // ステージごとの対象
                    foreach (Transform c in levelDesign.transform.GetChild(i))
                        if (c.name.Equals("Player"))
                            players[i] = c.gameObject;

                levelOwner.GetComponent<LevelOwner>().Players = players;

                // ゴールを登録
                var goalPoints = levelOwner.GetComponent<LevelOwner>().GoalPoints;
                for (var i = 0; i < goalPoints.Length; i++)
                    // ステージごとの対象
                    foreach (Transform c in levelDesign.transform.GetChild(i))
                        if (c.name.Equals("GoalPoint"))
                            goalPoints[i] = c.gameObject;

                levelOwner.GetComponent<LevelOwner>().GoalPoints = goalPoints;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }
}
