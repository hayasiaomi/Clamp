namespace ShanDian.LSRestaurant.ViewModel.Dishes
{
    public class DishGroupChild
    {
        /// <summary>
        /// 菜品组Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 菜品组类型： 10：套餐子菜品 20：可更换菜列表 30：多选套餐组  40：加料菜辅菜 50：拼合主菜 60：拼合辅菜
        /// </summary>
        public int GroupType { get; set; }

        /// <summary>
        /// 菜品分类Id
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// 子菜品主键Id
        /// </summary>
        public long ChildDishId { get; set; }

        /// <summary>
        /// 菜品Id
        /// </summary>
        public long DishId { get; set; }

        /// <summary>
        /// 菜品名称
        /// </summary>
        public string DishName { get; set; }

        /// <summary>
        /// 打包费
        /// </summary>
        public int BoxFee { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 菜品单价（以分计价）
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 菜品类型： 10：普通菜品 15：加料辅菜 20：计重菜 30：加料菜（有加料辅菜的普通菜） 40：固定套餐 50：换菜套餐 60：多选套餐 70：拼合菜
        /// 加料辅菜只能是 10（无份量）、15
        /// </summary>
        public int DishType { get; set; }

        /// <summary>
        /// 是否启用份量：0 否，1 是
        /// </summary>
        public int IsSkus { get; set; } = 0;

        /// <summary>
        /// 是否启用做法：0 关，1 公共做法，2私有做法；默认0
        /// </summary>
        public int IsPractice { get; set; } = 0;


        /// <summary>
        /// 是否启用加料菜：0 关，1 公共加料，2 私有加料；默认0
        /// </summary>
        public int IsCharging { get; set; } = 0;

    }
}
