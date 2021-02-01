using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

/// <summary>
/// 顔の表情管理
/// </summary>
public class FaceController : MonoBehaviour
{
    /// <summary> 表情を変える時に使う処理 </summary>
    public VRMBlendShapeProxy _VRMBlendShapeProxy;
    [Header("振り向き処理（Animatorがアタッチされているオブジェクトにしか使えません）")]
    /// <summary> モデルにアタッチされているAnimator </summary>
    public Animator _ModelAnimator;

    [Header("目パチ処理")]
    /// <summary> 目パチにかかる時間（秒） </summary>
    public float _BlinkTime = 0.1f;
    /// <summary> 目パチするかどうか </summary>
    public bool _IsBlink = true;

    /// <summary> 目パチしているかどうか </summary>
    private bool _IsPlayBilnk = false;
    /// <summary> 目パチ進行カウント </summary>
    private float _BilnkPlayCount = 0.0f;
    /// <summary> 目パチタイミングカウント </summary>
    private float _BilnkTimeingCount = 0.0f;

    [Header("口パク処理")]
    /// <summary> 口パクさせるかどうか </summary>
    public bool _IsTalk = false;
    /// <summary> 口パク速度 </summary>
    public float _TalkSpeedTime = 0.3f;

    /// <summary> 口パクの初期化フラグ </summary>
    private bool _IsInitializationTalk = false;
    /// <summary> 口パクの再生フラグ </summary>
    private bool _IsPlayTalk = false;
    /// <summary> 口パクの口の形種類 </summary>
    private BlendShapePreset _TalkPreset = BlendShapePreset.Neutral;
    /// <summary> 口パクの進行カウント </summary>
    private float _TalkPlayCount = 0.0f;

    [Header("振り向きフラグ（AnimatorのIK Passをオンにしておく）")]
    /// <summary> 振り向きをさせるかどうか </summary>
    public bool _IsLookAt = false;
    /// <summary> 振り向くオブジェクト（ターゲット） </summary>
    public GameObject _LookAtObj = null;
    /// <summary> 振り向く座標（ターゲット） </summary>
    public Vector3? _LookAtPos = null;
    /// <summary> 振り向くときの強さ（Animationの強さとの兼ね合い） </summary>
    public float _LookAtWeight = 0.5f;


    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        // 目パチ
        SetIniitializationBlinkProcess();

        // 口パク
        SetIniitializationTalkProcess();

