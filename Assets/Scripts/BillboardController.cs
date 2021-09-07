using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 常にカメラを向くビルボードオブジェクト管理
/// </summary>
public class BillboardController : MonoBehaviour
{
    /// <summary> カメラオブジェクト </summary>
    public Camera _MainCameraObject;
    /// <summary> カメラに向けるオブジェクト </summary>
    public GameObject _MainObject;

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
        // 指定された場所に向くようにする(反転しているので180度回転)
        Quaternion rot = Quaternion.LookRotation(_MainCameraObject.transform.position);
        _MainObject.transform.eulerAngles = rot.eulerAngles + new Vector3(0.0f, 180.0f, 0.0f);
    }
}
