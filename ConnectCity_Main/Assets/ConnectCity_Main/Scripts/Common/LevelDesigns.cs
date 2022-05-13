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
            var hits = new RaycastHit[1];
            return IsOnPlayeredAndInfo(postion, rayOriginOffset, rayDirection, rayMaxDistance, layerMask, out hits);
        }

        /// <summary>
        /// 敵が上に乗っているかの判定
        /// </summary>
        /// <param name="postion">位置・スケール</param>
        /// <param name="rayOriginOffset">始点</param>
        /// <param name="rayDirection">終点</param>
        /// <param name="rayMaxDistance">最大距離</param>
        /// <param name="layerMask">マスク情報</param>
        /// <returns>レイのヒット判定の有無</returns>
        public static bool IsOnPlayeredAndInfo(Vector3 postion, Vector3 rayOriginOffset, Vector3 rayDirection, float rayMaxDistance, int layerMask, out RaycastHit[] hits)
        {
            hits = new RaycastHit[1];
            if (layerMask < 0) return IsGrounded(postion, rayOriginOffset, rayDirection, rayMaxDistance);

            var ray = new Ray(postion + rayOriginOffset, rayDirection);
            Debug.DrawRay(postion + rayOriginOffset, rayDirection * rayMaxDistance, Color.green);
            var hitCount = Physics.RaycastNonAlloc(ray, hits, rayMaxDistance, layerMask);
            return hitCount >= 1f;
        }

        /// <summary>
        /// オブジェクト状態をリセット
        /// SceneInfoManagerからの呼び出し
        /// </summary>
        /// <param name="StagePrefab">ステージプレハブ</param>
        /// <param name="objectsOffset">オブジェクトリスト</param>
        /// <returns>成功／失敗</returns>
        public static bool LoadObjectOffset(GameObject StagePrefab, ObjectsOffset[] objectsOffset)
        {
            foreach (var off in objectsOffset)
            {
                off.GameObjectObj.transform.parent = StagePrefab.transform;
                off.GameObjectObj.transform.localPosition = off.localPosition;
            }
            return true;
        }

        /// <summary>
        /// オブジェクト状態を保存
        /// シーン読み込み時に一度だけ実行
        /// </summary>
        /// <param name="objectsOffset">オブジェクト（複数）</param>
        /// <returns>成功／失敗</returns>
        public static ObjectsOffset[] SaveObjectOffset(GameObject[] objectsOffset)
        {
            var offs = new List<ObjectsOffset>();
            foreach (var obj in objectsOffset)
            {
                var off = new ObjectsOffset();
                off.GameObjectObj = obj;
                off.localPosition = obj.transform.localPosition;
                offs.Add(off);
            }
            if (0 < offs.Count)
            {
                return offs.ToArray();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// オブジェクト状態を保存
        /// シーン読み込み時に一度だけ実行
        /// </summary>
        /// <param name="objectsOffset">オブジェクト（単体）</param>
        /// <returns>成功／失敗</returns>
        public static ObjectsOffset[] SaveObjectOffset(GameObject objectsOffset)
        {
            var offs = new List<ObjectsOffset>();
            var obj = objectsOffset;
            var off = new ObjectsOffset();
            off.GameObjectObj = obj;
            off.localPosition = obj.transform.localPosition;
            offs.Add(off);
            if (0 < offs.Count)
            {
                return offs.ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// レベルデザイン内の各ステージ内から対象のオブジェクトをリストで取得
        /// タグ名を設定する
        /// </summary>
        /// <param name="levelName">レベルデザインオブジェクト名</param>
        /// <param name="sceneInfoName">シーン情報オブジェクト名</param>
        /// <param name="findObjectTag">取得対象オブジェクトのタグ名</param>
        /// <param name="errorCheck">エラーチェック有無</param>
        /// <returns>オブジェクトの配列</returns>
        public static GameObject[] GetGameObjectsInLevelDesign(string levelName,string sceneInfoName, string findObjectTag, bool errorCheck)
        {
            var gameObjList = new List<GameObject>();
            var l = GameObject.Find(levelName);
            // レベルデザイン内の各ステージ
            foreach (Transform stage in l.transform)
            {
                foreach (Transform c in stage)
                {
                    if (c.CompareTag(findObjectTag))
                    {
                        gameObjList.Add(c.gameObject);
                    }
                }
            }
            if (errorCheck && gameObjList.Count < GameObject.Find(sceneInfoName).GetComponent<SceneInfoManager>().StageCountMax)
                Debug.LogError("の数が足りません");

            return gameObjList.ToArray();
        }
    }

    /// <summary>
    /// オブジェクトの初期状態
    /// </summary>
    public struct ObjectsOffset
    {
        /// <summary>オブジェクト</summary>
        public GameObject GameObjectObj { get; set; }
        /// <summary>位置</summary>
        public Vector3 localPosition { get; set; }
    }
}
