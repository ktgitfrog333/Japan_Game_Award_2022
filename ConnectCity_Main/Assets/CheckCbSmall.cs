using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common.LevelDesign;
using System.Linq;
using Main.Common.Const;
using UniRx;
using UniRx.Triggers;

namespace Gimmick
{
    /// <summary>
    /// 立方体のレイ判定用
    /// </summary>
    public class CheckCbSmall : MonoBehaviour
    {
        /// <summary>一辺を描画するレイのマップ</summary>
        [SerializeField] private CubeRaysMap[] raysMap;
        /// <summary>一辺を描画するレイの距離</summary>
        [SerializeField] private float distance;
        ///// <summary>レイヤーマスク</summary>
        //[SerializeField] private LayerMask layerMask;

        /// <summary>
        /// 一辺を描画するレイのマップ
        /// originPosition
        /// direction
        /// </summary>
        [System.Serializable]
        public class CubeRaysMap
        {
            /// <summary>ゴーストキューブのローカルPath</summary>
            [SerializeField] private Vector3[] dotPosition;
            /// <summary>ゴーストキューブのローカルPath</summary>
            public Vector3[] DotPosition => dotPosition;
        }

        /// <summary>
        /// 空間のチェック
        /// </summary>
        /// <returns>接触あり／無し</returns>
        public bool CheckSpace()
        {
            try
            {
                var result = new List<bool>();
                Debug.Log(transform.position);
                foreach (var rayMap in raysMap)
                {
                    var r = /*LevelDesisionIsObjected.*/IsHitCheckCube(transform.position, rayMap.DotPosition[0], rayMap.DotPosition[1], distance, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE));
                    //if (r)
                    //    Debug.LogError($"pos:{transform.position}dotPos:{rayMap.DotPosition[0]}dire:{rayMap.DotPosition[1]}");
                    result.Add(r);
                }
                this.UpdateAsObservable()
                    .Subscribe(_ =>
                    {
                        foreach (var rayMap in raysMap)
                        {
                            var r = /*LevelDesisionIsObjected.*/IsHitCheckCube(transform.position, rayMap.DotPosition[0], rayMap.DotPosition[1], distance, LayerMask.GetMask(LayerConst.LAYER_NAME_FREEZE));
                            Debug.Log(r);
                        }
                    });

                //Debug.Log(raysMap.Length);
                //Debug.Log(result.Where(q => q)
                //    .Select(q => q)
                //    .ToList().Count);
                return 0 < result.Where(q => q)
                    .Select(q => q)
                    .ToList().Count;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 接地判定
        /// </summary>
        /// <param name="postion">位置・スケール</param>
        /// <param name="rayOriginOffset">始点</param>
        /// <param name="rayDirection">終点</param>
        /// <param name="rayMaxDistance">最大距離</param>
        /// <returns>レイのヒット判定の有無</returns>
        private bool IsHitCheckCube(Vector3 postion, Vector3 rayOriginOffset, Vector3 rayDirection, float rayMaxDistance, int layerMask)
        {
            var hits = new RaycastHit[1];
            //Debug.Log(layerMask);
            if (layerMask < 0)
            {
                return IsGrounded(postion, rayOriginOffset, rayDirection, rayMaxDistance);
            }

            var ray = new Ray(postion + rayOriginOffset, rayDirection);
            //Debug.LogError($"{layerMask}");
            var hitCount = Physics.RaycastNonAlloc(ray, hits, rayMaxDistance, layerMask);
            if (hitCount >= 1f)
                Debug.Log(hits[0].transform.name);
            Debug.DrawRay(postion + rayOriginOffset, rayDirection * rayMaxDistance, Color.green);
            return hitCount >= 1f;
        }
        /// <summary>
        /// 接地判定
        /// </summary>
        /// <param name="postion">位置・スケール</param>
        /// <param name="rayOriginOffset">始点</param>
        /// <param name="rayDirection">終点</param>
        /// <param name="rayMaxDistance">最大距離</param>
        /// <returns>レイのヒット判定の有無</returns>
        private bool IsGrounded(Vector3 postion, Vector3 rayOriginOffset, Vector3 rayDirection, float rayMaxDistance)
        {
            var ray = new Ray(postion + rayOriginOffset, rayDirection);
            //Debug.DrawRay(postion + rayOriginOffset, rayDirection * rayMaxDistance, Color.green);
            var raycastHits = new RaycastHit[1];
            var hitCount = Physics.RaycastNonAlloc(ray, raycastHits, rayMaxDistance);
            return hitCount >= 1f;
        }
    }
}
