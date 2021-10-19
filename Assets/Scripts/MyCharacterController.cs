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
    /// <summary> キャラクターオブジェクトで使用する処理を纏めたもの </summary>
    public MyCharacterBundledProcessManager _MyCharacterBundledProcessManager;
    /// <summary> カメラオブジェクト </summary>
    public GameObject _CameraObject;
    /// <summary> 移動速度 </summary>
    public float _MoveVelocity = 2.0f;
    /// <summary> ジャンプするときの上向きの力 </summary>
    public float _JumpPower = 0.1f;
    /// <summary> ジャンプするときの最大の力 </summary>
    public float _JumpMaxPower = 7.0f;
    /// <summary> 回転速度 </summary>
    public float _RotVelocity = 0.2f;
    /// <summary> 攻撃用の当たり判定オブジェクト </summary>
    public GameObject _AttackHitObject;
    /// <summary> 攻撃用の当たり判定 </summary>
    public BoxCollider _BoxCollider;
    /// <summary> 自分のステータス </summary>
    public CharacterStatusManager _CharacterStatusManager;
    /// <summary> 初期位置のオブジェクト（NullでもOK） </summary>
    public GameObject _InitialPosObject;
    /// <summary> 初期位置の座標 </summary>
    public Vector3 _InitialPosVector3;
    /// <summary> 初期姿勢の座標 </summary>
    public Vector3 _InitialRotVector3;

    /// <summary> 移動しはじめを検知する </summary>
    private bool _IsMoveStart = false;
    /// <summary> ジャンプしはじめを検知する </summary>
    private bool _IsJumpStart = false;
    /// <summary> ジャンプの降下を検知する </summary>
    private bool _IsJumpDown = false;
    /// <summary> 現在のジャンプ力 </summary>
    private float _JumpNowPower = 0.0f;
    /// <summary> ジャンプアニメーションのタイミング </summary>
    private float _JumpAnimeSpeed = 0.0f;
    /// <summary> 攻撃フラグ </summary>
    private bool _IsAttackStart = false;

    /// <summary> 攻撃モーション解除のカウントをする </summary>
    private float _AttackAnimatonTimeCount = 0.0f;
    /// <summary> 特殊Motionフラグ </summary>
    private bool _IsSpecialMotion = false;
    /// <summary> ジャンプアニメーション終了フラグ </summary>
    private bool _IsJumpEndAnimation = false;

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        _IsMoveStart = false;
        _IsJumpStart = false;
        _IsJumpDown = false;
        _IsAttackStart = false;
        _JumpNowPower = 0.0f;
        _JumpAnimeSpeed = 0.0f;

        _AttackAnimatonTimeCount = 0.0f;
        _IsSpecialMotion = false;
        _IsJumpEndAnimation = false;

        SetPlayerReturn();
        // 初期化時に復帰回数がカウントされてしまうので調整
        _CharacterStatusManager._RespawningTimesCount = 0;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        bool isMove = true;

        // ゲームオーバー
        if(MainSceneManager.CallIsGameOver == true)
        {
            return;
        }

        // 体力がなかった場合移動処理をしないようにする
        if (_MyCharacterBundledProcessManager != null)
        {
            // 何らかの要因で移動出来ない
            if (_MyCharacterBundledProcessManager.GetIsMove() == false)
            {
                isMove = false;
            }

            // HPがあるかどうか
            if (_MyCharacterBundledProcessManager.GetIsDown() == true)
            {
                // HPがなくなった場合
                _ModelAnimatorController.SetStopAnimator();
                _MyCharacterBundledProcessManager._RagdollController._IsRagdoll = true;
                
                // カウントを減らす
                _CharacterStatusManager._ReturnTimeCount -= Time.deltaTime;
                if(_CharacterStatusManager._ReturnTimeCount <= 0.0f)
                {
                    // カウントが０になったら復活させる
                    // 復帰処理
                    SetPlayerReturn();
                    return;
                }

                isMove = false;
                return;
            }
            else
            {
                // HPがある場合
                // プレイヤーは3秒で復帰できるようにする
                _CharacterStatusManager._ReturnTimeCount = 3.0f;
            }

            // 無敵期間のカウンドダウン
            if(_CharacterStatusManager._ReturnInvincibleTimeCount > 0.0f)
            {
                _CharacterStatusManager._ReturnInvincibleTimeCount -= Time.deltaTime;
            }
        }

        if(isMove == false)
        {
            return;
        }

        // 地面との接触判定
        bool isHitGround = false;
        Ray ray = new Ray(
            _MainObject.transform.position, 
            _MainObject.transform.up * -1.0f
        );
        // Rayが衝突したコライダーの情報を得る
        RaycastHit hit;
        if (Physics.Raycast(
            ray, 
            out hit,
            _CharacterController.skinWidth + 0.001f))
        {
            isHitGround = true;
        }
        if(_CharacterController.isGrounded == true)
        {
            isHitGround = true;
        }

        if (_InitialPosObject != null)
        {
            // オブジェクト指定だった場合、そちらの座標を取得する
            _InitialPosVector3 = _InitialPosObject.transform.position;
            _InitialRotVector3 = _InitialPosObject.transform.rotation.eulerAngles;
        }

        float buffJumpNowPower = 0.0f;
        if (_IsJumpStart == true)
        {
            // 徐々に力を弱くする
            _JumpNowPower = Mathf.Lerp(_JumpNowPower, _JumpMaxPower, _JumpPower);
            buffJumpNowPower = (_JumpMaxPower - _JumpNowPower) - Physics.gravity.y;
            
            if (_JumpNowPower >= (_JumpMaxPower * 0.9f))
            {
                // 最大まで足されたらジャンプ上昇終了
                _IsJumpStart = false;
                _IsJumpDown = true;
                _JumpNowPower = 0.0f;
            }
        }
        if(_IsJumpDown == true)
        {
            // 徐々に力を強くしていく
            _JumpNowPower = Mathf.Lerp(_JumpNowPower, _JumpMaxPower, _JumpPower);
            buffJumpNowPower = -_JumpNowPower - Physics.gravity.y;
            
            if (_JumpNowPower >= (_JumpMaxPower * 0.9f))
            {
                _IsJumpDown = false;
                //地面に埋まらないように
                _JumpNowPower = 0.0f;
            }
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

        if(isHitGround == false || _JumpAnimeSpeed >= 0.5f)
        {
            // アニメーションの切り替えを判定する
            bool isImmediatelyAfterStart = false;
            if(isHitGround == false && _JumpAnimeSpeed == 0.0f)
            {
                isImmediatelyAfterStart = true;
            }
            if (isHitGround == true && _JumpAnimeSpeed == 0.5f)
            {
                isImmediatelyAfterStart = true;
            }

            _JumpAnimeSpeed += 2.0f * Time.deltaTime;
            if (isHitGround == false)
            {
                if (_JumpAnimeSpeed > 0.5f)
                {
                    _JumpAnimeSpeed = 0.5f;
                }
            }
            else
            {
                if (_IsJumpEndAnimation == true)
                {
                    _JumpAnimeSpeed = 0.0f;
                    _IsJumpEndAnimation = false;
                }
            }

            // アニメーション設定
            if (isImmediatelyAfterStart == true)
            {
                _ModelAnimatorController.SetChangeStartAnimator("JumpBlendTree");
            }

            // モーションブレンド
            if (_JumpAnimeSpeed > 0.5f)
            {
                // 着地
                _ModelAnimatorController.SetMotionBlendValue("JumpBlendTreeSpeed", 1.0f);
                // 着地中は動かせない
                direction = Vector3.zero;
            }
            else
            {
                // ジャンプ上昇から下降まで
                _ModelAnimatorController.SetMotionBlendValue("JumpBlendTreeSpeed", _JumpAnimeSpeed);
            }
            _IsMoveStart = false;
        }
        else
        {
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
        }

        // ジャンプボタンが押された瞬間
        if (Input.GetButtonDown("Jump") == true && _IsJumpStart == false)
        {
            _IsJumpStart = true;
        }

        // 攻撃ボタンを押したとき
        if(Input.GetButtonDown("Fire1") == true)
        {
            _IsAttackStart = true;
        }

        // 速度を追加する
        direction *= _MoveVelocity;
        // ジャンプ力を追加する
        direction.y += buffJumpNowPower;
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

        // 特殊モーション
        if (_IsSpecialMotion == true)
        {
            _ModelAnimatorController.SetChangeAnimator("jackochallange");
            return;
        }

        // 攻撃モーション
        if (_IsAttackStart == true)
        {
            _ModelAnimatorController.SetByLayerChangeAnimator(3, 1.0f);
            _ModelAnimatorController.SetChangeStartAnimator("Attack", 3);
        }
        else
        {
            // 攻撃モーションを解除する
            if(_AttackAnimatonTimeCount > 0.0f)
            {
                _AttackAnimatonTimeCount -= Time.deltaTime;
                if(_AttackAnimatonTimeCount <= 0.0f)
                {
                    // 攻撃モーションを解除する
                    _ModelAnimatorController.SetByLayerChangeAnimator(3, 0.0f);
                }
            }
        }
    }

    /// <summary>
    /// すべてのUpdate処理が回った後に処理される関数
    /// </summary>
    private void LateUpdate()
    {
        _IsAttackStart = false;
    }

    /// <summary>
    /// 例のポーズボタンを押したとき
    /// </summary>
    public void SetSpecialMotion()
    {
        _IsSpecialMotion = !_IsSpecialMotion;
    }

    /// <summary>
    /// 攻撃ボタンを押したとき
    /// </summary>
    public void OnClickAttckButton()
    {
        _IsAttackStart = true;
    }

    /// <summary>
    /// 攻撃アニメーションが開始されたとき
    /// </summary>
    public void GetStartAttackAnimation()
    {
        if(_AttackHitObject != null)
        {
            _AttackHitObject.SetActive(true);
        }
        if (_BoxCollider != null)
        {
            _BoxCollider.enabled = true;
        }
    }

    /// <summary>
    /// 攻撃アニメーションが終了されたとき
    /// </summary>
    public void GetEndAttackAnimation()
    {
        if (_AttackHitObject != null)
        {
            _AttackHitObject.SetActive(false);
        }
        if (_BoxCollider != null)
        {
            _BoxCollider.enabled = false;
        }
        // 攻撃モーションを解除するカウント
        _AttackAnimatonTimeCount = 5.0f;
    }

    /// <summary>
    /// ジャンプ終了のアニメーションを検知する
    /// </summary>
    public void GetJumpEndAnimation()
    {
        _IsJumpEndAnimation = true;
        Debug.Log("ジャンプアニメーション終了");
    }

    /// <summary>
    /// ダメージを相手に与える処理
    /// </summary>
    /// <param name="damageObject"> ダメージが加わる側のオブジェクト </param>
    public void SetCauseDamage(GameObject damageObject)
    {
        // 初期化が終わっているかを調べる
        if(_CharacterStatusManager == null)
        {
            return;
        }
        // ステータスオブジェクトを取得する
        CharacterStatusManager otherCharacterStatusManager = null;
        //　同じオブジェクトにアタッチされているかどうか
        if (damageObject.GetComponent<CharacterStatusManager>() != null)
        {
            otherCharacterStatusManager = damageObject.GetComponent<CharacterStatusManager>();
        }
        if(otherCharacterStatusManager == null)
        {
            // 1階層上のオブジェクトを見る
            // transform.root.gameObject ; ←一番上の親を取得
            // transform.parent.gameObject; ←一つ上の親を取得
            if (damageObject.transform.parent.gameObject.GetComponent<CharacterStatusManager>() != null)
            {
                otherCharacterStatusManager = damageObject.transform.parent.gameObject.GetComponent<CharacterStatusManager>();
            }
        }

        // 処理なし
        if (otherCharacterStatusManager == null)
        {
            return;
        }

        otherCharacterStatusManager.SetDamage(_CharacterStatusManager);
    }

    /// <summary>
    /// 復帰処理
    /// </summary>
    private void SetPlayerReturn()
    {
        // 復活回数をカウントする
        _CharacterStatusManager._RespawningTimesCount++;

        // 復活できない場合
        if (_CharacterStatusManager._RespawningTimesCount >= _CharacterStatusManager._RespawningTimes)
        {
            return;
        }
        
        // アニメーションを再生させる
        _ModelAnimatorController.SetChangeAnimator("WalkAndRunBlendTree");
        // ラグドール化を解除する
        if (_MyCharacterBundledProcessManager != null)
        {
            _MyCharacterBundledProcessManager._RagdollController._IsRagdoll = false;
        }

        // 攻撃の判定を消す
        if (_AttackHitObject != null)
        {
            _AttackHitObject.SetActive(false);
        }
        if (_BoxCollider != null)
        {
            _BoxCollider.enabled = false;
        }

        // 座標を初期位置にする
        // CharacterControllerコンポーネントを一旦無効化する
        _CharacterController.enabled = false;
        _MainObject.transform.position = _InitialPosVector3;
        _MainObject.transform.eulerAngles = _InitialRotVector3;
        // 移動させたのでCharacterControllerコンポーネントを有効化する
        _CharacterController.enabled = true;

        // 体力を回復させる
        _CharacterStatusManager._HP = _CharacterStatusManager._Max_HP;

        // 無敵期間の設定
        _CharacterStatusManager._ReturnInvincibleTimeCount = _CharacterStatusManager._ReturnInvincibleTime;
    }
}
