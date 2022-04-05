using Main.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Main.Common.Const.TagConst;
using System.Threading.Tasks;

namespace Main.Common
{
    /// <summary>
    /// ゲームオブジェクト間の関数実行の指令はこのオブジェクトを仲介する
    /// T.B.D SFXやPauseScreenなどシングルトンにしてしまっている物はここに集約させる？
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>クラス自身</summary>
        private static GameManager instance;
        /// <summary>シングルトンのインスタンス</summary>
        public static GameManager Instance
        {
            get { return instance; }
        }
        private void Awake()
        {
            // シングルトンのため複数生成禁止
            if (null != instance)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        /// <summary>プレイヤーのゲームオブジェクト</summary>
        [SerializeField] private GameObject player;
        /// <summary>T.B.D 重力操作ギミックのゲームオブジェクト</summary>
        [SerializeField] private GameObject[] gravityControllers;
        /// <summary>T.B.D 敵ギミックのゲームオブジェクト</summary>
        [SerializeField] private GameObject[] humanEnemies;
        /// <summary>T.B.D レーザー砲ギミックのゲームオブジェクト</summary>
        [SerializeField] private GameObject[] turretEnemies;

        private void Reset()
        {
            if (player == null)
                player = GameObject.FindGameObjectWithTag(TAG_NAME_PLAYER);
            // T.B.D 重力操作ギミックの仮実装
            //if (gravityControllers == null && gravityControllers.length)
            //    gravityControllers = GameObject.FindGameObjectsWithTag(TAG_NAME_DUMMY);
            // T.B.D 敵ギミックの仮実装
            //if (humanEnemies == null && humanEnemies.length)
            //    humanEnemies = GameObject.FindGameObjectsWithTag(TAG_NAME_DUMMY);
            // T.B.D レーザー砲ギミックの仮実装
            //if (turretEnemies == null && turretEnemies.length)
            //    turretEnemies = GameObject.FindGameObjectsWithTag(TAG_NAME_DUMMY);
        }

        /// <summary>
        /// 空間操作オブジェクトからプレイヤーオブジェクトへ
        /// プレイヤー操作指令を実行して、実行結果を返却する
        /// </summary>
        /// <param name="moveVelocity">移動座標</param>
        /// <returns>成功／失敗</returns>
        public bool MoveCharactorFromSpaceManager(Vector3 moveVelocity)
        {
            return player.GetComponent<PlayerController>().MoveChatactorFromGameManager(moveVelocity);
        }

        /// <summary>
        /// T.B.D 重力操作ギミックの仮実装(GameManager.Instance.MoveCharactorFromGravityController(3.5fなど);)
        /// 重力操作ギミックオブジェクトからプレイヤーオブジェクトへ
        /// プレイヤー操作指令を実行して、実行結果を返却する
        /// </summary>
        /// <param name="moveVelocity">移動座標</param>
        /// <returns>成功／失敗</returns>
        public bool MoveCharactorFromGravityController(Vector3 moveVelocity)
        {
            return player.GetComponent<PlayerController>().MoveChatactorFromGameManager(moveVelocity);
        }

        /// <summary>
        /// プレイヤーを死亡させる
        /// コネクトシステム処理からの呼び出し
        /// </summary>
        /// <returns>成功／失敗</returns>
        public async Task<bool> DeadPlayerFromSpaceManager()
        {
            return await player.GetComponent<PlayerController>().DeadPlayerFromGameManager();
        }

        /// <summary>
        /// プレイヤーを死亡させる
        /// 敵からの呼び出し
        /// </summary>
        /// <returns>成功／失敗</returns>
        public async Task<bool> DeadPlayerFromHumanEnemies()
        {
            return await player.GetComponent<PlayerController>().DeadPlayerFromGameManager();
        }

        /// <summary>
        /// プレイヤーを死亡させる
        /// レーザー砲からの呼び出し
        /// </summary>
        /// <returns>成功／失敗</returns>
        public async Task<bool> DeadPlayerFromTurretEnemies()
        {
            return await player.GetComponent<PlayerController>().DeadPlayerFromGameManager();
        }
    }
}
