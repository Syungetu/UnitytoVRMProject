using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ゴール到達時の情報を表示する
/// </summary>
public class GoolInfoPanelController : MonoBehaviour
{

    /// <summary> 残りゴール回数テキスト </summary>
    public TMP_Text _GoolCountTextObject;
    /// <summary> ゴールに接触している時間 </summary>
    public Slider _GoolTimeCountSliderObject;

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {

    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        if(MainSceneManager._Instance == null)
        {
            return;
        }

        // 回数表示
        _GoolCountTextObject.text = "残り" + MainSceneManager.CallGetHitGoolPointClearRemainingCount().ToString() + "回";

        // ゲージの表示
        _GoolTimeCountSliderObject.maxValue = MainSceneManager._Instance._HitGoolPointObjectTime;
        _GoolTimeCountSliderObject.minValue = 0.0f;
        _GoolTimeCountSliderObject.value = MainSceneManager.CallGetHitGoolPointObjectTimeCount();
    }
}
