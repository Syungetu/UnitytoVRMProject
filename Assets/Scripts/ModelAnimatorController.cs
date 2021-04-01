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
    /// <param name="value"> アニメーション名 </param>
    /// <param name="layer"> アニメーションのあるレイヤー番号（0～）  </param>
    public void SetChangeAnimator(string value, int layer = 0)
    {
        if(_ModelAnimator == null)
        {
            return;
        }
        if (_ModelAnimator.layerCount >= layer)
        {
            // アニメーションを切り替える
            _ModelAnimator.Play(value, layer);
        }
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
    /// アニメーションレイヤーのアニメーションの重さを変化させる
    /// </summary>
    /// <param name="layer"> 重さを変えるレイヤー番号（0～） </param>
    /// <param name="weight"> 重さ（０～１） </param>
    public void SetByLayerChangeAnimator(int layer, float weight)
    {
        if (_ModelAnimator == null)
        {
            return;
        }

        // 指定したレイヤー番号が超えてたら処理しない
        if (_ModelAnimator.layerCount >= layer)
        {
            _ModelAnimator.SetLayerWeight(layer, weight);
        }
    }

    /// <summary>
    /// 【仮配置】仮でスライダーからモーションブレンドの数値を受け取る変数
    /// </summary>
    /// <param name="value">強さ</param>
    public void SetSliderChangeValue(float value)
    {
        SetMotionBlendValue("WalkAndRunSpeed", value);
    }

    /// <summary>
    /// 【仮配置】仮でアニメーションを切り替えるボタンを押した時
    /// </summary>
    /// <param name="value">強さ</param>
    public void SetOnClickChangeButton(string value)
    {
        SetChangeAnimator(value);
    }

    /// <summary>
    /// 【仮配置】仮でスライダーからレイヤーの重さの数値を受け取る変数
    /// </summary>
    /// <param name="value">重さ</param>
    public void SetLayerWeightSliderChangeValue(float value)
    {
        SetByLayerChangeAnimator(2, value);
    }
}
