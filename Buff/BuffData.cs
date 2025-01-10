/*
        버프의 데이터를 직렬화하는 클래스입니다.

        버프 데이터는 Json 을 통해 직렬화됩니다.

        이 스크립트는 전부 제가 작성했습니다.
*/

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
