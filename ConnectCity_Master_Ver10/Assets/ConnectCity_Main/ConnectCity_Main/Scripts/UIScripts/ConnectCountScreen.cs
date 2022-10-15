using UnityEngine;

namespace Main.UI
{

    /// <summary>
    /// コネクト成功回数のカウントダウン表示UI
    /// </summary>
    public class ConnectCountScreen : MonoBehaviour
    {
        /// <summary>オブジェクトを映すカメラ</summary>
        [SerializeField] private Camera _targetCamera;
        /// <summary>UIを表示させる対象オブジェクト</summary>
        [SerializeField] private Transform _target;
        /// <summary>表示するUI</summary>
        [SerializeField] private Transform _targetUI;
        /// <summary>オブジェクト位置のオフセット</summary>
        [SerializeField] private Vector3 _worldOffset;
        /// <summary>UI位置のオフセット</summary>
        [SerializeField] private Vector2 offset = new Vector2(-30f, 30f);
        /// <summary>パネルの位置</summary>
        private RectTransform _parentUI;

        /// <summary>
        /// 初期化メソッド（Prefabから生成する時などに使う）
        /// </summary>
        /// <param name="target">配置先オブジェクトの位置情報</param>
        /// <param name="targetCamera">カメラの位置情報</param>
        public void Initialize(Transform target, Camera targetCamera = null)
        {
            _target = target;
            _targetCamera = targetCamera != null ? targetCamera : Camera.main;

            OnUpdatePosition();
        }

        private void Awake()
        {
            // カメラが指定されていなければメインカメラにする
            if (_targetCamera == null)
                _targetCamera = Camera.main;

            // 親UIのRectTransformを保持
            _parentUI = _targetUI.parent.GetComponent<RectTransform>();
        }

        /// <summary>
        /// UIの位置を更新する
        /// </summary>
        private void OnUpdatePosition()
        {
            // オブジェクトの位置
            var targetWorldPos = _target.position + _worldOffset;

            // オブジェクトのワールド座標→スクリーン座標変換
            var targetScreenPos = _targetCamera.WorldToScreenPoint(targetWorldPos);

            // スクリーン座標変換→UIローカル座標変換
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentUI,
                targetScreenPos,
                null,
                out var uiLocalPos
            );

            // RectTransformのローカル座標を更新
            _targetUI.localPosition = uiLocalPos + offset;
        }
    }
}
