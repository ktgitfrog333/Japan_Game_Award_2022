using Main.Common.Const;
using Main.Common.LevelDesign;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Gimmick
{
    /// <summary>
    /// ロボットオブジェクトのオーナー
    /// </summary>
    public class RobotEnemiesOwner : MonoBehaviour
    {
        /// <summary>敵の初期状態</summary>
        private ObjectsOffset[] _robotEmemOffsets;
        /// <summary>敵の初期状態</summary>
        public ObjectsOffset[] RobotEmemOffsets => _robotEmemOffsets;
        /// <summary>敵たち</summary>
        private GameObject[] _robotEmems;

        /// <summary>
        /// 初期化
        /// </summary>
        public bool Initialize()
        {
            try
            {
                _robotEmems = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_ROBOT_EMEMY);
                if (_robotEmems.Length == 0)
                    Debug.Log(TagConst.TAG_NAME_ROBOT_EMEMY + "の取得失敗");

                _robotEmemOffsets = LevelDesisionIsObjected.SaveObjectOffset(_robotEmems);
                if (_robotEmems != null && 0 < _robotEmems.Length && _robotEmemOffsets == null)
                    Debug.LogError("オブジェクト初期状態の保存の失敗");

                foreach (var robot in _robotEmems)
                    robot.GetComponent<Robot_Enemy>().Initialize();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
