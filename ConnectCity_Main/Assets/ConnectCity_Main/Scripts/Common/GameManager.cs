using Main.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Main.Common.Const.TagConst;

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
        [SerializeField] private GameObject gravityController;

        private void Reset()
        {
            if (player == null)
                player = GameObject.FindGameObjectWithTag(TAG_NAME_PLAYER);
            // T.B.D 重力操作ギミックの仮実装
            //if (gravityController == null)
            //    gravityController = GameObject.FindGameObjectsWithTag(TagConst.TAG_NAME_DUMMY);
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
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool DeadPlayerFromSpaceManager()
        {
            return player.GetComponent<PlayerController>().DeadPlayerFromGameManager();
        }
    }
}
