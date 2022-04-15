using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Common.LevelDesign
{
    public class LevelDesigns
    {
    }
    /// <summary>
    /// レベル共通判定
    /// </summary>
    public class LevelDesisionIsObjected
    {
        /// <summary>
        /// 接地判定
        /// </summary>
        /// <param name="postion">位置・スケール</param>
        /// <param name="rayOriginOffset">始点</param>
        /// <param name="rayDirection">終点</param>
        /// <param name="rayMaxDistance">最大距離</param>
        /// <returns>レイのヒット判定の有無</returns>
        public static bool IsGrounded(Vector3 postion, Vector3 rayOriginOffset, Vector3 rayDirection, float rayMaxDistance)
        {
            var ray = new Ray(postion + rayOriginOffset, rayDirection);
            //Debug.DrawRay(postion + rayOriginOffset, rayDirection * rayMaxDistance, Color.green);
            var raycastHits = new RaycastHit[1];
            var hitCount = Physics.RaycastNonAlloc(ray, raycastHits, rayMaxDistance);
            return hitCount >= 1f;
        }

        /// <summary>
        /// プレイヤーが上に乗っているかの判定
        /// </summary>
        /// <param name="postion">位置・スケール</param>
        /// <param name="rayOriginOffset">始点</param>
        /// <param name="rayDirection">終点</param>
        /// <param name="rayMaxDistance">最大距離</param>
        /// <param name="layerMask">マスク情報</param>
        /// <returns>レイのヒット判定の有無</returns>
        public static bool IsOnPlayeredAndInfo(Vector3 postion, Vector3 rayOriginOffset, Vector3 rayDirection, float rayMaxDistance, int layerMask)
        {
            if (layerMask < 0) return IsGrounded(postion, rayOriginOffset, rayDirection, rayMaxDistance);

            var ray = new Ray(postion + rayOriginOffset, rayDirection);
            Debug.DrawRay(postion + rayOriginOffset, rayDirection * rayMaxDistance, Color.green);
            var raycastHits = new RaycastHit[1];
            var hitCount = Physics.RaycastNonAlloc(ray, raycastHits, rayMaxDistance, layerMask);
            return hitCount >= 1f;
        }
    }
}
