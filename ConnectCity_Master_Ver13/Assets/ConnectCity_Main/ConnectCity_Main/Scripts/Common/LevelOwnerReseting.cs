using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Common
{
    /// <summary>
    /// LevelOwnerへプレイヤーとゴールポイントの自動設定を行う
    /// </summary>
    public class LevelOwnerReseting : MonoBehaviour
    {
        private void Reset()
        {
            AllApply();
        }

        private void AllApply()
        {
            var levelOwner = GameObject.Find("LevelOwner");
            var levelDesign = GameObject.Find("LevelDesign");

            // プレイヤーを登録
            var players = levelOwner.GetComponent<LevelOwner>().Players;
            for (var i = 0; i < players.Length; i++)
            {
                //Debug.Log(players[i]);
                if (players[i] == null)
                {
                    // ステージごとの対象
                    foreach (Transform c in levelDesign.transform.GetChild(i))
                    {
                        if (c.name.Equals("Player"))
                        {
                            players[i] = c.gameObject;
                            Debug.Log($"プレイヤーを追加:[{i}]");
                        }
                    }
                }
            }
            levelOwner.GetComponent<LevelOwner>().Players = players;

            // ゴールを登録
            var goalPoints = levelOwner.GetComponent<LevelOwner>().GoalPoints;
            for (var i = 0; i < goalPoints.Length; i++)
            {
                if (goalPoints[i] == null)
                {
                    // ステージごとの対象
                    foreach (Transform c in levelDesign.transform.GetChild(i))
                    {
                        if (c.name.Equals("GoalPoint"))
                        {
                            goalPoints[i] = c.gameObject;
                            Debug.Log($"ゴールを追加:[{i}]");
                        }
                    }

                }
            }
            levelOwner.GetComponent<LevelOwner>().GoalPoints = goalPoints;
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
