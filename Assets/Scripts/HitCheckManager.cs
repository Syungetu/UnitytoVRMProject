using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 接触判定管理
/// </summary>
public class HitCheckManager : MonoBehaviour
{

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
        Debug.Log("Collision当たった:" + other.gameObject.name + " → " + this.gameObject.name);
    }

    /// <summary>
    /// 接触判定ありの接触終了の当たり判定
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionExit(Collision other)
    {
        Debug.Log("Collision離れた:" + other.gameObject.name + " → " + this.gameObject.name);
    }

    /// <summary>
    /// 接触判定なしの接触開始の当たり判定
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger当たった:" + other.gameObject.name + " → " + this.gameObject.name);
    }

    /// <summary>
    /// 接触判定なしの接触終了の当たり判定
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger離れた:" + other.gameObject.name + " → " + this.gameObject.name);
    }
}
