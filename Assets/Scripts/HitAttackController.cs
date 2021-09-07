using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 攻撃を加える処理
/// </summary>
public class HitAttackController : MonoBehaviour
{

    /// <summary> 攻撃を加える関数を登録する </summary>
    [Serializable] public class AttackEvent : UnityEvent<GameObject> { };

    /// <summary> 攻撃を加える関数を登録する </summary>
    [SerializeField]
    public AttackEvent _CallAttackHitFunction = null;

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        
    }

    /// <summary>
    /// 接触判定ありの接触開始の当たり判定
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionStay(Collision other)
    {
        if(_CallAttackHitFunction != null)
        {
            // 接触したときのみ動くようにする
            _CallAttackHitFunction.Invoke(other.gameObject);
        }
    }

    /// <summary>
    /// 接触判定なしの接触開始の当たり判定
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (_CallAttackHitFunction != null)
        {
            // 接触したときのみ動くようにする
            _CallAttackHitFunction.Invoke(other.gameObject);
        }
    }


}
