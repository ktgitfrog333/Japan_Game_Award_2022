using Main.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Main.Common.Const.TagConst;
using System.Threading.Tasks;
using Main.Level;
using Main.Common.LevelDesign;
using System.Linq;
using Gimmick;
using UniRx;
using UniRx.Triggers;

namespace Main.Common
{
    /// <summary>
    /// レベルデザインのオーナー
    /// </summary>
    public class LevelOwner : MonoBehaviour, IOwner
    {
        /// <summary>レベルデザイン</summary>
        [SerializeField] private GameObject levelDesign;
        /// <summary>レベルデザイン</summary>
        public GameObject LevelDesign => levelDesign;
        /// <summary>Skyboxの設定</summary>
        [SerializeField] private GameObject skyBoxSet;
        /// <summary>Skyboxの設定</summary>
        public GameObject SkyBoxSet => skyBoxSet;
        /// <summary>プレイヤーのゲームオブジェクト</summary>
        [SerializeField] private GameObject[] player;
        /// <summary>プレイヤーのゲームオブジェクト</summary>
        public GameObject Player => player[GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().SceneIdCrumb.Current];
        /// <summary>プレイヤーの初期状態</summary>
        private ObjectsOffset[] _playerOffsets;
        /// <summary>プレイヤーの初期状態</summary>
        public ObjectsOffset[] PlayerOffsets => _playerOffsets;
        /// <summary>空間操作</summary>
        [SerializeField] private GameObject spaceOwner;
        /// <summary>空間操作</summary>
        public GameObject SpaceOwner => spaceOwner;
        /// <summary>カメラ</summary>
        [SerializeField] private GameObject mainCamera;
        /// <summary>カメラ</summary>
        public GameObject MainCamera { get { return mainCamera; } }
        /// <summary>ゴールポイントのゲームオブジェクト</summary>
        [SerializeField] private GameObject[] goalPoint;
        /// <summary>ゴールポイントのゲームオブジェクト</summary>
        public GameObject GoalPoint => goalPoint[GameManager.Instance.SceneOwner.GetComponent<SceneOwner>().SceneIdCrumb.Current];
        /// <summary>敵ギミックのオーナー</summary>
        [SerializeField] private GameObject robotEnemiesOwner;
        /// <summary>敵ギミックのオーナー</summary>
        public GameObject RobotEnemiesOwner => robotEnemiesOwner;
        /// <summary>ぼろいブロック・天井のオーナー</summary>
        [SerializeField] private GameObject breakBlookOwner;
        /// <summary>ぼろいブロック・天井のオーナー</summary>
        public GameObject BreakBlookOwner => breakBlookOwner;

        /// <summary>レーザー砲ギミックのオーナー</summary>
        [SerializeField] private GameObject turretEnemiesOwner;
        /// <summary>レーザー砲ギミックのオーナー</summary>
        public GameObject TurretEnemiesOwner => turretEnemiesOwner;
        /// <summary>自動追尾ドローンのオーナー</summary>
        [SerializeField] private GameObject autoDroneOwner;
        /// <summary>自動追尾ドローンのオーナー</summary>
        public GameObject AutoDroneOwner => autoDroneOwner;
        /// <summary>ワープゲートのオーナー</summary>
        [SerializeField] private GameObject warpGateOwner;
        /// <summary>ワープゲートのオーナー</summary>
        public GameObject WarpGateOwner => warpGateOwner;
        /// <summary>条件付きブロックのオーナー</summary>
        [SerializeField] private GameObject conditionalBlockOwner;
        /// <summary>条件付きブロックのオーナー</summary>
        public GameObject ConditionalBlockOwner => conditionalBlockOwner;
        /// <summary>ON/OFFブロックのオーナー</summary>
        [SerializeField] private GameObject switchOnOffBlockOwner;
        /// <summary>ON/OFFブロックのオーナー</summary>
        public GameObject SwitchOnOffBlockOwner => switchOnOffBlockOwner;
        /// <summary>メソッドをコールさせる優先順位</summary>
        private OmnibusCallCode _omnibusCall = OmnibusCallCode.None;
        /// <summary>優先メソッドをコール中の待機時間</summary>
        [SerializeField] private int omnibusCallDelayTime = 10;

