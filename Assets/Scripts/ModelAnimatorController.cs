using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アニメーション管理
/// </summary>
public class ModelAnimatorController : MonoBehaviour
{
    /// <summary> モデルのAnimator </summary>
    public Animator _ModelAnimator;

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        SetChangeAnimator("Idle001");
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        
    }

    /// <summary>
    /// アニメーションを変更する
    /// </summary>
    /// <param name="value"></param>
    public void SetChangeAnimator(string value)
    {
        if(_ModelAnimator == null)
        {
            return;
        }
        // アニメーションを切り替える
        _ModelAnimator.Play(value, 0);
    }

    /// <summary>
    /// モーションブレンドの強さを変更する
    /// </summary>
    /// <param name="value">ブレンドする変数名</param>
    /// <param name="value">強さ</param>
    public void SetMotionBlendValue(string name, float value)
    {
        if (_ModelAnimator == null)
        {
            return;
        }
        _ModelAnimator.SetFloat(name, value);
    }

    /// <summary>
    /// 仮でスライダーからモーションブレンドの数値を受け取る変数
    /// </summary>
    /// <param name="value">強さ</param>
    public void SetSliderChangeValue(float value)
    {
        SetMotionBlendValue("WalkAndRunSpeed", value);
    }
}
