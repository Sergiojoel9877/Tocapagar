using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
//using Tocapagar.Services;
using Tocapagar.Views;

namespace Tocapagar
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            //c.Register<MockDataStore>();
            MainPage = new Lazy<AppShell>(()=> new AppShell()).Value;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
