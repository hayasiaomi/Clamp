namespace ShanDian.LSRestaurant.ViewModel.Dishes
{
    public class RelationDish
    {
        /// <summary>
        /// 子菜品Id
        /// </summary>
        public long ChildDishId { get; set; }

        /// <summary>
        /// 菜品组Id
        /// </summary>
        public long DishGroupId { get; set; }

        /// <summary>
        /// 菜品Id
        /// </summary>
        public long DishId { get; set; }

        /// <summary>
        /// 菜品份Id
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// 菜品组类型： 10：套餐子菜品 20：可更换菜列表 30：多选套餐组  40：加料菜辅菜 50：拼合主菜 60：拼合辅菜
        /// </summary>
        public int GroupType { get; set; }
    }
}
