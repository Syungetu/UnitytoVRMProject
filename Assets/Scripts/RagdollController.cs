using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ラグドール処理管理
/// </summary>
public class RagdollController : MonoBehaviour
{

    [Header("オブジェクト設定")]
    /// <summary> メインで使っているRigidbody </summary>
    public Rigidbody _MainRigidbody;
    /// <summary> メインで使っているコライダー </summary>
    public BoxCollider _MainBoxCollider;
    /// <summary> モデルのボーンルートオブジェクト </summary>
    public GameObject _ModelBoonRootObject;
    /// <summary> モデルのAnimator </summary>
    public Animator _ModelAnimator;

    [Header("設定")]
    /// <summary> ラグドールフラグ </summary>
    public bool _IsRagdoll = false;

    /// <summary> ラグドールフラグバッファ </summary>
    private bool _BuffIsRagdoll = true;

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        // 起動時にはラグドール化をキャンセルできるように設定
        _IsRagdoll = false;
        _BuffIsRagdoll = true;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        SetChangeRagdoll();
    }

    /// <summary>
    /// ラグドールにするかどうか
    /// </summary>
    /// <param name="value">合否判定</param>
    private void SetChangeRagdoll()
    {
        if (_BuffIsRagdoll != _IsRagdoll)
        {
            // 前フレームとフラグの値が変わったら実行させる

            if (_IsRagdoll == true)
            {
                StartCoroutine(SetRagdollAsynchronousProcess());
            }
            else
            {
                StartCoroutine(ReleaseRagdollAsynchronousProcess());
            }

            // バッファに反映
            _BuffIsRagdoll = _IsRagdoll;
        }
    }

    /// <summary>
    /// ラグドール化する（徐々に処理を行わないと当たり判定で吹っ飛ぶので非同期処理とする）
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetRagdollAsynchronousProcess()
    {
        if (_MainRigidbody != null)
        {
            // 物理演算をキャンセル
            _MainRigidbody.isKinematic = true;
            // 重力計算をキャンセル
            _MainRigidbody.useGravity = false;

            // 念の為力を０にする
            _MainRigidbody.velocity = Vector3.zero;
            _MainRigidbody.AddForce(0, 0, 0);
        }

        if (_ModelAnimator != null)
        {
            // アニメーションを切らないとビクビクするので切る
            _ModelAnimator.enabled = false;
        }

        if (_MainBoxCollider != null)
        {
            // メインの当たり判定を切っておく（ダメージ用とか）
            _MainBoxCollider.enabled = false;
        }

        // 1フレーム待つ
        yield return null;

        // 各々のラグドール用のコライダーを変化させる
        SetRagdollCollider(_IsRagdoll);

        yield return null;

        // 各々のラグドール用のリジッドボディを有効化する
        SetRagdollRigidbody(_IsRagdoll);

        // リジッドボディのちからを初期化
        SetRagdollRigidbodyForce();
    }

    /// <summary>
    /// ラグドールを解除する（徐々に処理を行わないと当たり判定で吹っ飛ぶので非同期処理とする）
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReleaseRagdollAsynchronousProcess()
    {
        // リジッドボディのちからを初期化
        SetRagdollRigidbodyForce();

        yield return null;

        // 各々のラグドール用のコライダーを変化させる
        SetRagdollCollider(_IsRagdoll);

        yield return null;

        // 各々のラグドール用のリジッドボディを無効化する
        SetRagdollRigidbody(_IsRagdoll);

        if (_MainRigidbody != null)
        {
            // 物理演算をキャンセル
            _MainRigidbody.isKinematic = false;
            // 重力計算をキャンセル
            _MainRigidbody.useGravity = true;

            // 念の為力を０にする
            _MainRigidbody.velocity = Vector3.zero;
            _MainRigidbody.AddForce(0, 0, 0);
        }

        if (_ModelAnimator != null)
        {
            // アニメーションを再生する
            _ModelAnimator.enabled = true;
        }

        if(_MainBoxCollider != null)
        {
            // メインの当たり判定を戻しておく
            _MainBoxCollider.enabled = false;
        }

    }

    /// <summary>
    /// ラグドール用のコライダーを操作する
    /// </summary>
    /// <param name="value"></param>
    private void SetRagdollCollider(bool value)
    {
        Component[] boxColliderComponent = _ModelBoonRootObject.GetComponentsInChildren(typeof(BoxCollider));
        foreach (Component c in boxColliderComponent)
        {
            (c as BoxCollider).enabled = value;
        }

        Component[] sphereColliderComponent = _ModelBoonRootObject.GetComponentsInChildren(typeof(SphereCollider));
        foreach (Component c in sphereColliderComponent)
        {
            (c as SphereCollider).enabled = value;
        }

        Component[] capsuleColliderComponent = _ModelBoonRootObject.GetComponentsInChildren(typeof(CapsuleCollider));
        foreach (Component c in capsuleColliderComponent)
        {
            (c as CapsuleCollider).enabled = value;
        }
    }

    /// <summary>
    /// ラグドール用のリジッドボディを操作する
    /// </summary>
    /// <param name="value"></param>
    private void SetRagdollRigidbody(bool value)
    {
        Component[] rigidbodyComponent = _ModelBoonRootObject.GetComponentsInChildren(typeof(Rigidbody));
        foreach (Component c in rigidbodyComponent)
        {
            (c as Rigidbody).isKinematic = !value;
            (c as Rigidbody).useGravity = value;
        }
    }
    /// <summary>
    /// ラグドール用のリジッドボディのちからを初期化する
    /// </summary>
    /// <param name="value"></param>
    private void SetRagdollRigidbodyForce()
    {
        Component[] rigidbodyComponent = _ModelBoonRootObject.GetComponentsInChildren(typeof(Rigidbody));
        foreach (Component c in rigidbodyComponent)
        {
            (c as Rigidbody).velocity = Vector3.zero;
            (c as Rigidbody).AddForce(0,0,0);
        }
    }
}
