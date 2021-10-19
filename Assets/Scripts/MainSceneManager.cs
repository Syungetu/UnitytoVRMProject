using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シーン全体を管理する
/// </summary>
public class MainSceneManager : MonoBehaviour
{
    /// <summary> インスタンス変数 </summary>
    public static MainSceneManager _Instance = null;

    /// <summary> プレイヤーのステータスを管理する </summary>
    public CharacterStatusManager _PlayerCharacterStatusManager;
    /// <summary> 目的地を示すオブジェクトリスト </summary>
    public List<GameObject> _GoolPointObjectList = null;
    /// <summary> ゲームクリアを表示するパネル </summary>
    public GameObject _GameClearPanel;
    /// <summary> ゲームオーバーを表示するパネル </summary>
    public GameObject _GameOverPanel;

    /// <summary> 目的地オブジェクトに触れ続けてクリアになる時間（秒） </summary>
    public float _HitGoolPointObjectTime = 10.0f;
    /// <summary> ゲームクリアになる目的地オブジェクトに触れる数 </summary>
    public int _HitGoolPointClearMaxCount = 3;

    /// <summary> 有効な目的地インデックス </summary>
    private int _TargetIndex = 0; 
    /// <summary> 目的地オブジェクトに触れている時間 </summary>
    private float _HitGoolPointObjectTimeCount = 0.0f;
    /// <summary> ゲームクリアになる目的地オブジェクトに触れた数 </summary>
    private int _HitGoolPointClearNowCount = 0;
    /// <summary> 目的地オブジェクトに触れているかどうか </summary>
    private bool _IsHitGoolObject = false;
    /// <summary> 接触中の目的地オブジェクト </summary>
    private GameObject _NowHitGoolObject = null;
    /// <summary> ゲームクリアフラグ </summary>
    private bool _IsGameClear = false;
    /// <summary> ゲームオーバーフラグ </summary>
    private bool _IsGameOver = false;
    /// <summary> ゲームオーバーフラグ </summary>
    public static bool CallIsGameOver
    {
        set
        {
            if(_Instance == null)
            {
                return;
            }
            _Instance._IsGameOver = value;
        }
        get
        {
            if (_Instance == null)
            {
                return false;
            }
            return _Instance._IsGameOver;
        }
    }

    /// <summary>
    /// 目的地の残り回数
    /// </summary>
    /// <returns> 残り回数 </returns>
    public static int CallGetHitGoolPointClearRemainingCount()
    {
        if (_Instance == null)
        {
            return -1;
        }
        return _Instance._HitGoolPointClearMaxCount - _Instance._HitGoolPointClearNowCount;
    }

    /// <summary>
    /// 目的地オブジェクトに触れている時間を返す
    /// </summary>
    /// <returns> 触れている時間 </returns>
    public static float CallGetHitGoolPointObjectTimeCount()
    {
        if (_Instance == null)
        {
            return 0.0f;
        }
        return _Instance._HitGoolPointObjectTimeCount;
    }

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        _Instance = this;

        _HitGoolPointObjectTimeCount = 0.0f;
        _HitGoolPointClearNowCount = 0;
        _IsHitGoolObject = false;
        _NowHitGoolObject = null;

        _IsGameClear = false;
        _IsGameOver = false;

        // ゲームクリア、オーバーのパネルは初期化時に非表示にする
        if (_GameOverPanel != null)
        {
            _GameOverPanel.SetActive(false);
        }

        if(_GameClearPanel != null)
        {
            _GameClearPanel.SetActive(false);
        }

        // 目標となる目的地は１つだけ有効化する
        // 第一引数は最小値
        // 第二引数は最大値の-1
        _TargetIndex = UnityEngine.Random.Range(0, _GoolPointObjectList.Count);
        int index = 0;
        foreach(GameObject targetObj in _GoolPointObjectList)
        {
            if(index == _TargetIndex)
            {
                // 該当する目標だけ有効
                targetObj.SetActive(true);
            }
            else
            {
                targetObj.SetActive(false);
            }
            index++;
        }
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        // 生成されていなかったら処理しない
        if(_PlayerCharacterStatusManager == null)
        {
            return;
        }

