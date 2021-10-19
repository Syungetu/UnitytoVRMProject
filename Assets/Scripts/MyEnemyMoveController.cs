using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敵の動きを管理する
/// </summary>
public class MyEnemyMoveController : MonoBehaviour
{
    /// <summary> キャラクターオブジェクトで使用する処理を纏めたもの </summary>
    public MyCharacterBundledProcessManager _MyCharacterBundledProcessManager;
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
        // 復活処理でデータを初期化する
        SetEnemyReturn();

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
        bool isMove = true;

        // ゲームオーバー
        if (MainSceneManager.CallIsGameOver == true)
        {
            return;
        }

        if (_MyCharacterBundledProcessManager != null)
        {
            // 何らかの要因で移動出来ない
            if (_MyCharacterBundledProcessManager.GetIsMove() == false)
            {
                isMove = false;
            }

            // 体力がなかった場合移動処理をしないようにする
            if (_MyCharacterBundledProcessManager.GetIsDown() == true)
            {
                // やられた時の処理
                _ModelAnimatorController.SetStopAnimator();
                _MyCharacterBundledProcessManager._RagdollController._IsRagdoll = true;

                // やられた時の時間をカウントする
                _MyCharacterBundledProcessManager._CharacterStatusManager._ReturnTimeCount -= Time.deltaTime;

                if(_MyCharacterBundledProcessManager._CharacterStatusManager._ReturnTimeCount <= 0.0f)
                {
                    // 復帰処理
                    SetEnemyReturn();
                }
                isMove = false;
                return;
            }
            else
            {
                // 通常処理
                _MyCharacterBundledProcessManager._CharacterStatusManager._ReturnTimeCount = _MyCharacterBundledProcessManager._CharacterStatusManager._RespawningTimes;
            }
        }

        if (_TargetObject != null)
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
        // 移動処理をさせない
        if (isMove == false)
        {
            _Agent.nextPosition = _ModelObject.transform.position;
            return;
        }
        else
        {
            // ナビメッシュの移動をメインのオブジェクトに反映させる
            if (_Rigidbody != null)
            {
                // 物理演算を含めた移動
                _Rigidbody.velocity = new Vector3(_Agent.velocity.x, _Rigidbody.velocity.y, _Agent.velocity.z);
                //_Agent.nextPosition = _ModelObject.transform.position;
            }
            else
            {
                // 座標を直接変更
                _ModelObject.transform.position = _Agent.transform.position;
            }
        }

        // スピードからアニメーションのブレンド率を決める
        _ModelAnimatorController.SetMotionBlendValue("WalkAndRunSpeed", _Agent.velocity.magnitude / _Agent.speed);
    }

    /// <summary>
    /// すべてのUpdate処理が回った後に処理される関数
    /// </summary>
    private void LateUpdate()
    {
        _Agent.transform.position = _ModelObject.transform.position;
        _Agent.transform.eulerAngles = _ModelObject.transform.eulerAngles;
        _Agent.nextPosition = _ModelObject.transform.position;
    }

    /// <summary>
    /// 復帰処理
    /// </summary>
    private void SetEnemyReturn()
    {
        // アニメーションを再生させる
        _ModelAnimatorController.SetChangeAnimator("WalkAndRunBlendTree");
        // ラグドール化を解除する
        if (_MyCharacterBundledProcessManager != null)
        {
            _MyCharacterBundledProcessManager._RagdollController._IsRagdoll = false;
        }

        // 座標を初期位置にする
        if (_InitialPosObject != null)
        {
            // オブジェクト指定だった場合、そちらの座標を取得する
            _InitialPosVector3 = _InitialPosObject.transform.position;
            _InitialRotVector3 = _InitialPosObject.transform.rotation.eulerAngles;
        }

        // ナビメッシュの移動をメインのオブジェクトに反映させる
        if (_Rigidbody != null)
        {
            _Rigidbody.isKinematic = true;
            _Rigidbody.velocity = Vector3.zero;
            _Rigidbody.MovePosition(_InitialPosVector3);
        }

        _ModelObject.transform.position = _InitialPosVector3;
        _ModelObject.transform.eulerAngles = _InitialRotVector3;

        _Agent.transform.position = _InitialPosVector3;
        _Agent.transform.eulerAngles = _InitialRotVector3;

        _Agent.SetDestination(_InitialPosVector3);
        _Agent.nextPosition = _InitialPosVector3;
        _Agent.Warp(_InitialPosVector3);

        // 体力を回復させる
        if (_MyCharacterBundledProcessManager != null)
        {
            _MyCharacterBundledProcessManager._CharacterStatusManager._HP = _MyCharacterBundledProcessManager._CharacterStatusManager._Max_HP;
        }

        // 初期位置に戻っているかどうか
        _IsReturnToInitialPosition = false;
        // 自動で移動を止めるようにする
        _Agent.autoBraking = true;

        if(_Rigidbody != null)
        {
            _Rigidbody.isKinematic = false;
        }
    }
}
