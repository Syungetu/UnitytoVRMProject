﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敵の動きを管理する
/// </summary>
public class MyEnemyMoveController : MonoBehaviour
{
    /// <summary> 動かす敵オブジェクト </summary>
    public GameObject _ModelObject;
    /// <summary> 敵を動かすナビメッシュエージェント </summary>
    public NavMeshAgent _Agent;
    /// <summary> モデル本体の物理演算 </summary>
    public Rigidbody _Rigidbody;
    /// <summary> アニメーションを管理する </summary>
    public ModelAnimatorController _ModelAnimatorController;
    /// <summary> 移動目標のオブジェクト（NullでもOK） </summary>
    public GameObject _TargetObject;
    /// <summary> 移動目標の座標 </summary>
    public Vector3 _TargetVector3;
    /// <summary> 初期位置のオブジェクト（NullでもOK） </summary>
    public GameObject _InitialPosObject;
    /// <summary> 初期位置の座標 </summary>
    public Vector3 _InitialPosVector3;
    /// <summary> 初期姿勢の座標 </summary>
    public Vector3 _InitialRotVector3;
    /// <summary> プレイヤーを追跡する距離 </summary>
    public float _ChasingDistance = 5.0f;
    /// <summary> 回転速度 </summary>
    public float _RotVelocity = 0.2f;
    /// <summary> 停止距離 </summary>
    public float _StoppingDistance = 1.25f;

    /// <summary> 初期位置に戻る </summary>
    private bool _IsReturnToInitialPosition = false;

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        // 念の為にモーションを設定しておく
        _ModelAnimatorController.SetChangeAnimator("Blend Tree");

        // 初期位置に戻っているかどうか
        _IsReturnToInitialPosition = false;
        // 自動で移動を止めるようにする
        _Agent.autoBraking = true;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        if(_TargetObject != null)
        {
            // オブジェクト指定だった場合、そちらの座標を取得する
            _TargetVector3 = _TargetObject.transform.position;
        }

        if(_InitialPosObject != null)
        {
            // オブジェクト指定だった場合、そちらの座標を取得する
            _InitialPosVector3 = _InitialPosObject.transform.position;
            _InitialRotVector3 = _InitialPosObject.transform.rotation.eulerAngles;
        }

        // 目標と自分の距離を計算する
        if (_ChasingDistance > Vector3.Distance(_ModelObject.transform.position, _TargetVector3))
        {
            // 追跡距離内
            // 目標を追いかける
            _Agent.SetDestination(_TargetVector3);
            _Agent.stoppingDistance = _StoppingDistance;
            _IsReturnToInitialPosition = false;
        }
        else
        {
            // 追跡距離外
            // 初期位置に戻る
            _Agent.SetDestination(_InitialPosVector3);
            _Agent.stoppingDistance = 0.0f;
            _IsReturnToInitialPosition = true;
        }

        Quaternion targetRot = _ModelObject.transform.rotation;

        // 動きに合わせて回転させる
        if (_Agent.velocity.magnitude > 0.0f)
        {
            // 移動している
            // 移動している方向を向く
            targetRot = Quaternion.LookRotation(_Agent.velocity.normalized);
        }
        else
        {
            // 移動させてない
            if (_IsReturnToInitialPosition == true)
            {
                // 初期位置の方向を向く
                targetRot = Quaternion.Euler(
                    _ModelObject.transform.eulerAngles.x, 
                    _InitialRotVector3.y,
                    _ModelObject.transform.eulerAngles.z
                );
            }
            else
            {
                // 相手の方向を向く
                targetRot = Quaternion.LookRotation(
                    new Vector3(_TargetVector3.x, 0.0f, _TargetVector3.z) - 
                    new Vector3(_ModelObject.transform.position.x, 0.0f, _ModelObject.transform.position.z)
                );
            }
        }

        // 移動中なら移動先にオブジェクトを回転させる(Y軸の方向に対して有効にする)
        if (_Rigidbody != null)
        {
            // 物理演算を含めた回転
            _Rigidbody.MoveRotation(Quaternion.Slerp(
                _ModelObject.transform.rotation,
                targetRot,
                _RotVelocity
            ));
        }
        else
        {
            // 姿勢を直接変更
            _ModelObject.transform.rotation = Quaternion.Slerp(
                _ModelObject.transform.rotation,
                targetRot,
                _RotVelocity
            );
        }

        // ナビメッシュの移動をメインのオブジェクトに反映させる
        if (_Rigidbody != null)
        {
            // 物理演算を含めた移動
            _Rigidbody.MovePosition(_Agent.transform.position);
        }
        else
        {
            // 座標を直接変更
            _ModelObject.transform.position = _Agent.transform.position;
        }

        // スピードからアニメーションのブレンド率を決める
        _ModelAnimatorController.SetMotionBlendValue("Speed", _Agent.velocity.magnitude / _Agent.speed);
    }
}
