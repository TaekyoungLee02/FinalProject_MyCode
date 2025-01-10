using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class BuffData : DataTypeBase
{
    /// <summary>
    /// Please Check If the Element Name Equals with Class Name
    /// </summary>
    public enum BUFFSTATUSTYPE
    {
        StatusBuff,
        ElementAtkBuff,
        ElementDefBuff,
        ExtraStatusBuff,
        HPChangeBuff,
        ExampleBuff = 99,
    }

    [JsonIgnore] public BUFFSTATUSTYPE buffStatusType;
    [JsonProperty] private int buffStatusIndex;

    // 버프에 따른 효과 구분용 변수
    public int buffEffectIndex;

    public int buffPower;
    public float buffDuration;
    public float buffIntervalSeconds;
    public bool isEverlasting;

    [JsonConstructor]
    public BuffData(int key, string name, string description, List<int> folderIndex, List<string> filename, int buffPower, float buffDuration, 
                    float buffIntervalSeconds, bool isEverlasting, int buffStatusIndex, int buffEffectIndex)
                    : base(key, name, description, folderIndex, filename)
    {
        this.buffPower = buffPower;
        this.buffDuration = buffDuration;
        this.buffIntervalSeconds = buffIntervalSeconds;
        this.isEverlasting = isEverlasting;
        this.buffStatusIndex = buffStatusIndex;
        buffStatusType = (BUFFSTATUSTYPE)buffStatusIndex;

        this.buffEffectIndex = buffEffectIndex;
    }
}