using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;
using Main.Common.Const;
using Main.Common.LevelDesign;

namespace Gimmick
{
    /// <summary>
    /// 自動追尾ドローンのオーナー
    /// </summary>
    public class AutoDroneOwner : MonoBehaviour, IOwner
    {
        /// <summary>自動追尾ドローン（複数）</summary>
        private GameObject[] _autoDrones;
        /// <summary>自動追尾ドローンの初期状態</summary>
        private ObjectsOffset[] _autoDroneOffsets;
        /// <summary>自動追尾ドローンの初期状態</summary>
        public ObjectsOffset[] AutoDroneOffsets => _autoDroneOffsets;

        public bool ManualStart()
        {
            try
            {
                _autoDrones = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_AUTODRONE);
                if (_autoDrones != null)
                    foreach (var drone in _autoDrones)
                        if (!drone.GetComponent<AutoDrone>().ManualStart())
                            throw new System.Exception("自動追尾ドローン初期処理の失敗");
                _autoDroneOffsets = LevelDesisionIsObjected.SaveObjectOffset(_autoDrones);
                if (_autoDrones != null && 0 < _autoDrones.Length && _autoDroneOffsets == null)
                    throw new System.Exception("オブジェクト初期状態の保存の失敗");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// ドローンの操作制御
        /// </summary>
        /// <param name="active">操作可否フラグ</param>
        /// <returns>成功／失敗</returns>
        public bool SetAutoDroneMoveEnable(bool active)
        {
            try
            {
                if (_autoDrones != null)
                    foreach (var drone in _autoDrones)
                        if (!drone.GetComponent<AutoDrone>().SetAutoDroneMoveEnable(active))
                            throw new System.Exception("自動追尾ドローン初期処理の失敗");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Initialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Exit()
        {
            try
            {
                if (_autoDrones != null)
                    foreach (var drone in _autoDrones)
                        if (!drone.GetComponent<AutoDrone>().Exit())
                            throw new System.Exception("自動追尾ドローン終了処理の失敗");

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
