using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// デバッグ表示を切り替えるボタン処理
/// </summary>
public class DebugModeButtonController : MonoBehaviour
{

    /// <summary> デバッグメニューパネル </summary>
    public GameObject _DebugMenuPanel;

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        _DebugMenuPanel.SetActive(false);
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        
    }

    /// <summary>
    /// デバッグ表示を切り替えるボタンを押したとき
    /// </summary>
    public void OnClickDebugModeButton()
    {
        _DebugMenuPanel.SetActive(!_DebugMenuPanel.activeSelf);
    }
}
