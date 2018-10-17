using System.ComponentModel;

namespace ShanDian.LSRestaurant.Com
{
    /// <summary>
    ///菜品类型： 10：普通菜品 15：加料辅菜 20：计重菜 30：加料菜（有加料辅菜的普通菜） 40：固定套餐 50：换菜套餐 60：多选套餐 70：拼合菜
    /// </summary>
    public enum DishTypeEnum
    {
        /// <summary>
        /// 10：普通菜品
        /// </summary>
        [Description("普通菜")]
        CommDish = 10,

        /// <summary>
        /// 15：加料辅菜
        /// </summary>
        ChargingFood = 15,

        /// <summary>
        /// 20：计重菜
        /// </summary>
        WeightDish = 20,

        /// <summary>
        /// 30：加料菜（有加料辅菜的普通菜）
        /// </summary>
        ChargingDish = 30,

        /// <summary>
        /// 40：固定套餐 
        /// </summary>
        [Description("套餐")]
        FixedSetMeal = 40,

        /// <summary>
        /// 50：换菜套餐
        /// </summary>
        ChangeSetMeal = 50,

        /// <summary>
        /// 60：多选套餐
        /// </summary>
        MultiSetMeal = 60,

        /// <summary>
        /// 70：拼合菜
        /// </summary>
        MixedDish = 70


    }
}
