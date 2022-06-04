using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Main.Common;
using TMPro;
using System.Text;
using UniRx;
using UniRx.Triggers;
using Main.Level;
using Main.UI;

namespace Main.Direction
{
    /// <summary>
    /// ステージ開始演出
    /// </summary>
    public class StartCutscene : MonoBehaviour
    {
        /// <summary>ステージ名の表示</summary>
        [SerializeField] private GameObject cutSceneScreen;
        /// <summary>流星パーティクル</summary>
        [SerializeField] private GameObject sootingMovement;
        /// <summary>流星パーティクルの位置</summary>
        [SerializeField] private Vector3 sootingMovementLocalPosition = new Vector3(0f, 8.48f, 0f);
        /// <summary>拡散パーティクル</summary>
        [SerializeField] private GameObject diffusionLargePrefab;
        /// <summary>タイムライン制御</summary>
        private PlayableDirector _playable;
        /// <summary>コンティニューフラグ</summary>
        public bool Continue { get; set; }

        private void Reset()
        {
            if (cutSceneScreen == null)
            {
                cutSceneScreen = GameObject.Find("CutsceneScreen");
                cutSceneScreen.SetActive(false);
            }
            if (sootingMovement == null)
            {
                sootingMovement = GameObject.Find("DiffusonShootingStar");
                sootingMovement.SetActive(false);
            }
        }

        /// <summary>
        /// 初期処理
        /// </summary>
        public void Initialize()
        {
            if (!Continue)
            {
                // ステージ新規読み込み

                // ステージ情報読み込み
                var stage = SceneOwner.Instance.LevelDesign.transform.GetChild(SceneOwner.Instance.SceneIdCrumb.Current).gameObject;
                sootingMovement.transform.parent = stage.transform;
                sootingMovement.transform.localPosition = sootingMovementLocalPosition;

                // ステージテキストを変更
                var title = new StringBuilder().Append("ステージ")
                    .Append(SceneOwner.Instance.SceneIdCrumb.Current + 1);
                cutSceneScreen.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = title.ToString();

                // プレイエイブル再生（再生終了のタイミングでプレイヤーを有効）
                _playable = GetComponent<PlayableDirector>();
                _playable.Play();
            }
            else
            {
                // コンティニュー

                // プレイヤーを有効
                LevelOwner.Instance.Player.SetActive(true);
                // 拡散パーティクルのみ発生させる
                var obj = Instantiate(diffusionLargePrefab, LevelOwner.Instance.Player.transform.position, Quaternion.identity);
                var comp = obj.GetComponent<DiffusionLarge>().Completed;
                comp.ObserveEveryValueChanged(x => x.Value)
                    .Where(x => x)
                    .Subscribe(_ =>
                    {
                        Destroy(obj);
                    });
                // 空間操作を許可
                LevelOwner.Instance.SpaceOwner.GetComponent<SpaceOwner>().InputBan = false;
                // ショートカット入力を許可
                UIOwner.Instance.ShortcuGuideScreen.GetComponent<ShortcuGuideScreen>().InputBan = false;
                if (!LevelOwner.Instance.DelayInitializeBreakBlocksFromStartCutscene())
                    Debug.Log("ぼろいブロック・天井復活処理の失敗");
            }
        }

        /// <summary>
        /// タイムラインを停止
        /// 流星パーティクルオブジェクトからの呼び出し
        /// </summary>
        public void StopPlayAbleFromSootingMovement()
        {
            _playable.Stop();
            // 空間操作を許可
            LevelOwner.Instance.SpaceOwner.GetComponent<SpaceOwner>().InputBan = false;
            // ショートカット入力を許可
            UIOwner.Instance.ShortcuGuideScreen.GetComponent<ShortcuGuideScreen>().InputBan = false;
            if (!LevelOwner.Instance.DelayInitializeBreakBlocksFromStartCutscene())
                Debug.Log("ぼろいブロック・天井復活処理の失敗");
        }
    }
}
