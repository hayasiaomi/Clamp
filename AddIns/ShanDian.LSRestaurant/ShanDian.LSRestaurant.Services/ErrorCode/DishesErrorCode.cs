using ShanDian.Software.SDK;

namespace ShanDian.LSRestaurant.ErrorCode
{
    class DishesErrorCode : SystemError<DishesErrorCode>
    {
        /// <summary>
        /// 执行异常
        /// </summary>
        public int ExecError = 1401;

        /// <summary>
        /// 不存在数据
        /// </summary>
        public int NotDataError = 1402;

        /// <summary>
        /// 不允许删除操作
        /// </summary>
        public int NoDeleteError = 1403;

        /// <summary>
        /// 违反唯一值规则
        /// </summary>
        public int UniqueError = 1404;
    }
}
