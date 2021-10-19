using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ゴールオブジェクト処理
/// </summary>
public class GoolPointObjectController : MonoBehaviour
{

    /// <summary> ゴールオブジェクトに接触しているときに処理する関数を登録する </summary>
    [Serializable] public class HitEvent : UnityEvent<GameObject> { };

    /// <summary> ゴールオブジェクトに接触した瞬間に処理する関数を登録する </summary>
    [SerializeField]
    public HitEvent _CallEnterEventFunction = null;

    /// <summary> ゴールオブジェクトに離れた瞬間に処理する関数を登録する </summary>
    [SerializeField]
    public HitEvent _CallExitEventFunction = null;

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
    /// 接触した瞬間に処理する関数
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (_CallEnterEventFunction != null)
        {
            // 接触し続けているときのみ動く
            _CallEnterEventFunction.Invoke(this.gameObject);
        }
    }
    /// <summary>
    /// 離れた瞬間に処理する関数
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        if (_CallExitEventFunction != null)
        {
            // 接触し続けているときのみ動く
            _CallExitEventFunction.Invoke(this.gameObject);
        }
    }
}
