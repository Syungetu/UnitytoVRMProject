using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターのステータス設定
/// </summary>
public class CharacterStatusManager : MonoBehaviour
{
    /// <summary> 体力 </summary>
    public int _HP = 100;
    /// <summary> 最大体力 </summary>
    public int _Max_HP = 100;
    /// <summary> 攻撃力 </summary>
    public int _ATK = 20;
    /// <summary> 防御力 </summary>
    public int _DEF = 10;

    /// <summary> やられた時の復活までの時間（秒） </summary>
    public float _ReturnTime = 20.0f;
    /// <summary> 復活カウント </summary>
    public float _ReturnTimeCount = 0.0f;
    /// <summary> 復活の無敵時間（秒） </summary>
    public float _ReturnInvincibleTime = 5.0f;
    /// <summary> 復活の無敵カウント </summary>
    public float _ReturnInvincibleTimeCount = 0.0f;

    /// <summary> フレイヤーの復活制限回数（コンテニュー数） </summary>
    public int _RespawningTimes = 3;
    /// <summary> フレイヤーの復活回数 </summary>
    public int _RespawningTimesCount = 0;

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        _HP = _Max_HP;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="attackCharacterStatusManager"> 攻撃側のステータス </param>
    public void SetDamage(CharacterStatusManager attackCharacterStatusManager)
    {
        // 無敵期間
        if(_ReturnInvincibleTimeCount > 0.0f)
        {
            Debug.Log("ダメージ：無敵期間 " + _ReturnInvincibleTimeCount.ToString("F2") + "秒 " + this.gameObject.name);
            return;
        }

        // とりあえず簡単にダメージ計算する
        int damage = attackCharacterStatusManager._ATK - _DEF;
        if(damage <= 0)
        {
            damage = 1;
        }
        _HP -= damage;

        Debug.Log("ダメージ：" + damage.ToString() + " " + this.gameObject.name);
    }

}
