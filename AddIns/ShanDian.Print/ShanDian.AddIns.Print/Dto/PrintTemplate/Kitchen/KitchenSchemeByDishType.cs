using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShanDian.AddIns.Print.Dto.PrintDataDto;
using ShanDian.AddIns.Print.Dto.WebPrint.Dto;

namespace ShanDian.AddIns.Print.Dto.PrintTemplate.Kitchen
{
    public class KitchenSchemeByDishType
    {
        public KitchenSchemeByDishType()
        {
            PrintConfigs = new PrintConfigDto();
            KitchenDishes = new List<KitchenDish>();
            VoucherList = new List<SchemeVoucherDto>();
        }

        public PrintConfigDto PrintConfigs { get; set; }

        public List<KitchenDish> KitchenDishes { get; set; }

        public List<SchemeVoucherDto> VoucherList { get; set; }
    }
}
