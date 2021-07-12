using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーを中心としたカメラ移動処理
/// </summary>
public class MyCameraController : MonoBehaviour
{
    /// <summary> 動かしたいカメラオブジェクト </summary>
    public GameObject _CameraObject;
    /// <summary> 中心になるプレイヤーオブジェクト </summary>
    public GameObject _PlayerObject;
    /// <summary> プレイヤーとカメラの距離 </summary>
    public float _CameraDistance = 5.0f;
    /// <summary> カメラ移動速度 </summary>
    public float _CameraMoveVelocity = 0.2f;
    /// <summary> カメラ回転速度(デグリー値 0～360) </summary>
    public float _CameraAngleVelocity = 90.0f;
    /// <summary> カメラ高さ調整 </summary>
    public float _CameraAdjustmentHeight = 1.5f;

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
        // カメラの向きをプレイヤーの若干上にする
        _CameraObject.transform.LookAt(
            _PlayerObject.transform.position + new Vector3(0.0f,0.2f, 0.0f), 
            _PlayerObject.transform.up);

        SetCameraDistance();

        SetCameraRotation();

    }

    /// <summary>
    /// カメラとプレイヤーの距離を設定する
    /// </summary>
    private void SetCameraDistance()
    {
        // カメラとプレイヤーの距離を計算
        float dis = Vector3.Distance(_CameraObject.transform.position, _PlayerObject.transform.position);
        if (_CameraAdjustmentHeight > 0.0f)
        {
            // 高さをこちらで設定するので距離には含めないようにする
            dis = Vector3.Distance(
                new Vector3(_CameraObject.transform.position.x, 0.0f, _CameraObject.transform.position.z), 
                new Vector3(_PlayerObject.transform.position.x, 0.0f, _PlayerObject.transform.position.z));
        }

        // 近い時は処理しない
        if (_CameraDistance > dis)
        {
            return;
        }

        // 方角ベクトルにする
        var targetNormalized = (_CameraObject.transform.position - _PlayerObject.transform.position).normalized;

        Vector3 targetPos = _PlayerObject.transform.position + (targetNormalized * _CameraDistance);
        if (_CameraAdjustmentHeight > 0.0f)
        {
            targetPos.y = _PlayerObject.transform.position.y + _CameraAdjustmentHeight;
        }

        _CameraObject.transform.position = Vector3.Slerp(
             _CameraObject.transform.position,
             targetPos,
             _CameraMoveVelocity
        );

    }

    /// <summary>
    /// カメラ回転を設定する
    /// </summary>
    private void SetCameraRotation()
    {
        float angle = 0.0f;

        // キーに合わせて回転角度を変化させる
        if (Input.GetButton("CameraRotation L") == true)
        {
            angle += _CameraAngleVelocity * Time.deltaTime;
        }
        if (Input.GetButton("CameraRotation R") == true)
        {
            angle -= _CameraAngleVelocity * Time.deltaTime;
        }

        _CameraObject.transform.RotateAround(
             _PlayerObject.transform.position,
             _CameraObject.transform.up,
             angle
         );
    }
}
