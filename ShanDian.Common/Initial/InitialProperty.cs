using System;

namespace ShanDian.Common.Initial
{
    /// <summary>
    /// 节项类
    /// </summary>
    public sealed class InitialProperty : InitialElement
    {
        private string _rawValue = string.Empty;
        private int _cachedArraySize = 0;
        private bool _shouldCalculateArraySize = false;
        private char _cachedArrayElementSeparator;

        public InitialProperty(string name) : this(name, string.Empty)
        { }


        public InitialProperty(string name, object value) : base(name)
        {
            this.SetValue(value);
            this._cachedArrayElementSeparator = InitialFile.ArrayElementSeparator;
        }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(_rawValue); }
        }

        public string StringValueTrimmed
        {
            get
            {
                string value = StringValue;

                if (!string.IsNullOrWhiteSpace(value) && value[0] == '\"')
                    value = value.Trim('\"');

                return value;
            }
        }


        public string StringValue
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }


        public string[] StringValueArray
        {
            get { return GetValueArray<string>(); }
            set { SetValue(value); }
        }


        public int IntValue
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }


        public int[] IntValueArray
        {
            get { return GetValueArray<int>(); }
            set { SetValue(value); }
        }


        public float FloatValue
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }

        public float[] FloatValueArray
        {
            get { return GetValueArray<float>(); }
            set { SetValue(value); }
        }


        public double DoubleValue
        {
            get { return GetValue<double>(); }
            set { SetValue(value); }
        }


        public double[] DoubleValueArray
        {
            get { return GetValueArray<double>(); }
            set { SetValue(value); }
        }


        public bool BoolValue
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }


        public bool[] BoolValueArray
        {
            get { return GetValueArray<bool>(); }
            set { SetValue(value); }
        }


        public DateTime DateTimeValue
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }


        public DateTime[] DateTimeValueArray
        {
            get { return GetValueArray<DateTime>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 是否为数组
        /// </summary>
        public bool IsArray
        {
            get { return (ArraySize >= 0); }
        }


        public int ArraySize
        {
            get
            {
                if (this._cachedArrayElementSeparator != InitialFile.ArrayElementSeparator)
                {
                    this._cachedArrayElementSeparator = InitialFile.ArrayElementSeparator;
                    this._shouldCalculateArraySize = true;
                }

                if (this._shouldCalculateArraySize)
                {
                    this._cachedArraySize = CalculateArraySize();
                    this._shouldCalculateArraySize = false;
                }

                return this._cachedArraySize;
            }
        }
        /// <summary>
        /// 计算数组长度
        /// </summary>
        /// <returns></returns>
        private int CalculateArraySize()
        {
            int size = 0;
            var enumerator = new InitialPropertyEnumerator(_rawValue, false);

            while (enumerator.Next())
                ++size;

            return (enumerator.IsValid ? size : -1);
        }


        /// <summary>
        /// 根据类型返回对应的实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object GetValue(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type.IsArray)
                throw new InvalidOperationException("");

            if (this.IsArray)
                throw new InvalidOperationException("");

            return CreateObjectFromString(_rawValue, type);
        }


        /// <summary>
        /// 根据指定的类型获得对应的实例数组
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public object[] GetValueArray(Type elementType)
        {
            if (elementType.IsArray)
                throw CreateJaggedArraysNotSupportedEx(elementType);

            int myArraySize = this.ArraySize;
            if (ArraySize < 0)
                return null;

            var values = new object[myArraySize];

            if (myArraySize > 0)
            {
                var enumerator = new InitialPropertyEnumerator(_rawValue, true);
                int iElem = 0;
                while (enumerator.Next())
                {
                    values[iElem] = CreateObjectFromString(enumerator.Current, elementType);
                    ++iElem;
                }
            }

            return values;
        }

        /// <summary>
        /// 根据类型返回对应的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetValue<T>()
        {
            var type = typeof(T);

            if (type.IsArray)
                throw new InvalidOperationException("");

            if (this.IsArray)
                throw new InvalidOperationException("");

            return (T)CreateObjectFromString(_rawValue, type);
        }

        /// <summary>
        /// 根据指定的类型获得对应的实例数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] GetValueArray<T>()
        {
            var type = typeof(T);

            if (type.IsArray)
                throw CreateJaggedArraysNotSupportedEx(type);

            int myArraySize = this.ArraySize;
            if (myArraySize < 0)
                return null;

            var values = new T[myArraySize];

            if (myArraySize > 0)
            {
                var enumerator = new InitialPropertyEnumerator(_rawValue, true);
                int iElem = 0;
                while (enumerator.Next())
                {
                    values[iElem] = (T)CreateObjectFromString(enumerator.Current, type);
                    ++iElem;
                }
            }

            return values;
        }

        /// <summary>
        /// 根据文本返回对应的类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dstType"></param>
        /// <returns></returns>
        private static object CreateObjectFromString(string value, Type dstType)
        {
            var underlyingType = Nullable.GetUnderlyingType(dstType);
            if (underlyingType != null)
            {
                if (string.IsNullOrEmpty(value))
                    return null;

                dstType = underlyingType;
            }

            var converter = InitialFile.FindTypeStringConverter(dstType);

            try
            {
                return converter.ConvertFromString(value, dstType);
            }
            catch (Exception ex)
            {
                throw InitialPropertyCastException.Create(value, dstType, ex);
            }
        }

        /// <summary>
        /// 根据当前对象设置当前的节项
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(object value)
        {
            if (value == null)
            {
                SetEmptyValue();
                return;
            }

            var type = value.GetType();
            if (type.IsArray)
            {
                if (type.GetElementType().IsArray)
                    throw CreateJaggedArraysNotSupportedEx(type.GetElementType());

                var values = value as Array;
                var strings = new string[values.Length];

                for (int i = 0; i < values.Length; i++)
                {
                    object elemValue = values.GetValue(i);
                    var converter = InitialFile.FindTypeStringConverter(elemValue.GetType());
                    strings[i] = converter.ConvertToString(elemValue);
                }

                _rawValue = string.Format("{{{0}}}", string.Join(InitialFile.ArrayElementSeparator.ToString(), strings));
                _cachedArraySize = values.Length;
                _shouldCalculateArraySize = false;
            }
            else
            {
                var converter = InitialFile.FindTypeStringConverter(type);
                _rawValue = converter.ConvertToString(value);
                _shouldCalculateArraySize = true;
            }
        }

        /// <summary>
        /// 设置为空
        /// </summary>
        private void SetEmptyValue()
        {
            _rawValue = string.Empty;
            _cachedArraySize = 0;
            _shouldCalculateArraySize = false;
        }

        /// <summary>
        /// 一般用于写入的时候，转出文本
        /// </summary>
        /// <returns></returns>
        protected override string GetStringExpression()
        {
            return string.Format("{0} = {1}", Name, _rawValue);
        }

        private static ArgumentException CreateJaggedArraysNotSupportedEx(Type type)
        {
            Type elementType = type.GetElementType();

            while (elementType.IsArray)
                elementType = elementType.GetElementType();

            throw new ArgumentException("");
        }
    }
}
