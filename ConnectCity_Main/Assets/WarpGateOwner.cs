using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;
using Main.Common.Const;

namespace Gimmick
{
    /// <summary>
    /// ワープゲートのオーナー
    /// </summary>
    public class WarpGateOwner : MonoBehaviour, IOwner
    {
        /// <summary>自動追尾ドローン（複数）</summary>
        private GameObject[] _warpGatesPairs;

        public bool Initialize()
        {
            throw new System.NotImplementedException();
        }

        public bool ManualStart()
        {
            try
            {
                _warpGatesPairs = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_WARPGATE);
                if (_warpGatesPairs != null)
                    foreach (var pair in _warpGatesPairs)
                        if (!pair.GetComponent<WarpGatesPair>().ManualStart())
                            throw new System.Exception("ワープゲート初期処理の失敗");

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
                if (_warpGatesPairs != null)
                    foreach (var pair in _warpGatesPairs)
                        if (!pair.GetComponent<WarpGatesPair>().Exit())
                            throw new System.Exception("ワープゲート終了処理の失敗");

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
