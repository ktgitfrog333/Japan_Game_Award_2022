using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallFoor : MonoBehaviour
{
    //　床が落下するまでの時間
    [SerializeField] private float timeToFall = 5f;
    //　主人公が床に乗っていたトータル時間
    private float totalTime = 0f;
    private Rigidbody rigid;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        //　床が落下する時間を超えたらリジッドボディのisKinematicをfalseに
        //　isKinematicをfalseにしたことで重力が働く
        if(totalTime >= timeToFall)
        {
            rigid.isKinematic = false;
        }
    }

    //　主人公が床に乗っている時に呼び出す
    public void ReceiveForce()
    {
        totalTime += Time.deltaTime;
    }

    public void OnCollisionEnter(Collision collision)
    {
        //　床が落下し、衝突したゲームオブジェクトのレイヤーがFieldだった時床を消去
        if(collision.gameObject.layer == LayerMask.NameToLayer("Field"))
        {
            Destroy(this.gameObject, 1f);
        }
    }
}
