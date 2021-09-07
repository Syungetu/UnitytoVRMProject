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
