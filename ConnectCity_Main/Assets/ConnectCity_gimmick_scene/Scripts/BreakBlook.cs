using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common.Const;
using Main.Audio;
using UniRx;
using UniRx.Triggers;

namespace Gimmick
{
    /// <summary>
    /// ぼろい天井・床
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class BreakBlook : MonoBehaviour
    {
        /// <summary>SE設定</summary>
        [SerializeField] private ClipToPlay breakBlookSE = ClipToPlay.se_collapse_No1;
        /// <summary>監視管理</summary>
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();

        /// <summary>
        /// 初期処理
        /// </summary>
        public void Initialize()
        {
            var isBreaked = false;
            this.OnCollisionEnterAsObservable()
                .Where(x => x.gameObject.CompareTag(TagConst.TAG_NAME_MOVECUBEGROUP) && 0f < x.relativeVelocity.magnitude && !isBreaked)
                .Subscribe(x =>
                {
                    isBreaked = true;
                    gameObject.GetComponent<BoxCollider>().enabled = false;
                    gameObject.GetComponent<Renderer>().enabled = false;
                    SfxPlay.Instance.PlaySFX(breakBlookSE);
                })
                .AddTo(_compositeDisposable);
        }

        /// <summary>
        /// 管理系の処理を破棄
        /// </summary>
        public void DisposeAll()
        {
            _compositeDisposable.Clear();
        }
    }
}
