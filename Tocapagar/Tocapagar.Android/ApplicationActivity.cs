using System;
using Android.App;
using Android.Runtime;
using Shiny;
using Tocapagar.Services;

namespace Tocapagar.Droid
{
    [Application]
    public class ApplicationActivity : ShinyAndroidApplication<BackgroundJobStartup>
    {
        public ApplicationActivity(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }
    }
}
