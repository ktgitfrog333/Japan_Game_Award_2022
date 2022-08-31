using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOnOFFBlock : MonoBehaviour
{
    [SerializeField] private bool isEnabled;    //ブロックの有効／無効ステータス（有効ならtrue、無効ならfalse）
    [SerializeField] private float enabledEndValue; //有効状態のフェード値
    [SerializeField] private float disabledEndValue;    //無効状態のフェード値
    [SerializeField] private float doFadeDuration;  //フェードアニメーションの遷移時間
    private bool _defaultIsEnabled; //有効／無効ステータスの初期値
    private Renderer _renderer; //オブジェクトのマテリアル等の情報
    private BoxCollider _collider;  //コライダー
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
