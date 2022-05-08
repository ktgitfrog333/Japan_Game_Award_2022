using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Main.Direction
{
    /// <summary>
    /// 拡散パーティクル
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class DiffusionLarge : MonoBehaviour
    {
        /// <summary>処理の完了を待ち受ける</summary>
        private BoolReactiveProperty _completed = new BoolReactiveProperty();
        /// <summary>処理の完了を待ち受ける</summary>
        public BoolReactiveProperty Completed => _completed;

        private void OnParticleSystemStopped()
        {
            _completed.Value = true;
        }
    }
}
