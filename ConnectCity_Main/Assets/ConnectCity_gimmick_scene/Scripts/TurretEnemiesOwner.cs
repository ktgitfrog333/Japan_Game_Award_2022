using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common.Const;

namespace Gimmick
{
    /// <summary>
    /// レーザー砲のオーナー
    /// </summary>
    public class TurretEnemiesOwner : MonoBehaviour
    {
        /// <summary>レーザー砲たち</summary>
        private GameObject[] _turretEnemies;

        /// <summary>
        /// 初期処理
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool Initialize()
        {
            try
            {
                _turretEnemies = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_TURRETENEMIES);
                // 非対象の場合は何もせずにfalseを返却
                if (_turretEnemies.Length == 0)
                    return false;

                foreach (var g in _turretEnemies)
                    g.GetComponent<TurretEnemies>().Initialize();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 疑似的な終了
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool OnImitationDestroy()
        {
            try
            {
                // 非対象の場合は何もせずにfalseを返却
                if (_turretEnemies.Length == 0)
                    return false;

                foreach (var g in _turretEnemies)
                    g.GetComponent<TurretEnemies>().OnImitationDestroy();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
