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
        // _buffTypeDictionary �� buffStatusType �� ���� ��� �־���
        if (!_buffTypeDictionary.ContainsKey(data.buffStatusType))
        {
            // Ŭ������ Ÿ���� �޾ƿ�
            var enumClass = Type.GetType(data.buffStatusType.ToString());

            // ���ٸ� ���� ó��
            if (enumClass == null)
            {
                throw new Exception("enumClass is null : Check If Buff Class of BuffStatusType Valid or Assembly is Valid");
            }
            // Ÿ���� ��ųʸ��� ����
            _buffTypeDictionary.Add(data.buffStatusType, enumClass);
        }
        // ����Ÿ���� ��ųʸ����� ������ �ν��Ͻ��� ����
        var buff = (IDataInitializer<BuffData>)Activator.CreateInstance(_buffTypeDictionary[data.buffStatusType]);
        // ������ �ʱ�ȭ
        buff.Initialize(data);
        // �������̽��� ĳ�����ؼ� �Ѱ���
        return (IBuffApplier)buff;
    }
}