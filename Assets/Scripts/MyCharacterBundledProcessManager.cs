using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターオブジェクトで使用する処理を纏めたもの
/// </summary>
public class MyCharacterBundledProcessManager : MonoBehaviour
{
    /// <summary> プレイヤーの動きを管理する処理 </summary>
    public MyCharacterController _MyCharacterController;
    /// <summary> 敵の動きを管理する処理 </summary>
    public MyEnemyMoveController _MyEnemyMoveController;
    /// <summary> 敵の攻撃を管理する処理 </summary>
    public MyEnemyAttackController _MyEnemyAttackController;

    /// <summary> キャラクターのステータスを管理する処理 </summary>
    public CharacterStatusManager _CharacterStatusManager;
    /// <summary> ラグドール化を管理する処理 </summary>
    public RagdollController _RagdollController;

    /// <summary> 動けるかどうか </summary>
    private bool _IsMove = true;

    /// <summary>
    /// 移動できるかどうかを返す
    /// </summary>
    /// <returns> false : 移動できない </returns>
    public bool GetIsMove()
    {
        return _IsMove;
    }

    /// <summary>
    /// HPが０で倒されたかどうか
    /// </summary>
    /// <returns> true : 倒された </returns>
    public bool GetIsDown()
    {
        if (_CharacterStatusManager != null && _CharacterStatusManager._HP <= 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        _IsMove = true;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        if (_CharacterStatusManager != null && _CharacterStatusManager._HP <= 0)
        {
            // 体力がなければ動かないようにする
            _IsMove = false;
        }
        if(_MyEnemyAttackController != null && _MyEnemyAttackController.GetIsAttack() == true)
        {
            // 攻撃中は動けないようにする（ゲーム的な設定）
            _IsMove = false;
        }
        if(MainSceneManager.CallIsGameOver == true)
        {
            // ゲームオーバーだったら動かさないようにする
            _IsMove = false;
        }
    }

    /// <summary>
    /// すべての更新処理が終わった後に処理される関数
    /// </summary>
    private void LateUpdate()
    {
        _IsMove = true;
    }
}
