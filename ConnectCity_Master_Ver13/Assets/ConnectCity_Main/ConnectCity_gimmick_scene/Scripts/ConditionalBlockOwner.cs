using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;
using Main.Common.LevelDesign;
using Main.Common.Const;

namespace Gimmick
{
    /// <summary>
    /// 条件付きブロックのオーナー
    /// </summary>
    public class ConditionalBlockOwner : MonoBehaviour, IOwner
    {
        /// <summary>条件付きブロック（複数）</summary>
        private GameObject[] _conditionnalBlocks;

        public bool Initialize()
        {
            throw new System.NotImplementedException();
        }

        public bool ManualStart()
        {
            try
            {
                _conditionnalBlocks = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_CONDITIONALBLOCK);
                if (_conditionnalBlocks.Length < 1)
                    Debug.LogWarning($"{TagConst.TAG_NAME_CONDITIONALBLOCK}の取得数{_conditionnalBlocks.Length}");

                foreach (var block in _conditionnalBlocks)
                    block.GetComponent<ConditionalBlock>().ManualStart();

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// 条件付きブロックのカウントダウン
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool UpdateCountDownConditionalBlock()
        {
            try
            {
                if (_conditionnalBlocks != null && 0 < _conditionnalBlocks.Length)
                    foreach (var block in _conditionnalBlocks)
                        if (!block.GetComponent<ConditionalBlock>().UpdateCountDownConditionalBlock())
                            throw new System.Exception("カウントダウン処理の失敗");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Exit()
        {
            try
            {
                if (_conditionnalBlocks != null && 0 < _conditionnalBlocks.Length)
                    foreach (var block in _conditionnalBlocks)
                        if (!block.GetComponent<ConditionalBlock>().Exit())
                            throw new System.Exception("条件付きブロック終了処理の失敗");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}
