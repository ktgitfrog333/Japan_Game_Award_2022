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
                    Debug.Log("ぼろいブロック・天井はありません");
                foreach (var g in _breakBlocks)
                {
                    if (!g.GetComponent<BoxCollider>().enabled)
                        g.GetComponent<BoxCollider>().enabled = true;
                    if (!g.GetComponent<Renderer>().enabled)
                        g.GetComponent<Renderer>().enabled = true;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Start()
        {
            Initialize();
        }
    }
}
