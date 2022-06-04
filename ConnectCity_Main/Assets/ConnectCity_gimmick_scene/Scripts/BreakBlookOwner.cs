using Main.Common.Const;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gimmick
{
    /// <summary>
    /// ぼろいブロック・天井のオーナー
    /// </summary>
    public class BreakBlookOwner : MonoBehaviour
    {
        /// <summary>ぼろいブロック・天井たち</summary>
        private GameObject[] _breakBlocks;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            try
            {
                _breakBlocks = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_BREAKBLOOK);
                if (_breakBlocks.Length == 0)
                    Debug.Log("初期処理：ぼろいブロック・天井はありません");
                foreach (var g in _breakBlocks)
                {
                    if (!g.GetComponent<Renderer>().enabled)
                        g.GetComponent<Renderer>().enabled = true;
                    if (!g.GetComponent<BoxCollider>().enabled)
                        g.GetComponent<BoxCollider>().enabled = true;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ディレイ付きの初期処理
        /// ※空間操作ブロックの衝突判定のタイミングより後に実行させる必要があり、暫定対応
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool DelayInitializeBreakBlocksFromLevelOwner()
        {
            try
            {
                if (_breakBlocks.Length == 0)
                    Debug.Log("開始演出：ぼろいブロック・天井はありません");
                foreach (var g in _breakBlocks)
                {
                    g.GetComponent<BreakBlook>().Initialize();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 監視の停止
        /// </summary>
        public void DisposeAllFromLevelOwner()
        {
            if (_breakBlocks.Length == 0)
                Debug.Log("開始演出：ぼろいブロック・天井はありません");
            foreach (var g in _breakBlocks)
            {
                g.GetComponent<BreakBlook>().DisposeAll();
            }
        }

        private void Start()
        {
            Initialize();
        }
    }
}
