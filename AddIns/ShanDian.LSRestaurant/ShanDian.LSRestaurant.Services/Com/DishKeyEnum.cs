using System.ComponentModel;

namespace ShanDian.LSRestaurant.Com
{
    /// <summary>
    /// 定义菜品各表主键id头部枚举值(Description=10,代表之前菜品组件值)
    /// </summary>
    [Description("10")]
    public enum DishKeyEnum
    {
        /// <summary>
        /// 菜品分类
        /// </summary>
        Category = 10,
        /// <summary>
        /// 做法组
        /// </summary>
        PracticeGroup = 20,
        /// <summary>
        /// 做法详情
        /// </summary>
        PracticeInfo = 21,
        /// <summary>
        /// 菜品做法
        /// </summary>
        PracticeDish = 22,
        /// <summary>
        /// 菜品
        /// </summary>
        Dish = 30,
        /// <summary>
        /// 菜品组
        /// </summary>
        DishGroup = 31,
        /// <summary>
        /// 子菜品
        /// </summary>
        ChildDish = 32,
        /// <summary>
        /// 估清
        /// </summary>
        Estimate = 33,
        /// <summary>
        /// 份量
        /// </summary>
        SkusDish = 40,

        /// <summary>
        /// 单位
        /// </summary>
        Unit = 50
    }
}
