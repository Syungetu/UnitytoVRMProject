using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターの動き管理（ユーザー側）
/// </summary>
public class MyCharacterController : MonoBehaviour
{
    /// <summary> 動かしたいゲームオブジェクト </summary>
    public GameObject _MainObject;
    /// <summary> モデルの動き管理コントローラー </summary>
    public CharacterController _CharacterController;
    /// <summary> アニメーションを管理するコントローラー </summary>
    public ModelAnimatorController _ModelAnimatorController;
    /// <summary> カメラオブジェクト </summary>
    public GameObject _CameraObject;
    /// <summary> 移動速度 </summary>
    public float _MoveVelocity = 2.0f;
    /// <summary> ジャンプするときの上向きの力 </summary>
    public float _JumpPower = 1.0f;
    /// <summary> ジャンプするときの最大の力 </summary>
    public float _JumpMaxPower = 20.0f;
    /// <summary> 回転速度 </summary>
    public float _RotVelocity = 0.2f;

    /// <summary> 移動しはじめを検知する </summary>
    private bool _IsMoveStart = false;
    /// <summary> ジャンプしはじめを検知する </summary>
    private bool _IsJumpStart = false;
    /// <summary> 現在のジャンプ力 </summary>
    private float _JumpNowPower = 0.0f;

    /// <summary> 特殊Motionフラグ </summary>
    private bool _IsSpecialMotion = false;

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        _IsMoveStart = false;
        _IsJumpStart = false;
        _IsSpecialMotion = false;
        _JumpNowPower = 0.0f;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        if (_IsJumpStart == true)
        {
            // ジャンプ中
            // 徐々に力を強める（急激に飛ばないようにする）
            _JumpNowPower += _JumpPower;
            if (_JumpNowPower >= _JumpMaxPower)
            {
                // 最大まで足されたらジャンプ上昇終了
                _IsJumpStart = false;
            }
        }
        else
        {
            // 下降中
            // 下降中は徐々に力を減らす
            _JumpNowPower -= _JumpPower;
            if (_JumpNowPower < 0.0f)
            {
                //地面に埋まらないように
                _JumpNowPower = 0.0f;
            }
        }

        // 特殊モーション
        if(_IsSpecialMotion == true)
        {
            _ModelAnimatorController.SetChangeAnimator("jackochallange");
            return;
        }

        // キー入力に合わせて移動方向をセットする（-１～１）
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));

        // カメラオブジェクトが指定されていたときに処理をする
        if(_CameraObject != null)
        {
            // カメラの向きに合わせて移動方向を設定する
            direction = (Input.GetAxis("Vertical") * _CameraObject.transform.forward) + 
                (Input.GetAxis("Horizontal") * _CameraObject.transform.right);
        }

        // 移動モーションを設定する
        float animSpeed = Mathf.Max(Mathf.Abs(Input.GetAxis("Horizontal")), Mathf.Abs(Input.GetAxis("Vertical")));
        if (animSpeed > 0.0f)
        {
            // 移動している
            if (_IsMoveStart == false)
            {
                // 移動しはじめの一瞬だけ検知する
                _ModelAnimatorController.SetChangeAnimator("WalkAndRunBlendTree");
            }
            _ModelAnimatorController.SetMotionBlendValue("WalkAndRunSpeed", animSpeed);
            _IsMoveStart = true;
        }
        else
        {
            // 止まっている
            _ModelAnimatorController.SetChangeAnimator("Idle001");
            _IsMoveStart = false;
        }

        // ジャンプボタンが押された瞬間
        if (Input.GetButtonDown("Jump") == true && _IsJumpStart == false)
        {
            _IsJumpStart = true;
        }

        // 速度を追加する
        direction *= _MoveVelocity;
        // ジャンプ力を追加する
        direction.y = _JumpNowPower;
        // 重力落下を追加する
        direction.y += Physics.gravity.y;

        // 実際の移動（処理スピードに合わせて移動幅を変化させる）
        _CharacterController.Move(direction * Time.deltaTime);

        if (_CharacterController.velocity.magnitude > 0.0f)
        {
            Quaternion targetRot = _MainObject.transform.rotation;
            // 移動していない
            if (_CharacterController.velocity.x != 0.0f || _CharacterController.velocity.z != 0.0f)
            {
                targetRot = Quaternion.LookRotation(new Vector3(
                    _CharacterController.velocity.x,
                    0.0f,
                    _CharacterController.velocity.z
                ));
            }

            // 移動中なら移動先にオブジェクトを回転させる(Y軸の方向に対して有効にする)
            _MainObject.transform.rotation = Quaternion.Slerp(
                _MainObject.transform.rotation,
                targetRot,
                _RotVelocity
            );
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public void SetSpecialMotion()
    {
        _IsSpecialMotion = !_IsSpecialMotion;
    }

}
