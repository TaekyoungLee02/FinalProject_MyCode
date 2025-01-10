/*
        BuffData 클래스의 확장함수입니다.

        BuffData 를 바로 IBuffApplier 인스턴스로 교환해 주는 함수입니다.

        자세한 내용은 주석으로 적어 두었습니다.

        다만 이 부분은 지금 와서는 틀린 판단이었다는 생각이 듭니다.

        버프는 이렇게 상속 구조로 짠다면 너무 많은 하위 클래스들이 만들어진다는 점을 간과했습니다.

        사실 처음 작성할 때 이 부분을 생각하지 않은 건 아니지만, (변명..)

        버프의 가짓수가 그렇게 많지 않을 것이라 생각해 진행했던 것이 패착이었던 것 같습니다.

        다음부터는 상속 구조보다는 버프의 종류들을 정규화해서 Enum으로 작성하는 게 나을 것 같습니다.

        switch 문의 노가다(?) 는 필요하겠지만, 그 부분이 오히려 상속 구조가 깔끔하게 나오고 확장성도 늘어날 것 같습니다.

        이 부분은 전부 제가 작성했습니다.
*/

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
