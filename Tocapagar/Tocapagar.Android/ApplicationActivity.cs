using System;
using Android.App;
using Android.Runtime;
using Shiny;
using Tocapagar.Services;

namespace Tocapagar.Droid
{
    [Application]
    public class ApplicationActivity : Application
    {
        public ApplicationActivity(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            this.ShinyOnCreate(new BackgroundJobStartup());
        }
    }
}