        // ゲームーバーになっている
        if(_IsGameOver == true)
        {
            return;
        }

        // ゲームクリアになっている
        if(_IsGameClear == true)
        {
            return;
        }

        if (_PlayerCharacterStatusManager._HP <= 0)
        {
            // 倒されたときのみ判定する
            if(_PlayerCharacterStatusManager._RespawningTimesCount >= _PlayerCharacterStatusManager._RespawningTimes)
            {
                // 指定された復活回数を超えたらゲームオーバー
                _IsGameOver = true;
                if (_GameOverPanel != null)
                {
                    _GameOverPanel.SetActive(true);
                }
            }
        }

        // ゴールに触れている時のカウント
        if (_IsHitGoolObject == true)
        {
            // カウントする
            _HitGoolPointObjectTimeCount += Time.deltaTime;
            if(_HitGoolPointObjectTimeCount >= _HitGoolPointObjectTime)
            {
                // クリア
                _HitGoolPointClearNowCount++;
                _HitGoolPointObjectTimeCount = 0.0f;

                // 目標となる目的地は１つだけ有効化する
                int targetIndex = UnityEngine.Random.Range(0, _GoolPointObjectList.Count);
                while(_TargetIndex == targetIndex)
                {
                    // 同じ場所を目的地にしないために再抽選をする
                    targetIndex = UnityEngine.Random.Range(0, _GoolPointObjectList.Count);
                }
                _TargetIndex = targetIndex;
                int index = 0;
                foreach (GameObject targetObj in _GoolPointObjectList)
                {
                    if (index == _TargetIndex)
                    {
                        // 該当する目標だけ有効
                        targetObj.SetActive(true);
                    }
                    else
                    {
                        targetObj.SetActive(false);
                    }
                    index++;
                }

                if (_HitGoolPointClearNowCount >= _HitGoolPointClearMaxCount)
                {
                    // 指定回数達したのでゲームクリア
                    _IsGameClear = true;

                    if (_GameClearPanel != null)
                    {
                        _GameClearPanel.SetActive(true);
                    }
                }
            }
        }
        else
        {
            //触れていなければカウントを下げる
            if (_HitGoolPointObjectTimeCount > 0.0f)
            {
                _HitGoolPointObjectTimeCount -= Time.deltaTime;
                if (_HitGoolPointObjectTimeCount < 0.0f)
                {
                    // マイナスにならないようにする
                    _HitGoolPointObjectTimeCount = 0.0f;
                }
            }
        }

        // 目的地が切り替わった時に、接触判定が残り続けてしまうので調整
        if (_NowHitGoolObject != null)
        {
            if (_NowHitGoolObject.activeSelf == false)
            {
                // 接触判定オブジェクトがあり、なおかつ非表示状態の時
                if (_IsHitGoolObject == true)
                {
                    // 目的地の接触判定を消す
                    _IsHitGoolObject = false;
                    _NowHitGoolObject = null;
                }
            }
        }

    }

    /// <summary>
    /// Update処理が終わった後に呼ばれる関数
    /// </summary>
    private void LateUpdate()
    {
    }

    /// <summary>
    /// ゴールオブジェクトに接触した瞬間に動く関数
    /// </summary>
    /// <param name="targetObj"></param>
    public void GetGoolPointObjectEnterEvent(GameObject targetObj)
    {
        _IsHitGoolObject = true;
        _NowHitGoolObject = targetObj;
    }

    /// <summary>
    /// ゴールオブジェクトに離れた瞬間に動く関数
    /// </summary>
    /// <param name="targetObj"></param>
    public void GetGoolPointObjectExitEvent(GameObject targetObj)
    {
        _IsHitGoolObject = false;
        _NowHitGoolObject = null;
    }
}
