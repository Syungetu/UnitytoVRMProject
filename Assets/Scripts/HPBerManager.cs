using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// HPバー処理
/// </summary>
public class HPBerManager : MonoBehaviour
{
    /// <summary> HPバー </summary>
    public Slider _HPBarSlider;
    /// <summary> HPテキスト </summary>
    public TMP_Text _HPValueText;
    /// <summary> ステータス </summary>
    public CharacterStatusManager _CharacterStatusManager;


    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        if (_CharacterStatusManager != null)
        {
            _HPBarSlider.value = _CharacterStatusManager._HP;
            _HPBarSlider.minValue = 0.0f;
            _HPBarSlider.maxValue = _CharacterStatusManager._Max_HP;
            _HPValueText.text = "HP : " + _CharacterStatusManager._HP.ToString();
        }
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        if (_CharacterStatusManager != null)
        {
            _HPBarSlider.value = _CharacterStatusManager._HP;
            _HPValueText.text = "HP : " + _CharacterStatusManager._HP.ToString();
        }
    }
}
