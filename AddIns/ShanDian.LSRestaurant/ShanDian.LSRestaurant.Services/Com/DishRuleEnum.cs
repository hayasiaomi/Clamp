namespace ShanDian.LSRestaurant.Com
{
    public enum DishRuleEnum
    {
        /// <summary>
        /// 是否必选项：0 非，1是
        /// </summary>
        IsMust,

        /// <summary>
        /// 最多点菜数量
        /// </summary>
        MaxAmount,

        /// <summary>
        /// 最少点菜数量
        /// </summary>
        MinAmount,

        /// <summary>
        /// 每次加点数量
        /// </summary>
        StepAmount,

        /// <summary>
        /// 计重菜点菜
        /// [0,0]为顾客手动输入
        ///其他值为商户设置的可选重量，顾客只能选择这些重量下单,商户最后还需要进行斤两确认，
        ///[0,1] 前端显示为1斤以下
        ///[1.5,2] 前端显示 为 1.5-2斤
        ///[3,0] 前端显示 为3斤以上
        /// </summary>
        WeightType,

        /// <summary>
        /// 计重菜_重量确认:1、需要重量确认，0、不需要重量确认
        /// </summary>
        WeightConfirm,

        /// <summary>
        /// 是否支持多选,拼合菜使用，1、同道主菜可以选多份，0、同道主菜不支持选择多份
        /// </summary>
        MultiChoose,

        /// <summary>
        /// 必点类型：0非必点，10按人数必点，20按订单必点
        /// </summary>
        MustOrderType,

        /// <summary>
        /// 必点数量
        /// </summary>
        MustAmount,

        /// <summary>
        /// 套餐子菜做法规则:1：子菜品不使用任何做法，2：使用做法但不加价，3：使用做法并且加价
        /// </summary>
        SubSpecType,

        /// <summary>
        /// 是否默认 :0 非默认，1默认
        /// </summary>
        IsDefault


    }
}