        // 振り向き
        SetIniitializationLookAtProcess();
    }

    /// <summary>
    /// 目パチ周りの初期化処理
    /// </summary>
    private void SetIniitializationBlinkProcess()
    {
        _IsBlink = true;
        _IsPlayBilnk = false;
        _BilnkPlayCount = 0.0f;
        _BilnkTimeingCount = 0.0f;
    }

    /// <summary>
    /// 口パク周りの初期化処理
    /// </summary>
    private void SetIniitializationTalkProcess()
    {
        _IsTalk = false;
        _IsInitializationTalk = false;
        _IsPlayTalk = false;
        _TalkPreset = BlendShapePreset.Neutral;
        _TalkPlayCount = 0.0f;
    }

    /// <summary>
    /// 振り向き周り処理帰化処理
    /// </summary>
    private void SetIniitializationLookAtProcess()
    {
        _IsLookAt = false;
        _LookAtObj = null;
        _LookAtPos = null;
        _LookAtWeight = 0.5f;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        // 目パチ
        SetPlayBilk();

        // 口パク
        SetPlayTalk();
    }

    /// <summary>
    /// 目パチ処理
    /// </summary>
    private void SetPlayBilk()
    {
        if (_IsBlink == false)
        {
            // 目パチさせない場合は処理を抜けるようにする
            return;
        }

        if (_BilnkTimeingCount <= 0.0f)
        {
            // 目パチを行うタイミングになった
            _IsPlayBilnk = true;
            _BilnkPlayCount = 0.0f;

            // 次の目パチまでの時間
            _BilnkTimeingCount = UnityEngine.Random.Range(1.0f, 5.0f);
        }

        if (_IsPlayBilnk == true && _BilnkTimeingCount > 0.0f)
        {
            // 目パチ実行中
            _BilnkPlayCount += Time.deltaTime;

            // モーフィングの強さ
            float buffValue = 0.0f;
            if (_BilnkPlayCount <= (_BlinkTime / 2.0f))
            {
                // 目を閉じる
                buffValue = _BilnkPlayCount / (_BlinkTime / 2.0f);
            }
            else if (_BilnkPlayCount <= _BlinkTime)
            {
                // 目を開く
                buffValue = 1.0f - (_BilnkPlayCount - (_BlinkTime / 2.0f)) / (_BlinkTime / 2.0f);
            }

            // 目パチ終了
            if (_BilnkPlayCount > _BlinkTime)
            {
                _IsPlayBilnk = false;
                buffValue = 0.0f;
            }

            if (_VRMBlendShapeProxy != null)
            {
                // モーフィングさせる処理
                _VRMBlendShapeProxy.ImmediatelySetValue(
                    BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink),
                    buffValue
                );
            }
        }

        if (_IsPlayBilnk == false && _BilnkTimeingCount > 0.0f)
        {
            // 次の目パチまでのカウント
            _BilnkTimeingCount -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 口パク処理
    /// </summary>
    private void SetPlayTalk()
    {
        if (_IsTalk == false)
        {
            // 口パクさせない場合は処理を抜けるようにする

            // 口の形を戻しておく
            if(_IsInitializationTalk == false)
            {
                if (_VRMBlendShapeProxy != null)
                {
                    for (BlendShapePreset n = BlendShapePreset.A; n <= BlendShapePreset.O; n++)
                    {
                        // モーフィングさせる処理
                        _VRMBlendShapeProxy.ImmediatelySetValue(
                            BlendShapeKey.CreateFromPreset(n),
                            0.0f
                        );
                    }
                }
                _IsInitializationTalk = true;
            }
            return;
        }

        if (_IsPlayTalk == false)
        {
            // 口パクを行うタイミングになった
            _IsPlayTalk = true;
            _TalkPlayCount = 0.0f;

            // 口の形をランダムで変更させる(あ～おまでの口の形からランダムで取得）
            _TalkPreset = (BlendShapePreset)UnityEngine.Random.Range((int)BlendShapePreset.A, (int)BlendShapePreset.O);
        }

        if (_IsPlayTalk == true)
        {
            _IsInitializationTalk = false;

            // 口パク実行中
            _TalkPlayCount += Time.deltaTime;

            // モーフィングの強さ
            float buffValue = 0.0f;
            if (_TalkPlayCount <= (_TalkSpeedTime / 2.0f))
            {
                // 口を開く
                buffValue = _TalkPlayCount / (_TalkSpeedTime / 2.0f);
            }
            else if (_TalkPlayCount <= _TalkSpeedTime)
            {
                // 口を閉じる
                buffValue = 1.0f - (_TalkPlayCount - (_TalkSpeedTime / 2.0f)) / (_TalkSpeedTime / 2.0f);
            }

            // 口パク終了
            if (_TalkPlayCount > _TalkSpeedTime)
            {
                _IsPlayTalk = false;
                buffValue = 0.0f;
            }

            if (_VRMBlendShapeProxy != null)
            {
                // モーフィングさせる処理
                _VRMBlendShapeProxy.ImmediatelySetValue(
                    BlendShapeKey.CreateFromPreset(_TalkPreset),
                    buffValue
                );
            }
        }
    }

    /// <summary>
    /// AnimatorIKを使ってボーンを操作する処理
    /// </summary>
    /// <param name="layerIndex">レイヤー番号</param>
    private void OnAnimatorIK(int layerIndex)
    {
        SetLookAt();
    }

    /// <summary>
    /// 振り向き処理
    /// </summary>
    private void SetLookAt()
    {
        if (_IsLookAt == false)
        {
            // 振り向き処理をしないときは処理を抜ける

            // 各ボーンに対しての動く重みを初期化する
            _ModelAnimator.SetLookAtWeight(
                0.0f,   // 全体の重み
                0.0f,   // 上半身を動かす重み
                0.0f,   // 頭を動かす重み
                0.0f,   // 目を動かす重み
                0.0f    // モーションの制限量（後で数値を入れる）
            );

            return;
        }

        if(_LookAtObj != null)
        {
            // 指定されたオブジェクトが入っていたらそこから座標を取得する
            _LookAtPos = (Vector3)_LookAtObj.transform.position;
        }

        if(_LookAtPos == null)
        {
            // 座標が入力されていないので、目標地点がないので処理しない

            // 各ボーンに対しての動く重みを初期化する
            _ModelAnimator.SetLookAtWeight(
                0.0f,   // 全体の重み
                0.0f,   // 上半身を動かす重み
                0.0f,   // 頭を動かす重み
                0.0f,   // 目を動かす重み
                0.0f    // モーションの制限量（後で数値を入れる）
            );

            return;
        }

        // 各ボーンに対しての動く重みを設定する
        _ModelAnimator.SetLookAtWeight(
            1.0f * _LookAtWeight,   // 全体の重み
            0.5f * _LookAtWeight,   // 上半身を動かす重み
            0.8f * _LookAtWeight,   // 頭を動かす重み
            1.0f * _LookAtWeight,   // 目を動かす重み
            0.0f                    // モーションの制限量（後で数値を入れる）
        );
        // 振り向く位置を設定する
        _ModelAnimator.SetLookAtPosition((Vector3)_LookAtPos);
    }
}
