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
        /// <summary>ワープゲート（複数）</summary>
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

        /// <summary>
        /// ワープ可否のステータスを切り替える
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool SwitchWarpState()
        {
            try
            {
                if (_warpGatesPairs != null)
                    foreach (var pair in _warpGatesPairs)
                        if (!pair.GetComponent<WarpGatesPair>().SwitchWarpState())
                            throw new System.Exception("ワープ可否フラグ切り替え処理の失敗");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 吸い込まれるTweenアニメーションを実行済み状態でKill
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool KillCompleteTweeenSuction()
        {
            try
            {
                if (_warpGatesPairs != null)
                    foreach (var pair in _warpGatesPairs)
                        if (!pair.GetComponent<WarpGatesPair>().KillCompleteTweeenSuction())
                            throw new System.Exception("Tweenアニメーションを実行済み状態でKill処理の失敗");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// WarpGatesPairの操作禁止フラグをセット
        /// </summary>
        /// <param name="flag">入力禁止フラグ</param>
        /// <returns>成功／失敗</returns>
        public bool SetWarpGatesPairInputBan(bool flag)
        {
            try
            {
                if (_warpGatesPairs != null)
                    foreach (var pair in _warpGatesPairs)
                        if (!pair.GetComponent<WarpGatesPair>().SetWarpGatesPairInputBan(flag))
                            throw new System.Exception("操作禁止フラグ切り替えの失敗");

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
