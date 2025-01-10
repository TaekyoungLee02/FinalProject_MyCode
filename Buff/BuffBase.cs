using System;
using System.Collections;
using UnityEngine;

public abstract class BuffBase : IBuffApplier, IDataInitializer<BuffData>
{
    protected int buffKey;

    protected int buffEffectIndex;

    protected int buffPower;
    private float curDuration;
    private float buffDuration;
    private float buffIntervalSeconds;
    private bool isEverlasting;

    private WaitForSeconds buffInterval;
    private Coroutine buffTimer;

    public event Action<CharacterStatus, IBuffApplier> BuffStartEvent;
    public event Action<CharacterStatus, IBuffApplier> BuffUpdateEvent;
    public event Action<CharacterStatus, IBuffApplier> BuffEndEvent;

    public int BuffKey { get { return buffKey; } }

    /// <summary>
    /// Buff Increses and Decreses
    /// </summary>
    public int BuffPower { get { return buffPower; } }
    /// <summary>
    /// Curent Buff Duration
    /// </summary>
    public float CurDuration { get { return curDuration; } set { curDuration = value; } }
    /// <summary>
    /// Buff Lasts During curDuration is less than BuffDuration
    /// </summary>
    public float BuffDuration { get { return buffDuration; } set { buffDuration = value; } }
    /// <summary>
    /// Invokes Buff Update Every BuffIntervalSeconds
    /// </summary>
    public float BuffIntervalSeconds { get { return buffIntervalSeconds; } set { buffIntervalSeconds = value; buffInterval = new WaitForSeconds(buffIntervalSeconds); } }
    /// <summary>
    /// Doesn't Check If the BuffDuration OutDated When IsEverlasting is Ture
    /// </summary>
    public bool IsEverlasting { get { return isEverlasting; } }


    public void Initialize(BuffData data)
    {
        buffKey = data.key;

        buffEffectIndex = data.buffEffectIndex;

        buffPower = data.buffPower;
        buffDuration = data.buffDuration;
        buffIntervalSeconds = data.buffIntervalSeconds;
        isEverlasting = data.isEverlasting;
        curDuration = 0;
        buffInterval = new WaitForSeconds(buffIntervalSeconds);
    }

    public void OnBuffStart(CharacterStatus status)
    {
        if (status == null) throw new ArgumentNullException("CharacterStatus is Null");

        BuffStarter(status);
        BuffStartEvent?.Invoke(status, this);
        buffTimer = status.StartCoroutine(BuffTimer());
    }

    public void OnBuffEnd(CharacterStatus status)
    {
        if (status == null) throw new ArgumentNullException("CharacterStatus is Null");

        BuffEnder(status);
        BuffEndEvent?.Invoke(status, this);
        status.StopCoroutine(buffTimer);
        buffTimer = null;
    }

    public IEnumerator OnBuffUpdate(CharacterStatus status)
    {
        if (status == null) throw new ArgumentNullException("CharacterStatus is Null");

        while (curDuration < buffDuration || isEverlasting)
        {
            BuffUpdater(status);
            BuffUpdateEvent?.Invoke(status, this);
            yield return buffInterval;
        }
    }

    /// <summary>
    /// Function Called When Buff Starts
    /// </summary>
    /// <param name="status"></param>
    protected abstract void BuffStarter(CharacterStatus status);

    /// <summary>
    /// Function Called Every BuffIntervalSeconds
    /// </summary>
    /// <param name="status"></param>
    protected abstract void BuffUpdater(CharacterStatus status);

    /// <summary>
    /// Function Called When Buff Ends
    /// </summary>
    /// <param name="status"></param>
    protected abstract void BuffEnder(CharacterStatus status);

    private IEnumerator BuffTimer()
    {
        while (true)
        {
            curDuration += Time.deltaTime;
            yield return null;
        }
    }
}