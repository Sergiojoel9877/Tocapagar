using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.Material;
using Shiny;
using Android.Content;

namespace Tocapagar.Droid
{
    [Activity
        (
            Label = "Tocapagar",
            Icon = "@mipmap/icon",
            Theme = "@style/MainTheme",
            MainLauncher = true,
            ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation
        )
    ]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        readonly Lazy<App> _app = new Lazy<App>(()=> new App());

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            this.ShinyOnCreate();
            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.SetFlags("");
            
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);
            FormsMaterial.Init(this, savedInstanceState);
            LoadApplication(_app.Value);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            this.ShinyRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnNewIntent(Intent intent)
        {
            this.ShinyOnNewIntent(intent);
            base.OnNewIntent(intent);
        }
    }
}