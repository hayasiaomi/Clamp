using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.LSRestaurant.Interface.Dto.Dishes
{
    public class DishGroupSummaryDto
    {
        /// <summary>
        /// 菜品组Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 菜品组类型： 10：套餐子菜品 20：可更换菜列表 30：多选套餐组  40：加料菜辅菜 50：拼合主菜 60：拼合辅菜
        /// </summary>
        public int GroupType { get; set; }


        public List<ChildDishSummaryDto> ChildDishList { get; set; } = new List<ChildDishSummaryDto>();
    }
}
