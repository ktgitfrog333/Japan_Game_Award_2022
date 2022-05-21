using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Main.Direction
{
    /// <summary>
    /// テキスト効果
    /// </summary>
    public class TextMaterialController : MonoBehaviour
    {
        /// <summary>グロー調整確認用</summary>
        [SerializeField, Range(0, 1)] private float _glowPower = 0f;
        /// <summary>TextMeshPro</summary>
        [SerializeField] private Material _textMaterial;

        private void Start()
        {
            // アニメーションによってパラメータ変更を即反映させる
            this.UpdateAsObservable()
                .Subscribe(_ => _textMaterial.SetFloat("_GlowPower", _glowPower));
        }
    }
}
