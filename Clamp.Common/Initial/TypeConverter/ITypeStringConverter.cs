using System;

namespace Clamp.Common.Initial
{
    /// <summary>
    /// 类型转化器
    /// </summary>
    public interface ITypeStringConverter
    {
        string ConvertToString(object value);

       
        object ConvertFromString(string value, Type hint);

        Type ConvertibleType { get; }
    }

    
   
}
