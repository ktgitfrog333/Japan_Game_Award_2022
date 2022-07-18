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
            //Debug.DrawRay(postion + rayOriginOffset, rayDirection * rayMaxDistance, Color.green);
            var hitCount = Physics.RaycastNonAlloc(ray, hits, rayMaxDistance, layerMask);
            return hitCount >= 1f;
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
        public static GameObject IsOnEnemiesAndInfo(Vector3 postion, Vector3 rayOriginOffset, Vector3 rayDirection, float rayMaxDistance, int layerMask/*, out RaycastHit[] hits*/)
        {
            var ray = new Ray(postion + rayOriginOffset, rayDirection);
            Debug.DrawRay(postion + rayOriginOffset, rayDirection * rayMaxDistance, Color.green);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayMaxDistance, layerMask))
            {
                //Debug.Log(hit.transform.gameObject);
            }

            return hit.transform == null ? null : hit.transform.gameObject;
        }

        /// <summary>
        /// オブジェクト状態をリセット
        /// SceneOwnerからの呼び出し
        /// </summary>
        /// <param name="StagePrefab">ステージプレハブ</param>
        /// <param name="objectsOffset">オブジェクトリスト</param>
        /// <returns>成功／失敗</returns>
        public static bool LoadObjectOffset(GameObject StagePrefab, ObjectsOffset[] objectsOffset)
        {
            // 必要なオブジェクトが存在しないなら失敗
            if (StagePrefab == null || objectsOffset == null || (objectsOffset != null && objectsOffset.Length == 0))
                return false;
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
            if (errorCheck && gameObjList.Count < GameObject.Find(sceneInfoName).GetComponent<SceneOwner>().StageCountMax)
                Debug.LogError("の数が足りません");

            return gameObjList.ToArray();
        }

        /// <summary>
        /// ベクター情報を取得
        /// </summary>
        /// <param name="direction">enumの向き</param>
        /// <returns>ローカルのベクター情報</returns>
        public static Vector3 GetVectorFromDirection(Direction direction)
        {
            var v = new Vector3();
            switch (direction)
            {
                case Direction.UP:
                    // 重力を上向きにセット
                    return Vector3.up;
                case Direction.DOWN:
                    // 重力を下向きにセット
                    return Vector3.down;
                case Direction.LEFT:
                    // 重力を左向きにセット
                    return Vector3.left;
                case Direction.RIGHT:
                    // 重力を右向きにセット
                    return Vector3.right;
                default:
                    break;
            }
            return v;
        }

        /// <summary>
        /// レイの光線を3本たてる
        /// 補正値によって幅を調整する
        /// </summary>
        /// <param name="rayOriginOffset">基準点（中央点）</param>
        /// <param name="range">幅を広げる範囲</param>
        /// <returns>3点ベクター</returns>
        public static Vector3[] GetThreePointHorizontal(Vector3 rayOriginOffset, float range)
        {
            var idx = 0;
            var result = new Vector3[3];
            result[idx++] = new Vector3(-1f * range, rayOriginOffset.y);
            result[idx++] = rayOriginOffset;
            result[idx++] = new Vector3(1f * range, rayOriginOffset.y);
            return result;
        }

        /// <summary>
        /// レイの光線を2本たてる
        /// 補正値によって幅を調整する
        /// </summary>
        /// <param name="rayOriginOffset">基準点（中央点）</param>
        /// <param name="range">幅を広げる範囲</param>
        /// <returns>2点ベクター</returns>
        public static Vector3[] GetTwoPointHorizontal(Vector3 rayOriginOffset, float range)
        {
            var idx = 0;
            var result = new Vector3[2];
            result[idx++] = new Vector3(rayOriginOffset.x - range / 2f, rayOriginOffset.y);
            result[idx++] = new Vector3(rayOriginOffset.x + range / 2f, rayOriginOffset.y);
            return result;
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

    /// <summary>
    /// 向き
    /// </summary>
    public enum Direction
    {
        /// <summary>上</summary>
        UP,
        /// <summary>下</summary>
        DOWN,
        /// <summary>左</summary>
        LEFT,
        /// <summary>右</summary>
        RIGHT,
    }
}
