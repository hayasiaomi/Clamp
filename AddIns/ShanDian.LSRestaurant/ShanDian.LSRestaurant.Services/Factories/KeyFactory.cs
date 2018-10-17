using System;
using System.ComponentModel;

namespace ShanDian.LSRestaurant.Factories
{
    public static class KeyFactory
    {
        private static readonly DateTime Basic = new DateTime(2016, 1, 1);
        private static long _lastTime = (long)((DateTime.Now - Basic).TotalMilliseconds);
        private static short _counter = 0;

        public static readonly int KeyNum = 19;


        /// <summary>
        /// 生成随机数（确保一毫秒内生成1000个不重复，时间可用到2050-11-3 00:00:00为止）
        /// </summary>
        /// <param name="count">一次生成多少个</param>
        /// <returns>生成14位到15位随机数</returns>
        private static long GetRandom(byte count = 1)
        {
            var newTime = (long)((DateTime.Now - Basic).TotalMilliseconds);
            var bytes = BitConverter.GetBytes(newTime);
            //bytes[7] = bytes[5];
            bytes[6] = bytes[4];
            bytes[5] = bytes[3];
            bytes[4] = bytes[2];
            bytes[3] = bytes[1];
            bytes[2] = bytes[0];
            long result;
            short startcount = _counter;
            lock (typeof(KeyFactory))
            {
                if (newTime != _lastTime)
                    startcount = _counter = 0;
                startcount++;
                var t = BitConverter.GetBytes(startcount);
                bytes[0] = t[0];
                bytes[1] = t[1];
                result = BitConverter.ToInt64(bytes, 0);
                _lastTime = newTime;
                _counter = (short)(_counter + count);
            }
            return result;
        }

        /// <summary>
        /// 生产表主键Id(19位 = 2位业务值 + 17位随机数)
        /// </summary>
        /// <param name="emun"></param>
        /// <returns></returns>
        public static long GetPrimaryKey(Enum emun)
        {
            var enumType = emun.GetType();
            var emunNum = Convert.ToInt32(Enum.Parse(enumType, emun.ToString()));
            var enumDes = (DescriptionAttribute)enumType.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
            string key = $"{enumDes.Description}{emunNum}{GetRandom():D15}";
            //string key = $"{emun}{GetRandom():D17}";
            return Convert.ToInt64(key);
        }
    }
}
