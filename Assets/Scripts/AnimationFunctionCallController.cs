using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// アニメーションからの関数呼び出しを管理する
/// </summary>
public class AnimationFunctionCallController : MonoBehaviour
{

    /// <summary> アニメーション開始時に呼び出す関数 </summary>
    [SerializeField]
    public UnityEvent _CallStartAnimationFunction = null;
    /// <summary> アニメーション終了時に呼び出す関数 </summary>
    [SerializeField]
    public UnityEvent _CallEndAnimationFunction = null;

    /// <summary> ジャンプ開始と終了に呼び出す関数 </summary>
    [SerializeField]
    public UnityEvent _CallStartEndJumpAnimationFunction = null;

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
    /// アニメーション開始時に呼ばれる関数
    /// </summary>
    public void CallStartAnimation()
    {
        if(_CallStartAnimationFunction != null)
        {
            _CallStartAnimationFunction.Invoke();
        }

        Debug.Log("アニメーションの再生を開始");
    }

    /// <summary>
    /// アニメーション終了時に呼ばれる関数
    /// </summary>
    public void CallEndAnimation()
    {
        if(_CallEndAnimationFunction != null)
        {
            _CallEndAnimationFunction.Invoke();
        }
        Debug.Log("アニメーションの再生を終了");
    }

    /// <summary>
    /// ジャンプのアニメーション開始と開始時に呼ばれる関数
    /// </summary>
    public void CallStartEndJumpAnimation()
    {
        if (_CallStartEndJumpAnimationFunction != null)
        {
            _CallStartEndJumpAnimationFunction.Invoke();
        }

        Debug.Log("ジャンプアニメーションの再生を開始");
    }

}
