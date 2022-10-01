using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TitleSelect
{
    /// <summary>
    /// プレイヤーの待機モーション管理
    /// </summary>
    public class PlayerIdleController : MonoBehaviour
    {
        /// <summary>プレイヤーのアニメーション</summary>
        [SerializeField] private Animator _playerAnimator;
        /// <summary>操作状況に応じて変化する光パーティクル</summary>
        [SerializeField] private ParticleSystem signalHeartLight;
        /// <summary>進む信号カラー</summary>
        [SerializeField] private Color GoLight = new Color(0f, 255f, 182f, 255f);

        private void Reset()
        {
            if (_playerAnimator == null)
                _playerAnimator = transform.GetChild(2).GetComponent<Animator>();
            // 操作状態のパーティクル
            if (signalHeartLight == null)
                signalHeartLight = transform.GetChild(2).GetChild(3).GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            var m = signalHeartLight.main;
            signalHeartLight.Play();
            //if (!x)
                m.startColor = GoLight;
        }
    }
}