        private void Reset()
        {
            if (levelDesign == null)
                levelDesign = GameObject.Find("LevelDesign");
            if (skyBoxSet == null)
                skyBoxSet = GameObject.Find("SkyBoxSet");
            if (player == null || player.Length == 0)
                player = LevelDesisionIsObjected.GetGameObjectsInLevelDesign("LevelDesign", "SceneOwner", TAG_NAME_PLAYER, true);
            if (spaceOwner == null)
                spaceOwner = GameObject.FindGameObjectWithTag(TAG_NAME_SPACEMANAGER);
            if (mainCamera == null)
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            if (goalPoint == null || goalPoint.Length == 0)
                goalPoint = LevelDesisionIsObjected.GetGameObjectsInLevelDesign("LevelDesign", "SceneOwner", TAG_NAME_GOALPOINT, true);
            if (breakBlookOwner == null)
                breakBlookOwner = GameObject.Find("BreakBlookOwner");
            if (robotEnemiesOwner == null)
                robotEnemiesOwner = GameObject.Find("RobotEnemiesOwner");
            if (turretEnemiesOwner == null)
                turretEnemiesOwner = GameObject.Find("TurretEnemiesOwner");
            if (autoDroneOwner == null)
                autoDroneOwner = GameObject.Find("AutoDroneOwner");
            if (warpGateOwner == null)
                warpGateOwner = GameObject.Find("WarpGateOwner");
            if (conditionalBlockOwner == null)
                conditionalBlockOwner = GameObject.Find("ConditionalBlockOwner");
            if (switchOnOffBlockOwner == null)
                switchOnOffBlockOwner = GameObject.Find("SwitchOnOffBlockOwner");
        }

