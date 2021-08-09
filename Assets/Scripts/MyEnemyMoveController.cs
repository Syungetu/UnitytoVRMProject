using System.Collections;
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
    /// <summary> プレイヤーを追跡する距離 </summary>
    public float _ChasingDistance = 5.0f;
    /// <summary> 回転速度 </summary>
    public float _RotVelocity = 0.2f;

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        // 念の為にモーションを設定しておく
        _ModelAnimatorController.SetChangeAnimator("Blend Tree");
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
        }

        // 目標と自分の距離を計算する
        if (_ChasingDistance > Vector3.Distance(_ModelObject.transform.position, _TargetVector3))
        {
            // 追跡距離内
            // 目標を追いかける
            _Agent.SetDestination(_TargetVector3);
        }
        else
        {
            // 追跡距離外
            // 初期位置に戻る
            _Agent.SetDestination(_InitialPosVector3);
        }

        // ナビメッシュの移動をメインのオブジェクトに反映させる
        _ModelObject.transform.position = _Agent.transform.position;

        // 動きに合わせて回転させる
        if (_Agent.velocity.magnitude > 0.0f)
        {
            Quaternion targetRot = _ModelObject.transform.rotation;
            // 移動していない
            if (_Agent.velocity.x != 0.0f || _Agent.velocity.z != 0.0f)
            {
                targetRot = Quaternion.LookRotation(new Vector3(
                    _Agent.velocity.x,
                    0.0f,
                    _Agent.velocity.z
                ));
            }

            // 移動中なら移動先にオブジェクトを回転させる(Y軸の方向に対して有効にする)
            _ModelObject.transform.rotation = Quaternion.Slerp(
                _ModelObject.transform.rotation,
                targetRot,
                _RotVelocity
            );

            // スピードからアニメーションのブレンド率を決める
            _ModelAnimatorController.SetMotionBlendValue("Speed", _Agent.velocity.magnitude / _Agent.speed);
        }
        else
        {
            // 移動させてない
            _ModelAnimatorController.SetMotionBlendValue("Speed", 0);
        }
    }
}
