using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.LSRestaurant.Interface.Dto.Dishes
{
    public class DishMenuDetailDto
    {
        /// <summary>
        /// 菜品Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 菜品编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 菜品名称(唯一，【菜品与加料菜之间可重名】)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜品分类Id
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// 菜品分类名称
        /// </summary>
        public string CategoryDishName { get; set; }

        /// <summary>
        /// 菜品单价
        /// </summary>
        public decimal Price { get; set; } = 0;

        /// <summary>
        /// 菜品类型： 10：普通菜品 15：加料辅菜 20：计重菜 30：加料菜（有加料辅菜的普通菜） 40：固定套餐 50：换菜套餐 60：多选套餐 70：拼合菜
        /// </summary>
        public int DishType { get; set; } = 10;

        /// <summary>
        /// 菜品单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 是否参加全场优惠：0 否，1 是  ；默认1
        /// </summary>
        public int IsDiscount { get; set; } = 1;

        /// <summary>
        /// 餐盒费
        /// </summary>
        public decimal BoxFee { get; set; } = 0;

        /// <summary>
        /// 菜品估清数量： -1 不估清，0 已估清，>0 估清剩余数量
        /// </summary>
        public decimal EstimateCnt { get; set; } = -1;

        /// <summary>
        /// 菜品分量
        /// </summary>
        public List<SkusDishDto> SkusDishList { get; set; } = new List<SkusDishDto>();

        /// <summary>
        /// 菜品做法
        /// </summary>
        public List<PracticeDetailDto> PracticeList { get; set; } = new List<PracticeDetailDto>();

        /// <summary>
        /// 菜品组（子菜品、加料辅菜）
        /// </summary>
        public List<MenuDishGroupDto> DishGroupList { get; set; } = new List<MenuDishGroupDto>();

    }
}
