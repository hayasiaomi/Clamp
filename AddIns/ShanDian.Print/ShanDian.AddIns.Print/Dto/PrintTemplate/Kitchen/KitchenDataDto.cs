using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShanDian.AddIns.Print.Dto.PrintDataDto;

namespace ShanDian.AddIns.Print.Dto.PrintTemplate
{
    public class KitchenDataDto 
    {
        public KitchenDataDto()
        {
            KitchenDishes=new List<KitchenDishDto>();
        }

        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime OrderTime { get; set; }

        /// <summary>
        /// 打印扫码点菜单餐桌模式
        /// 1. 桌牌号 2. 取餐号
        /// </summary>
        public string TableType { get; set; }

        /// <summary>
        /// 餐桌名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 人数
        /// </summary>
        public int Person { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 订单内菜品
        /// 整单打印
        /// </summary>
        public List<KitchenDishDto> KitchenDishes { get; set; }

        /// <summary>
        /// 打印时间
        /// </summary>
        public DateTime PrintTime { get; set; }
    }

    public class KitchenDishDto
    {
        public KitchenDishDto()
        {
            SubDishes = new List<KitchenSetMealDto>();
        }

        /// <summary>
        /// 菜品名 + 单位信息
        /// </summary>
        public string DishName { get; set; }

        /// <summary>
        /// 菜品分类ID
        /// </summary>
        public string DishTypeId { get; set; }

        /// <summary>
        /// 菜品数量
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 菜品单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 菜品数量是否重量单位
        /// </summary>
        public bool IsWeight { get; set; }

        /// <summary>
        /// 是否是套餐(是套餐：1，不是套餐：0)
        /// </summary>
        public bool IsSetMeal { get; set; }

        /// <summary>
        /// 记重菜单位信息(斤、两、克)
        /// </summary>
        public string WeightUnit { set; get; }

        /// <summary>
        /// 做法信息
        /// </summary>
        public string TagInfo { set; get; }

        /// <summary>
        /// 套餐子菜品
        /// </summary>
        public List<KitchenSetMealDto> SubDishes { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string DishRemark { get; set; }
    }

    public class KitchenSetMealDto
    {
        /// <summary>
        /// 子菜品名 
        /// </summary>
        public string DishSetName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 菜品分类ID
        /// </summary>
        public string SetMealTypeId { get; set; }

        /// <summary>
        /// 做法信息
        /// </summary>
        public string TagInfo { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string DishRemark { get; set; }
    }


}
