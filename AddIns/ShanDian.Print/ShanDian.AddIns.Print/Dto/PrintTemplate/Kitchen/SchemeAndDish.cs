using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintTemplate.Kitchen
{
    public class SchemeAndDish
    {
        public SchemeAndDish()
        {
            IsSetmeal = false;
            IsNullScheme = false;
            SetmealList = new List<SchemeAndDishSetmeal>();
        }

        public int SchemeId { get; set; }

        public string DishTypeId { get; set; }

        public bool IsSetmeal { get; set; }

        public List<SchemeAndDishSetmeal> SetmealList { get; set; }

        /// <summary>
        /// 区分套餐的标识
        /// </summary>
        public int SetmealFlowId { get; set; }

        /// <summary>
        /// 是否没有打印方案
        /// </summary>
        public bool IsNullScheme { get; set; }
    }

    public class SchemeAndDishSetmeal
    {
        public SchemeAndDishSetmeal()
        {
            IsNullScheme = false;
        }

        public int SchemeId { get; set; }

        public string DishName { get; set; }

        public string DishId { get; set; }

        public string SetDishTypeId { get; set; }

        public bool IsNullScheme { get; set; }
    }
}
