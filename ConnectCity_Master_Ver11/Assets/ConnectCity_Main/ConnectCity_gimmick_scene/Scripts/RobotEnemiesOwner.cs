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

        /// <summary>
        /// 敵操作を実行
        /// </summary>
        /// <param name="moveVelocity">移動座標</param>
        /// <param name="origin">対象オブジェクト</param>
        /// <returns>成功／失敗</returns>
        public bool MoveCharactorRobotEnemy(Vector3 moveVelocity, GameObject origin)
        {
            var obj = GetRobotEnemy(origin);
            if (obj != null)
                return obj.GetComponent<Robot_Enemy>().MoveRobotEnemy(moveVelocity);
            return false;
        }

        /// <summary>
        /// オーナーからCharactorのステータスを変更
        /// </summary>
        /// <param name="isEnabled">有効／無効フラグ</param>
        /// <param name="origin">対象オブジェクト</param>
        /// <returns>成功／失敗</returns>
        public bool ChangeCharactorControllerStateRobotEnemy(bool isEnabled, GameObject origin)
        {
            var obj = GetRobotEnemy(origin);
            if (obj != null)
                return obj.GetComponent<Robot_Enemy>().ChangeCharactorControllerStateRobotEnemy(isEnabled);
            return false;
        }

        /// <summary>
        /// オーナーからカプセルコライダーのステータスを変更
        /// </summary>
        /// <param name="isEnabled">有効／無効フラグ</param>
        /// <param name="origin">対象オブジェクト</param>
        /// <returns>成功／失敗</returns>
        public bool ChangeCapsuleColliderStateRobotEnemy(bool isEnabled, GameObject origin)
        {
            var obj = GetRobotEnemy(origin);
            if (obj != null)
                return obj.GetComponent<Robot_Enemy>().ChangeCapsuleColliderStateRobotEnemy(isEnabled);
            return false;
        }

        /// <summary>
        /// 敵をリストから取得
        /// </summary>
        /// <param name="origin">対象の敵オブジェクト</param>
        /// <returns>敵オブジェクト（単体）</returns>
        private GameObject GetRobotEnemy(GameObject origin)
        {
            if (_robotEmems != null && 0 < _robotEmems.Length)
                return _robotEmems.Where(q => q.Equals(origin))
                    .Select(q => q)
                    .ToArray()[0];
            return null;
        }
    }
}
