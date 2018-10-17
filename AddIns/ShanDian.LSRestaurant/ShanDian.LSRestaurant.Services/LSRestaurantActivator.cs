using ShanDian.AddIns;
using ShanDian.LSRestaurant.Services;

namespace ShanDian.LSRestaurant
{
    public class LSRestaurantActivator : ShanDian.AddIns.AddInActivator
    {

        public static AddIn AddIn;

        public override void Start(AddInContext addInContext)
        {
            AddIn = addInContext.AddIn;

            DatabaseServices.InitDatabase();
            //DatabaseServices.LoadStandardVerSiteMethod();
            //BusinessHelper.Instance.RestSystemMode();
        }

        public override void Stop(AddInContext addInContext)
        {

        }
    }
}
