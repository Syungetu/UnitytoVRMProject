using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーを中心としたカメラの移動管理
/// </summary>
public class MyCameraController : MonoBehaviour
{

    /// <summary> 動かしたいカメラオブジェクト </summary>
    public GameObject _CameraObject;
    /// <summary> 中心になるプレイヤーオブジェクト </summary>
    public GameObject _PlayerObject;
    /// <summary> プレイヤーとカメラの距離 </summary>
    public float _CameraDistance = 3.5f;
    /// <summary> カメラの移動速度 </summary>
    public float _CameraMoveVelocity = 0.2f;
    /// <summary> カメラの回転速度（デグリー：0～360） </summary>
    public float _CameraAngleVelocity = 90.0f;
    /// <summary> カメラの調整高さ </summary>
    public float _CameraAdjustmentHeight = 1.5f;
    /// <summary> カメラが対象に近づくことを許可するかどうか </summary>
    public bool _IsAllowApproach = false;

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
        // カメラの向きをプレイヤー座標にする
        _CameraObject.transform.LookAt(
            _PlayerObject.transform.position + new Vector3(0.0f, _CameraAdjustmentHeight + 0.2f, 0.0f),
            _PlayerObject.transform.up
        );

        // 移動処理
        SetCameraDistance();
        // 回転処理
        SetCametaRotation();
    }

    /// <summary>
    /// カメラとプレイヤーの距離を設定する
    /// </summary>
    private void SetCameraDistance()
    {
        // カメラとプレイヤーの距離を計算する
        float dis = Vector3.Distance(_CameraObject.transform.position, _PlayerObject.transform.position);
        if(_CameraAdjustmentHeight > 0.0f)
        {
            // 高さを設定していた場合、高さは距離に含めないようにする
            dis = Vector3.Distance(
                new Vector3(_CameraObject.transform.position.x, 0.0f, _CameraObject.transform.position.z),
                new Vector3(_PlayerObject.transform.position.x, 0.0f, _PlayerObject.transform.position.z)
            );
        }

        // 指定した距離より近かった場合は移動処理をさせない
        if(_IsAllowApproach == true && _CameraDistance > dis)
        {
            return;
        }

        // プレイヤーとカメラの向きベクトルを取得する
        Vector3 targetNormalized = (_CameraObject.transform.position - _PlayerObject.transform.position).normalized;

        // 指定された距離内の座標を求める
        Vector3 targetPos = _PlayerObject.transform.position + (targetNormalized * _CameraDistance);
        if (_CameraAdjustmentHeight > 0.0f)
        {
            // 高さが指定されていた場合は、高さだけ代入する
            targetPos.y = _PlayerObject.transform.position.y + _CameraAdjustmentHeight;
        }

        // 移動させる
        _CameraObject.transform.position = Vector3.Slerp(
             _CameraObject.transform.position,
             targetPos,
             _CameraMoveVelocity
        );

    }

    /// <summary>
    /// カメラのプレイヤーを中心とした回転を設定する
    /// </summary>
    private void SetCametaRotation()
    {
        // 回転する角度
        float angle = 0.0f;

        // キー操作に合わせて回転角度を変化させる
        if(Input.GetButton("CameraRotation L") == true)
        {
            // フレーム更新似合わせて数値を変える
            angle += _CameraAngleVelocity * Time.deltaTime;
        }
        if (Input.GetButton("CameraRotation R") == true)
        {
            // フレーム更新似合わせて数値を変える
            angle -= _CameraAngleVelocity * Time.deltaTime;
        }

        // 移動させる(円運動)
        _CameraObject.transform.RotateAround(
            _PlayerObject.transform.position,
            _CameraObject.transform.up,
            angle
        );
    }
}
