using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tocapagar.Helpers;
using Xamarin.Forms.PancakeView;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;

namespace Tocapagar.Views
{
    public partial class HomePage : ContentPage
    {
        readonly object _locker = new object();

        public HomePage()
        {
            InitializeComponent();
        }

        void TouchEff_Completed(Xamarin.Forms.VisualElement sender, TouchEffect.EventArgs.TouchCompletedEventArgs args)
        {
            Shell.Current.FlyoutIsPresented = true;
        }

        void NavArrowTapped(Xamarin.Forms.VisualElement sender, TouchEffect.EventArgs.TouchCompletedEventArgs args)
        {
            lock (sender)
            {
                if (sender is Label arrowIcon)
                {
                    var arrow = (Label)FindByName("ToggleArrow");
                    var arrow2 = (Label)FindByName("ToggleArrow2");
                    var navBar = (PancakeView)FindByName("NavBar");
                    var navBar2 = (PancakeView)FindByName("NavBar2");
                    var messageContainer = (PancakeView)FindByName("MessageContainer");
                    var messageContainer2 = MessageContainer2;
                    var navGrid = (Grid)FindByName("NavGrid");
                    var containerWidth = messageContainer.Width;
                    var containerHeight = messageContainer.Height;
                    var navBarHeight = navBar.Height;
                    navBar2.IsEnabled = false;
                    navBar2.IsVisible = false;
                    MainGrid.LowerChild(navBar2);
                    messageContainer2.FadeTo(0,200);
                    //await messageContainer.FadeTo(1, 200);

                    if (arrowIcon.Text == Icons.AngleDown)
                    {
                        Device.InvokeOnMainThreadAsync(async () =>
                        {
                            await messageContainer.FadeTo(0, 200);
                            navBar2.IsEnabled = true;
                            navBar2.IsVisible = true;
                            MainGrid.RaiseChild(navBar2);
                            await AnimateViewInAbsoluteLayout(navBar2, AbsoluteLayout.GetLayoutBounds(navBar2), new Rectangle(0, 0, NavBar2.Bounds.Width, NavBar2.Bounds.Height + 200), 250, Easing.Linear);
                            await messageContainer2.FadeTo(1, 200);
                            arrow2.Text = Icons.AngleUp;
                        }).SafeFireAndForget();
                        return;
                    }

                    if (arrowIcon.Text == Icons.AngleUp)
                    {
                        Device.InvokeOnMainThreadAsync(async () =>
                        {
                            arrow2.Text = Icons.AngleDown;
                            await messageContainer2.FadeTo(0, 200);
                            await AnimateViewInAbsoluteLayout(navBar2, AbsoluteLayout.GetLayoutBounds(navBar2), new Rectangle(0, 0, NavBar2.Width, NavBar2.Height), 250, Easing.Linear);
                            MainGrid.LowerChild(navBar2); 
                            navBar2.IsEnabled = false;
                            navBar2.IsVisible = false;
                            await messageContainer.FadeTo(1, 200);
                        }).SafeFireAndForget();
                        return;
                    }
                }
            }
        }

        public static async Task AnimateViewInAbsoluteLayout(View view, Rectangle rectAbsolute, Rectangle rectDest, uint time, Easing easing = null)
        {
            await view.LayoutTo(rectDest, time, easing);
            AbsoluteLayout.SetLayoutBounds(view, rectAbsolute);
            AbsoluteLayout.SetLayoutFlags(view, AbsoluteLayoutFlags.All);
        }
    }
}
