using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 残り復活回数を表示する
/// </summary>
public class RespawningCountPanelController : MonoBehaviour
{
    /// <summary> プレイヤーのステータスを管理する </summary>
    public CharacterStatusManager _CharacterStatusManager;
    /// <summary>  残り復活回数を表示するテキスト </summary>
    public TMP_Text _RespawningTextObject;

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        if (_RespawningTextObject != null)
        {
            _RespawningTextObject.text = "";
        }
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        if(_CharacterStatusManager == null)
        {
            return;
        }
        if (_RespawningTextObject == null)
        {
            return;
        }
        int count = _CharacterStatusManager._RespawningTimes - _CharacterStatusManager._RespawningTimesCount + 1;
        _RespawningTextObject.text = "残機：" + (_CharacterStatusManager._RespawningTimes - _CharacterStatusManager._RespawningTimesCount).ToString();
    }
}
