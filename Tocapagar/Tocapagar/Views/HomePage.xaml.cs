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
        int _tryCount { get; set; } = 0;

        public HomePage()
        {
            InitializeComponent();
        }

        void TouchEff_Completed(Xamarin.Forms.VisualElement sender, TouchEffect.EventArgs.TouchCompletedEventArgs args)
        {
            Shell.Current.FlyoutIsPresented = true;
        }

        async void NavArrowTapped(Xamarin.Forms.VisualElement sender, TouchEffect.EventArgs.TouchCompletedEventArgs args)
        {
            _tryCount++;

            if (_tryCount > 1)
                return;

            if (sender is Label)
                if (ToggleArrow.RotationX == 0)
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await Task.WhenAll(new Task[]
                        {
                            ToggleArrow.RotateXTo(180),
                            AnimateViewInAbsoluteLayout(NavBar, AbsoluteLayout.GetLayoutBounds(NavBar), new Rectangle(0, 0, NavBar.Bounds.Width, NavBar.Bounds.Height + 200), 250, Easing.Linear),
                        });
                        MessageContainer2.IsVisible = true;
                    });
                else if(ToggleArrow.RotationX == 180)
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        MessageContainer2.IsVisible = false;
                        await Task.WhenAll(new Task[]
                        {
                            ToggleArrow.RotateXTo(0),
                            AnimateViewInAbsoluteLayout(NavBar, AbsoluteLayout.GetLayoutBounds(NavBar), new Rectangle(0, 0, NavBar.Bounds.Width, NavBar.Bounds.Height - 200), 250, Easing.Linear)
                        });
                    });

            _tryCount = 0;
        }

        public static async Task AnimateViewInAbsoluteLayout(View view, Rectangle rectAbsolute, Rectangle rectDest, uint time, Easing easing = null)
        {
            await view.LayoutTo(rectDest, time, easing);
            AbsoluteLayout.SetLayoutBounds(view, rectAbsolute);
            AbsoluteLayout.SetLayoutFlags(view, AbsoluteLayoutFlags.All);
        }
    }
}
