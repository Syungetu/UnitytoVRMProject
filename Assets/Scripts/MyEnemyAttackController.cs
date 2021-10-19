using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の攻撃処理を管理する
/// </summary>
public class MyEnemyAttackController : MonoBehaviour
{
    /// <summary> キャラクターオブジェクトで使用する処理を纏めたもの </summary>
    public MyCharacterBundledProcessManager _MyCharacterBundledProcessManager;
    /// <summary> 攻撃用の当たり判定オブジェクト </summary>
    public GameObject _AttackHitObject;
    /// <summary> 攻撃用の当たり判定 </summary>
    public BoxCollider _BoxCollider;
    /// <summary> アニメーションを管理する </summary>
    public ModelAnimatorController _ModelAnimatorController;

    /// <summary> 攻撃対象オブジェクト </summary>
    public GameObject _AttackTargetObject;
    /// <summary> 攻撃を行う距離 </summary>
    public float _AttackRange = 1.5f;
    /// <summary> 攻撃と攻撃の間隔（秒） </summary>
    public float _AttackIntervalTime = 1.0f;

    /// <summary> 攻撃フラグ </summary>
    private bool _IsAttackStart = false;
    /// <summary> 攻撃と攻撃の間隔（秒） </summary>
    private float _AttackIntervalTimeCount = 0.0f;
    /// <summary> 攻撃モーション解除のカウントをする </summary>
    private float _AttackAnimatonTimeCount = 0.0f;

    /// <summary>
    /// 攻撃しているかどうかを送る
    /// </summary>
    /// <returns></returns>
    public bool GetIsAttack()
    {
        return _IsAttackStart;
    }

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        _IsAttackStart = false;
        _AttackIntervalTimeCount = 0.0f;
        _AttackAnimatonTimeCount = 0.0f;

        _AttackHitObject.SetActive(false);
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        // 攻撃を連続で行わせないようにする
        if(_AttackIntervalTimeCount > 0.0f)
        {
            _AttackIntervalTimeCount -= Time.deltaTime;

            if(_AttackIntervalTimeCount <= 0.0f)
            {
                // 待ち時間終了
                _IsAttackStart = false;
            }

            return;
        }

        // すでに攻撃中は処理しない
        if (_IsAttackStart == true)
        {
            return;
        }

        bool isAttack = false;

        // 攻撃対象との距離を測る
        Vector3 myPosition = _MyCharacterBundledProcessManager._MyEnemyMoveController._ModelObject.transform.position;
        Vector3 targetPosition = _AttackTargetObject.transform.position;

        float dis = Vector3.Distance(targetPosition, myPosition);
        if(dis <= _AttackRange)
        {
            // 攻撃範囲に居る
            isAttack = true;
        }

        // 攻撃しない場合は以下の処理は行わないようにする
        if(isAttack == false)
        {
            // 攻撃モーションを解除する
            if (_AttackAnimatonTimeCount > 0.0f)
            {
                _AttackAnimatonTimeCount -= Time.deltaTime;
                if (_AttackAnimatonTimeCount <= 0.0f)
                {
                    // 攻撃モーションを解除する
                    _ModelAnimatorController.SetByLayerChangeAnimator(3, 0.0f);
                }
            }
            return;
        }

        // 攻撃フラをを建てる
        _IsAttackStart = true;

        // 攻撃のモーションを再生する
        _ModelAnimatorController.SetByLayerChangeAnimator(3, 1.0f);
        _ModelAnimatorController.SetChangeStartAnimator("Attack", 3);

        Debug.Log("敵の攻撃を開始");
    }

    /// <summary>
    /// 攻撃アニメーションが開始されたとき
    /// </summary>
    public void GetStartAttackAnimation()
    {
        if (_AttackHitObject != null)
        {
            _AttackHitObject.SetActive(true);
        }
        if(_BoxCollider != null)
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
        // 次の攻撃までのカウント
        _AttackIntervalTimeCount = _AttackIntervalTime;
        // 攻撃モーションを解除するカウント
        _AttackAnimatonTimeCount = 5.0f;
    }

    /// <summary>
    /// ダメージを相手に与える処理
    /// </summary>
    /// <param name="damageObject"> ダメージが加わる側のオブジェクト </param>
    public void SetCauseDamage(GameObject damageObject)
    {
        // 初期化が終わっているかを調べる
        if (_MyCharacterBundledProcessManager._CharacterStatusManager == null)
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
        if (otherCharacterStatusManager == null)
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

        otherCharacterStatusManager.SetDamage(_MyCharacterBundledProcessManager._CharacterStatusManager);
    }
}
