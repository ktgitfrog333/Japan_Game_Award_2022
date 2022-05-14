using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallFoor : MonoBehaviour
{
    //　床が落下するまでの時間
    [SerializeField] private float timeToFall = 5f;
    private Rigidbody rigid;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //
    }

    public void OnCollisionEnter(Collision collision)
    {
        //　床が落下し、衝突したゲームオブジェクトのレイヤーがFieldだった時床を消去
        if(collision.gameObject.layer == LayerMask.NameToLayer("MoveCubeGroup"))
        {
            Destroy(this.gameObject);
        }
    }
}
