using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Common;
using UniRx;
using UniRx.Triggers;
using Main.Common.Const;
using Main.UI;
using UnityEngine.UI;
using Main.Level;
using System.Linq;

namespace Gimmick
{
    /// <summary>
    /// 条件付きブロック
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class ConditionalBlock : MonoBehaviour, IOwner
    {
        /// <summary>操作可能ブロックに切り替わるまでの回数</summary>
        [SerializeField, Range(1, 30)] private int movingCountDown = 1;
        /// <summary>カウントゼロ時のエフェクト用プレハブ</summary>
        [SerializeField] private GameObject conditionalBlockDiffusionCubesPrefab;
        /// <summary>操作可能ブロックに切り替わるまでの回数（監視用）</summary>
        private IntReactiveProperty _mCountDown;
        /// <summary>監視管理</summary>
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        /// <summary>コライダー</summary>
        private BoxCollider _collider;
        /// <summary>レンダラー</summary>
        private Renderer _renderer;


        public bool ManualStart()
        {
            try
            {
                var level = GameManager.Instance.LevelOwner.GetComponent<LevelOwner>();

                // 条件付きブロックのコライダーを切り替える
                _collider = GetComponent<BoxCollider>();
                _renderer = GetComponent<Renderer>();

                _mCountDown = new IntReactiveProperty(movingCountDown);
                _mCountDown.ObserveEveryValueChanged(x => x.Value)
                    .Subscribe(x =>
                    {
                        if (x < 1)
                        {
                            // 0になったら空間操作ブロックへ変更
                            Debug.Log("空間操作ブロックへ変更");
                            _collider.enabled = false;
                            _renderer.enabled = false;
                            transform.GetChild(0).GetComponent<TextMesh>().text = "";
                            if (!level.CreateNewMoveCube(transform.position))
                                throw new System.Exception("空間操作ブロック生成の失敗");
                            if (!PlayEffectInPoolGroup(conditionalBlockDiffusionCubesPrefab))
                                throw new System.Exception("エフェクト再生の失敗");
                        }
                        else
                        {
                            // カウンターを更新
                            transform.GetChild(0).GetComponent<TextMesh>().text = x + "";
                        }
                    })
                    .AddTo(_compositeDisposable);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// エフェクトを再生する
        /// </summary>
        /// <param name="prefabs">ブロック破砕エフェクトのオブジェクト</param>
        /// <returns>成功／失敗</returns>
        private bool PlayEffectInPoolGroup(GameObject prefabs)
        {
            try
            {
                Instantiate(conditionalBlockDiffusionCubesPrefab, transform.position, Quaternion.identity);

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 条件付きブロックのカウントダウン
        /// </summary>
        /// <returns>成功／失敗</returns>
        public bool UpdateCountDownConditionalBlock()
        {
            try
            {
                if (_mCountDown != null && 0 < _mCountDown.Value)
                {
                    _mCountDown.Value--;
                }
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
                _compositeDisposable.Clear();
                _collider.enabled = true;
                _renderer.enabled = true;
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Initialize()
        {
            throw new System.NotImplementedException();
        }
    }
}
