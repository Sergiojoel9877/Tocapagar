using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tocapagar.Helpers;
using Xamarin.Forms.PancakeView;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using System.Linq;
using Xamarin.Essentials;

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

            var NB = GR;
            var SIZE = 150;

            if (sender is Label)
                if (ToggleArrow.RotationX == 0)
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var tsk1 = ToggleArrow.RotateXTo(180);
                        var tsk2 = NB.LayoutTo(new Rectangle(NB.Bounds.X, NB.Bounds.Y, NB.Bounds.Width, NB.Bounds.Height + SIZE), easing: Easing.SinOut);
                        await Task.WhenAll(tsk1, tsk2);
                        return;
                    });
                else if (ToggleArrow.RotationX == 180)
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        var tsk1 = ToggleArrow.RotateXTo(0);
                        var tsk2 = NB.LayoutTo(new Rectangle(NB.Bounds.X, NB.Bounds.Y, NB.Bounds.Width, NB.Bounds.Height - SIZE), easing: Easing.SinIn);
                        await Task.WhenAll(tsk1, tsk2);
                        return;
                    });
            #region v2
            //var NB = GR.RowDefinitions.FirstOrDefault();

            //var SIZE = 150 /*NavBar.Height - 3000 * Math.PI / (Math.Pow(NavBar.Height, 2) / Math.Log(500))*/;
            //var animationNavBarResizeHeightSizeUp = new Animation(s => NB.Height = s, NB.Height.Value, NB.Height.Value + SIZE);
            //var animationNavBarResizeHeightSizeDown = new Animation(s => NB.Height = s, NB.Height.Value, NB.Height.Value - SIZE);

            //if (sender is Label)
            //    if (ToggleArrow.RotationX == 0)
            //        await Device.InvokeOnMainThreadAsync(async () =>
            //        {
            //            //await BX.ScaleTo(0, 50);
            //            MessageContainer2.IsVisible = true;
            //            animationNavBarResizeHeightSizeUp.Commit(GR, "ChangeHeightUp", 16, 250, Easing.Linear, (d, b) =>
            //            {
            //                GR.RowDefinitions.First().Height = NB.Height;
            //            }, () => false);
            //            await ToggleArrow.RotateXTo(180);
            //            return;
            //        });
            //    else if (ToggleArrow.RotationX == 180)
            //        await Device.InvokeOnMainThreadAsync(async () =>
            //        {
            //            animationNavBarResizeHeightSizeDown.Commit(GR, "ChangeHeightDown", 16, 250, Easing.Linear, (d, b) =>
            //            {
            //                GR.RowDefinitions.First().Height = NB.Height;
            //            }, () => false);
            //            await ToggleArrow.RotateXTo(0);
            //            MessageContainer2.IsVisible = false;
            //            //await BX.ScaleTo(1, 50);
            //            return;
            //        });
            #endregion
            #region v1
            //var SIZE = 300 /*NavBar.Height - 3000 * Math.PI / (Math.Pow(NavBar.Height, 2) / Math.Log(500))*/;
            //var animationNavBarResizeHeightSizeUp = new Animation(s => NavBar.HeightRequest = s, NavBar.Height, NavBar.Height + SIZE);
            //var animationNavBarResizeHeightSizeDown = new Animation(s => NavBar.HeightRequest = s, NavBar.Height, NavBar.Height - SIZE);

            //if (sender is Label)
            //    if (ToggleArrow.RotationX == 0)
            //        await Device.InvokeOnMainThreadAsync(async () =>
            //        {
            //            MessageContainer2.IsVisible = true;
            //            animationNavBarResizeHeightSizeUp.Commit(NavBar, "ChangeHeightUp", 16, 250, Easing.Linear, (d, b) =>
            //            {
            //                NavBar.HeightRequest = NavBar.HeightRequest;
            //            }, () => false);
            //            await ToggleArrow.RotateXTo(180);
            //            return;
            //        });
            //    else if (ToggleArrow.RotationX == 180)
            //        await Device.InvokeOnMainThreadAsync(async () =>
            //        {
            //            animationNavBarResizeHeightSizeDown.Commit(NavBar, "ChangeHeightDown", 16, 250, Easing.Linear, (d, b) =>
            //            {
            //                NavBar.HeightRequest = NavBar.HeightRequest;
            //            }, () => false);
            //            await ToggleArrow.RotateXTo(0);
            //            MessageContainer2.IsVisible = false;
            //            return;
            //        });
            #endregion
            _tryCount--;
        }

        public static async Task AnimateViewInAbsoluteLayout(View view, Rectangle rectDest, uint time, Easing easing = null)
        {
            await view.LayoutTo(rectDest, time, easing);
            AbsoluteLayout.SetLayoutBounds(view, rectDest);
            AbsoluteLayout.SetLayoutFlags(view, AbsoluteLayoutFlags.WidthProportional);
        }
    }
}
