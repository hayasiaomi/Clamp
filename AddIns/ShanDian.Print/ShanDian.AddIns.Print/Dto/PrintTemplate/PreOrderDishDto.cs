using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintTemplate
{
    public class PreOrderDishDto
    {
        public PreOrderDishDto()
        {
            this.SubDishes = new List<PreOrderDishDto>();
            this.WeightNames = new List<string>();
            this.PracticeNames = new List<string>();
        }
        /// <summary>
        /// 菜品
        /// </summary>
        public string DishName { set; get; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal TakeCount { set; get; }

        /// <summary>
        /// 菜品备注
        /// </summary>
        public string DishDescription { set; get; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { set; get; }

        /// <summary>
        /// 分量名称
        /// </summary>
        public List<string> WeightNames { set; get; }
        /// <summary>
        /// 做法名称
        /// </summary>
        public List<string> PracticeNames { set; get; }
        /// <summary>
        /// 套餐子菜品
        /// </summary>
        public List<PreOrderDishDto> SubDishes { set; get; }
    }
}
