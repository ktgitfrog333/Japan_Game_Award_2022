using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Main.Common;

namespace Gimmick
{
    /// <summary>
    /// ON/OFFブロック
    /// </summary>
    public class SwitchOnOffBlock : MonoBehaviour, IOwner
    {
        /// <summary>ブロックの有効／無効ステータス（有効ならtrue、無効ならfalse）</summary>
        [SerializeField] private bool isEnabled = true;
        /// <summary>有効状態のフェード値</summary>
        [SerializeField] private float enabledEndValue = 1.0f;
        /// <summary>無効状態のフェード値</summary>
        [SerializeField] private float disabledEndValue = .25f;
        /// <summary>フェードアニメーションの遷移時間</summary>
        [SerializeField] private float doFadeDuration = .1f;
        /// <summary>有効／無効ステータスの初期値</summary>
        private bool _defaultIsEnabled;
        /// <summary>オブジェクトのマテリアル等の情報</summary>
        private Renderer _renderer;
        /// <summary>コライダー</summary>
        private BoxCollider _collider;

        public bool Initialize()
        {
            throw new System.NotImplementedException();
        }

        public bool ManualStart()
        {
            try
            {
                _defaultIsEnabled = isEnabled;
                _renderer = GetComponent<Renderer>();
                _collider = GetComponent<BoxCollider>();

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
            try
            {
                isEnabled = _defaultIsEnabled;
                CheckOnOffState();
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// ステータスチェック
        /// </summary>
        public void CheckOnOffState()
        {
            if (isEnabled)
            {
                _collider.enabled = true;
                _renderer.material.DOFade(endValue: enabledEndValue, duration: doFadeDuration)
                    .SetLink(gameObject);
            }
            else
            {
                _collider.enabled = false;
                _renderer.material.DOFade(endValue: disabledEndValue, duration: doFadeDuration)
                    .SetLink(gameObject);
            }
        }

        /// <summary>
        /// ステータス変更
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool UpdateOnOffState()
        {
            try
            {
                if (isEnabled == true)
                {
                    isEnabled = false;
                }
                else
                {
                    isEnabled = true;
                }
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}