        /// <summary>
        /// 初期処理
        /// </summary>
        public bool Initialize()
        {
            try
            {
                this.UpdateAsObservable()
                    .Subscribe(async _ =>
                    {
                        if (_omnibusCall.Equals(OmnibusCallCode.MoveCharactorFromSpaceOwner))
                        {
                        // ProjectSettings > Physics > Sleep Threshold（1フレームの待機時間）
                        await Task.Delay(omnibusCallDelayTime);
                        }
                        _omnibusCall = OmnibusCallCode.None;
                    });

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 疑似スタート
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool ManualStart()
        {
            try
            {
                _playerOffsets = LevelDesisionIsObjected.SaveObjectOffset(Player);
                if (_playerOffsets == null)
                    throw new System.Exception("オブジェクト初期状態の保存の失敗");
                if (!autoDroneOwner.GetComponent<AutoDroneOwner>().ManualStart())
                    throw new System.Exception("自動追尾ドローンオーナー初期処理の失敗");
                if (!warpGateOwner.GetComponent<WarpGateOwner>().ManualStart())
                    throw new System.Exception("ワープゲートのオーナー初期処理の失敗");
                if (!conditionalBlockOwner.GetComponent<ConditionalBlockOwner>().ManualStart())
                    throw new System.Exception("条件付きブロックのオーナー初期処理の失敗");
                if (!switchOnOffBlockOwner.GetComponent<SwitchOnOffBlockOwner>().ManualStart())
                    throw new System.Exception("ON/OFFブロックのオーナー初期処理の失敗");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Exit()
        {
            if (!autoDroneOwner.GetComponent<AutoDroneOwner>().Exit())
                Debug.LogError("自動追尾ドローンオーナー終了処理の失敗");
            if (!warpGateOwner.GetComponent<WarpGateOwner>().Exit())
                Debug.LogError("ワープゲートのオーナー終了処理の失敗");
            if (!conditionalBlockOwner.GetComponent<ConditionalBlockOwner>().Exit())
                Debug.LogError("条件付きブロックのオーナー終了処理の失敗");
            if (!switchOnOffBlockOwner.GetComponent<SwitchOnOffBlockOwner>().Exit())
                Debug.LogError("ON/OFFブロックのオーナー終了処理の失敗");

            return true;
        }

        /// <summary>
        /// SpaceOwnerの操作禁止フラグをセット
        /// </summary>
        /// <param name="flag">有効／無効</param>
        public void SetSpaceOwnerInputBan(bool flag)
        {
            spaceOwner.GetComponent<SpaceOwner>().InputBan = flag;
        }

        /// <summary>
        /// PlayerControllerの操作禁止フラグをセット
        /// </summary>
        /// <param name="flag">有効／無効</param>
        public void SetPlayerControllerInputBan(bool flag)
        {
            Player.GetComponent<PlayerController>().InputBan = flag;
        }

        /// <summary>
        /// ドローンの操作制御
        /// </summary>
        /// <param name="active">操作可否フラグ</param>
        /// <returns>成功／失敗</returns>
        public bool SetAutoDroneMoveEnable(bool active)
        {
            return autoDroneOwner.GetComponent<AutoDroneOwner>().SetAutoDroneMoveEnable(active);
        }

        /// <summary>
        /// スカイボックスを設定
        /// SceneOwnerからの呼び出し
        /// </summary>
        /// <param name="idx">パターン番号</param>
        /// <returns></returns>
        public bool SetRenderSkybox(RenderSettingsSkybox idx)
        {
            return skyBoxSet.GetComponent<SkyBoxSet>().SetRenderSkybox(idx);
        }

        /// <summary>
        /// ゴールポイント初期処理
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool GoalPointInitialize()
        {
            return GoalPoint.GetComponent<GoalPoint>().Initialize();
        }

        /// <summary>
        /// ぼろいブロック・天井の初期処理
        /// ディレイ付きの初期処理
        /// ※空間操作ブロックの衝突判定のタイミングより後に実行させる必要があり、暫定対応
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool DelayInitializeBreakBlocks()
        {
            return breakBlookOwner.GetComponent<BreakBlookOwner>().DelayInitializeBreakBlocks();
        }

        /// <summary>
        /// ぼろいブロック・天井の監視の停止
        /// </summary>
        public void DisposeAll()
        {
            breakBlookOwner.GetComponent<BreakBlookOwner>().DisposeAll();
        }

        /// <summary>
        /// カウントダウン表示を更新
        /// </summary>
        /// <param name="count">コネクト回数</param>
        /// <param name="maxCount">クリア条件のコネクト必要回数</param>
        /// <returns>成功／失敗</returns>
        public bool UpdateCountDown(int count, int maxCount)
        {
            return GoalPoint.GetComponent<GoalPoint>().UpdateCountDown(count, maxCount);
        }

        /// <summary>
        /// 条件付きブロックのカウントダウン
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool UpdateCountDownConditionalBlock()
        {
            return conditionalBlockOwner.GetComponent<ConditionalBlockOwner>().UpdateCountDownConditionalBlock();
        }

        /// <summary>
        /// ON/OFFブロックのステータス変更
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool UpdateOnOffState()
        {
            return switchOnOffBlockOwner.GetComponent<SwitchOnOffBlockOwner>().UpdateOnOffState();
        }

        /// <summary>
        /// ドアを開く
        /// ゴール演出のイベント
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool OpenDoor()
        {
            return GoalPoint.GetComponent<GoalPoint>().OpenDoor();
        }

        /// <summary>
        /// プレイヤー操作指令を実行して、実行結果を返却する
        /// </summary>
        /// <param name="moveVelocity">移動座標</param>
        /// <returns>成功／失敗</returns>
        public bool MoveCharactorPlayer(Vector3 moveVelocity)
        {
            _omnibusCall = OmnibusCallCode.MoveCharactorFromSpaceOwner;
            return Player.GetComponent<PlayerController>().MoveCharactorPlayer(moveVelocity);
        }

        /// <summary>
        /// オーナーからCharactorのステータスを変更
        /// </summary>
        /// <param name="isEnabled">有効／無効フラグ</param>
        /// <returns>成功／失敗</returns>
        public bool ChangeCharactorControllerStatePlayer(bool isEnabled)
        {
            return Player.GetComponent<PlayerController>().ChangeCharactorControllerStatePlayer(isEnabled);
        }

        /// <summary>
        /// オーナーからCharactorのステータスを変更
        /// </summary>
        /// <param name="isEnabled">有効／無効フラグ</param>
        /// <param name="origin">対象オブジェクト</param>
        /// <returns>成功／失敗</returns>
        public bool ChangeCharactorControllerStateRobotEnemy(bool isEnabled, GameObject origin)
        {
            return robotEnemiesOwner.GetComponent<RobotEnemiesOwner>().ChangeCharactorControllerStateRobotEnemy(isEnabled, origin);
        }

        /// <summary>
        /// オーナーからCharactorのステータスを変更
        /// </summary>
        /// <param name="isEnabled">有効／無効フラグ</param>
        /// <param name="origin">対象オブジェクト</param>
        /// <returns>成功／失敗</returns>
        public bool ChangeCapsuleColliderStateRobotEnemy(bool isEnabled, GameObject origin)
        {
            return robotEnemiesOwner.GetComponent<RobotEnemiesOwner>().ChangeCapsuleColliderStateRobotEnemy(isEnabled, origin);
        }

        /// <summary>
        /// オーナーからRigidbodyのステータスを変更
        /// </summary>
        /// <param name="isEnabled">有効／無効フラグ</param>
        /// <param name="origin">対象オブジェクト</param>
        /// <returns>成功／失敗</returns>
        public bool ChangeRigidBodyStateMoveCube(bool isKinematic, GameObject origin)
        {
            return spaceOwner.GetComponent<SpaceOwner>().ChangeRigidBodyStateMoveCube(isKinematic, origin);
        }

        /// <summary>
        /// オーナーからRigidbodyのステータスを変更
        /// </summary>
        /// <param name="isEnabled">有効／無効フラグ</param>
        /// <param name="origin">対象オブジェクト</param>
        /// <returns>成功／失敗</returns>
        public bool ChangeBoxColliderStateMoveCube(bool isEnabled, GameObject origin)
        {
            return spaceOwner.GetComponent<SpaceOwner>().ChangeBoxColliderStateMoveCube(isEnabled, origin);
        }

        /// <summary>
        /// 敵操作指令を実行して、実行結果を返却する
        /// </summary>
        /// <param name="moveVelocity">移動座標</param>
        /// <returns>成功／失敗</returns>
        public bool MoveCharactorRobotEnemy(Vector3 moveVelocity, GameObject hitObject)
        {
            //return hitObject.GetComponent<Robot_Enemy>().MoveRobotEnemy(moveVelocity);
            return robotEnemiesOwner.GetComponent<RobotEnemiesOwner>().MoveCharactorRobotEnemy(moveVelocity, hitObject);
        }

        /// <summary>
        /// MoveCubeをRigidBodyから動かす
        /// </summary>
        /// <param name="moveVelocitySpace">移動ベクトル</param>
        /// <param name="origin">対象オブジェクト</param>
        /// <returns>移動処理完了／RigidBodyの一部がnull</returns>
        public bool MoveRigidBodyMoveCube(Vector3 moveVelocitySpace, GameObject origin)
        {
            return spaceOwner.GetComponent<SpaceOwner>().MoveRigidBodyMoveCube(moveVelocitySpace, origin);
        }

        /// <summary>
        /// 重力操作ギミックオブジェクトからプレイヤーオブジェクトへ
        /// プレイヤー操作指令を実行して、実行結果を返却する
        /// </summary>
        /// <param name="moveVelocity">移動座標</param>
        /// <returns>成功／失敗</returns>
        public bool MoveCharactorPlayerOrWait(Vector3 moveVelocity)
        {
            // 空間操作による呼び出しがあるなら実行しない
            if (_omnibusCall.Equals(OmnibusCallCode.MoveCharactorFromSpaceOwner))
                return true;

            _omnibusCall = OmnibusCallCode.MoveCharactorFromGravityController;
            return Player.GetComponent<PlayerController>().MoveCharactorPlayer(moveVelocity);
        }

        /// <summary>
        /// プレイヤーを死亡させる
        /// </summary>
        /// <returns>成功／失敗</returns>
        public async Task<bool> DestroyPlayer()
        {
            return await Player.GetComponent<PlayerController>().DestroyPlayer();
        }

        /// <summary>
        /// 敵ギミックを破壊する
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool DestroyRobotEnemies(GameObject hitObject)
        {
            return hitObject.GetComponent<Robot_Enemy>().DestroyRobotEnemies();
        }

        /// <summary>
        /// ワープ可否のステータスを切り替える
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool SwitchWarpState()
        {
            return warpGateOwner.GetComponent<WarpGateOwner>().SwitchWarpState();
        }

        /// <summary>
        /// 新たに空間操作ブロックを生成
        /// </summary>
        /// <param name="target">生成する座標</param>
        /// <returns>成功／失敗</returns>
        public bool CreateNewMoveCube(Vector3 target)
        {
            return spaceOwner.GetComponent<SpaceOwner>().CreateNewMoveCube(target);
        }
    }
}
/// <summary>
/// メソッドをコールする際に複数実行の場合、優先順位を決める
/// </summary>
public enum OmnibusCallCode
{
    None,
    MoveCharactorFromSpaceOwner,
    MoveCharactorFromGravityController
}
