using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ChangeGravity : MonoBehaviour
{
    [SerializeField] private Vector3 localGravity;
    [SerializeField] private Direction direction;
    [SerializeField] private TriggerEvent onTriggerEnter = new TriggerEvent();
    [SerializeField] private TriggerEvent onTriggerStay = new TriggerEvent();
    private Rigidbody rBody;

    // Start is called before the first frame update
    void Start()
    {
        rBody = this.GetComponent<Rigidbody>();
        rBody.useGravity = false; //最初にrigidBodyの重力を使わなくする
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    private void FixedUpdate()
    {
        SetLocalGravity(); //重力をAddForceでかけるメソッドを呼ぶ。FixedUpdateが好ましい。
    }

    private void SetLocalGravity()
    {
        rBody.AddForce(localGravity, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke(other);
    }

    /// <summary>
    /// Is TriggerがONで他のColliderと重なっているときは、このメソッドが常にコールされる
    /// </summary>
    /// <param name="other"></param>

    private void OnTriggerStay(Collider other)
    {
        //onTriggerStayで指定された処理を実行する
        onTriggerStay.Invoke(other);
    }

    //UnityEventを継承したクラスに[Serializable]属性を付与することで、Inspectorウィンドウ上に表示できるようになる。
    [Serializable]
    public class TriggerEvent : UnityEvent<Collider>
    { }
}

public enum Direction
{
    /// <summary>上</summary>
    UP
    /// <summary>下</summary>
    , DOWN
    /// <summary>左</summary>
    , LEFT
    /// <summary>右</summary>
    , RIGHT
}
