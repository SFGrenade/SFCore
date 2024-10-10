using System;
using UnityEngine;

// Token: 0x02000702 RID: 1794
namespace SFCore.MonoBehaviours;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ArrayForEnumAttribute : PropertyAttribute
{
    // Token: 0x06001F8F RID: 8079 RVA: 0x000BA384 File Offset: 0x000B8784
    public ArrayForEnumAttribute(Type enumType)
    {
        EnumType = enumType;
        if (enumType != null && enumType.IsEnum)
        {
            int num = 0;
            Array values = Enum.GetValues(enumType);
            for (int i = 0; i < values.Length; i++)
            {
                int num2 = (int) values.GetValue(i);
                if (num2 >= 0)
                {
                    if (num < num2)
                    {
                        num = num2 + 1;
                    }
                }
            }
            EnumLength = num;
        }
        else
        {
            EnumLength = 0;
        }
    }

    // Token: 0x170001A7 RID: 423
    // (get) Token: 0x06001F90 RID: 8080 RVA: 0x000BA406 File Offset: 0x000B8806
    public bool IsValid
    {
        get
        {
            return EnumType != null && EnumType.IsEnum && EnumLength > 0;
        }
    }

    // Token: 0x06001F91 RID: 8081 RVA: 0x000BA430 File Offset: 0x000B8830
    public static void EnsureArraySize<T>(ref T[] array, Type enumType)
    {
        int num = Enum.GetNames(enumType).Length;
        if (array != null)
        {
            if (array.Length != num)
            {
                T[] array2 = array;
                array = new T[num];
                for (int i = 0; i < Mathf.Min(num, array2.Length); i++)
                {
                    array[i] = array2[i];
                }
            }
        }
        else
        {
            array = new T[num];
        }
    }

    // Token: 0x04002128 RID: 8488
    public readonly Type EnumType;

    // Token: 0x04002129 RID: 8489
    public readonly int EnumLength;
}