using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

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
        /// <summary>タイムライン制御</summary>
        private PlayableDirector _playable;

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

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// 初期処理
        /// </summary>
        public void Initialize()
        {
            _playable = GetComponent<PlayableDirector>();
            _playable.Play();
        }

        /// <summary>
        /// 死亡復帰の開始処理
        /// 演出の短縮版
        /// </summary>
        public void InitializeContinue()
        {
            sootingMovement.SetActive(true);
        }

        /// <summary>
        /// タイムラインを停止
        /// 流星パーティクルオブジェクトからの呼び出し
        /// </summary>
        public void StopPlayAbleFromSootingMovement()
        {
            _playable.Stop();
        }
    }
}
