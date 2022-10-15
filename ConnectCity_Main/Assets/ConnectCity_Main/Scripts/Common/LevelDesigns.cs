using Main.InputSystem;
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
        /// <param name="position">位置・スケール</param>
        /// <param name="rayOriginOffset">始点</param>
        /// <param name="rayDirection">終点</param>
        /// <param name="rayMaxDistance">最大距離</param>
        /// <returns>レイのヒット判定の有無</returns>
        public static bool IsGrounded(Vector3 position, Vector3 rayOriginOffset, Vector3 rayDirection, float rayMaxDistance)
        {
            var ray = new Ray(position + rayOriginOffset, rayDirection);
            //Debug.DrawRay(postion + rayOriginOffset, rayDirection * rayMaxDistance, Color.green);
            var raycastHits = new RaycastHit[1];
            var hitCount = Physics.RaycastNonAlloc(ray, raycastHits, rayMaxDistance);
            return hitCount >= 1f;
        }

        /// <summary>
        /// プレイヤーが上に乗っているかの判定
        /// </summary>
        /// <param name="position">位置・スケール</param>
        /// <param name="rayOriginOffset">始点</param>
        /// <param name="rayDirection">終点</param>
        /// <param name="rayMaxDistance">最大距離</param>
        /// <param name="layerMask">マスク情報</param>
        /// <returns>レイのヒット判定の有無</returns>
        public static bool IsOnPlayeredAndInfo(Vector3 position, Vector3 rayOriginOffset, Vector3 rayDirection, float rayMaxDistance, int layerMask)
        {
            var hits = new RaycastHit[1];
            return IsOnPlayeredAndInfo(position, rayOriginOffset, rayDirection, rayMaxDistance, layerMask, out hits);
        }

        /// <summary>
        /// 敵が上に乗っているかの判定
        /// </summary>
        /// <param name="position">位置・スケール</param>
        /// <param name="rayOriginOffset">始点</param>
        /// <param name="rayDirection">終点</param>
        /// <param name="rayMaxDistance">最大距離</param>
        /// <param name="layerMask">マスク情報</param>
        /// <returns>レイのヒット判定の有無</returns>
        public static bool IsOnPlayeredAndInfo(Vector3 position, Vector3 rayOriginOffset, Vector3 rayDirection, float rayMaxDistance, int layerMask, out RaycastHit[] hits)
        {
            hits = new RaycastHit[1];
            if (layerMask < 0) return IsGrounded(position, rayOriginOffset, rayDirection, rayMaxDistance);

            var ray = new Ray(position + rayOriginOffset, rayDirection);
            //Debug.DrawRay(position + rayOriginOffset, rayDirection * rayMaxDistance, Color.green);
            var hitCount = Physics.RaycastNonAlloc(ray, hits, rayMaxDistance, layerMask);
            return hitCount >= 1f;
        }

        /// <summary>
        /// 敵が上に乗っているかの判定
        /// </summary>
        /// <param name="position">位置・スケール</param>
        /// <param name="rayOriginOffset">始点</param>
        /// <param name="rayDirection">終点</param>
        /// <param name="rayMaxDistance">最大距離</param>
        /// <param name="layerMask">マスク情報</param>
        /// <returns>レイのヒット判定の有無</returns>
        public static GameObject IsOnEnemiesAndInfo(Vector3 position, Vector3 rayOriginOffset, Vector3 rayDirection, float rayMaxDistance, int layerMask/*, out RaycastHit[] hits*/)
        {
            var ray = new Ray(position + rayOriginOffset, rayDirection);
            Debug.DrawRay(position + rayOriginOffset, rayDirection * rayMaxDistance, Color.green);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayMaxDistance, layerMask))
            {
                //Debug.Log(hit.transform.gameObject);
            }

            return hit.transform == null ? null : hit.transform.gameObject;
        }

        /// <summary>
        /// 敵が上に乗っているかの判定
        /// レイの光線を3本たてる処理との統合版
        /// </summary>
        /// <param name="position">位置・スケール</param>
        /// <param name="rayOriginOffset">始点</param>
        /// <param name="rayDirection">終点</param>
        /// <param name="rayMaxDistance">最大距離</param>
        /// <param name="layerMask">マスク情報</param>
        /// <param name="range">幅を広げる範囲</param>
        /// <returns>レイのヒット判定の有無</returns>
        public static GameObject IsOnEnemiesAndInfoThreePointHorizontal(Vector3 position, Vector3 rayOriginOffset, Vector3 rayDirection, float rayMaxDistance, int layerMask, float range)
        {
            var rOrgOffAry = GetThreePointHorizontal(rayOriginOffset, range);
            var objList = new List<GameObject>();
            foreach (var offset in rOrgOffAry)
            {
                var g = IsOnEnemiesAndInfo(position, offset, rayDirection, rayMaxDistance, layerMask);
                if (g != null)
                    objList.Add(g);
            }
            return 0 < objList.Count ? objList[0] : null;
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

        /// <summary>
        /// 操作入力を元に制御情報を更新
        /// </summary>
        /// <returns>処理結果の成功／失敗</returns>
        public static Vector3[] SetMoveVelocotyLeftAndRight()
        {
            // キーボード
            var lCom = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.ManualLAxcel;
            var hztlLKey = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.ManualLMove.x;
            var vtclLkey = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.ManualLMove.y;
            var rCom = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.ManualRAxcel;
            var hztlRKey = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.ManualRMove.x;
            var vtclRkey = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.ManualRMove.y;

            // コントローラー
            var hztlL = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.AutoLMove.x;
            var vtclL = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.AutoLMove.y;
            var hztlR = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.AutoRMove.x;
            var vtclR = GameManager.Instance.InputSystemsOwner.GetComponent<InputSystemsOwner>().InputSpace.AutoRMove.y;

            if ((lCom && 0f < Mathf.Abs(hztlLKey)) || (lCom && 0f < Mathf.Abs(vtclLkey)) ||
                (rCom && 0f < Mathf.Abs(hztlRKey)) || (rCom && 0f < Mathf.Abs(vtclRkey)))
            {
                // キーボード操作のインプット
                return SetVelocity(hztlLKey, vtclLkey, hztlRKey, vtclRkey);
            }
            else if (0f < Mathf.Abs(hztlL) || 0f < Mathf.Abs(vtclL) || 0f < Mathf.Abs(hztlR) || 0f < Mathf.Abs(vtclR))
            {
                // コントローラー操作のインプット
                return SetVelocity(hztlL, vtclL, hztlR, vtclR);
            }
            else
            {
                Vector3[] space = { new Vector3(), new Vector3() };
                return space;
            }
        }

        /// <summary>
        /// 移動先情報をセット
        /// </summary>
        /// <param name="horizontalLeft">左空間のHorizontal</param>
        /// <param name="verticalLeft">左空間のVertical</param>
        /// <param name="horizontalRight">右空間のHorizontal</param>
        /// <param name="verticalRight">右空間のVertical</param>
        /// <returns>成功</returns>
        private static Vector3[] SetVelocity(float horizontalLeft, float verticalLeft, float horizontalRight, float verticalRight)
        {
            Vector3[] space = { new Vector3(horizontalLeft, verticalLeft), new Vector3(horizontalRight, verticalRight) };
            return space;
        }

        /// <summary>
        /// 対象の位置が左空間 or 右空間に存在するかをチェック
        /// </summary>
        /// <param name="targetPosition">対象のポジション</param>
        /// <returns>左右の方向（0なら-1）</returns>
        public static int CheckPositionAndGetDirection2D(Transform centerTransform, Vector3 targetPosition)
        {
            return CheckPositionAndGetDirection2D(centerTransform, targetPosition, 0f);
        }

        /// <summary>
        /// 対象の位置が左空間 or 右空間に存在するかをチェック
        /// </summary>
        /// <param name="targetPosition">対象のポジション</param>
        /// <returns>左右の方向（0なら-1）</returns>
        public static int CheckPositionAndGetDirection2D(Transform centerTransform, Vector3 targetPosition, float offset)
        {
            if (targetPosition.x < centerTransform.localPosition.x - offset)
            {
                return (int)Direction2D.Left;
            }
            else if (centerTransform.localPosition.x + offset < targetPosition.x)
            {
                return (int)Direction2D.Right;
            }
            return -1;
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

    /// <summary>
    /// 2Dの角度
    /// </summary>
    public enum Direction2D
    {
        /// <summary>左方向</summary>
        Left,
        /// <summary>右方向</summary>
        Right,
    }
}
