using System;
using System.Collections.Generic;
using UnityEngine;

public static class BuffExtension
{
    private static Dictionary<BuffData.BUFFSTATUSTYPE, Type> _buffTypeDictionary = new();

    /// <summary>
    /// Gets Buff Class From Buff Data. Buff Type is Defined from Buff Status Type.
    /// </summary>
    /// <param name="data">Casting Data</param>
    /// <returns></returns>
    /// <exception cref="Exception">Buff Class is Not Valid or Assembly is Not Valid</exception>
    public static IBuffApplier GetBuff(this BuffData data)
    {
        // _buffTypeDictionary 에 buffStatusType 이 없을 경우 넣어줌
        if (!_buffTypeDictionary.ContainsKey(data.buffStatusType))
        {
            // 클래스의 타입을 받아옴
            var enumClass = Type.GetType(data.buffStatusType.ToString());

            // 없다면 예외 처리
            if (enumClass == null)
            {
                throw new Exception("enumClass is null : Check If Buff Class of BuffStatusType Valid or Assembly is Valid");
            }
            // 타입을 딕셔너리에 넣음
            _buffTypeDictionary.Add(data.buffStatusType, enumClass);
        }
        // 버프타입을 딕셔너리에서 꺼내와 인스턴스를 생성
        var buff = (IDataInitializer<BuffData>)Activator.CreateInstance(_buffTypeDictionary[data.buffStatusType]);
        // 버프를 초기화
        buff.Initialize(data);
        // 인터페이스로 캐스팅해서 넘겨줌
        return (IBuffApplier)buff;
    }
}