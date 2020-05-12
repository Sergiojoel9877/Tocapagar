using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tocapagar.Helpers;
using Xamarin.Forms.PancakeView;

namespace Tocapagar.Views
{
    public partial class HomePage : ContentPage
    {
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
            if (sender is Label arrowIcon)
            {
                var arrow = (Label)FindByName("ToggleArrow");
                var navBar = (PancakeView)FindByName("NavBar");
                var messageContainer = (PancakeView)FindByName("MessageContainer");

                var containerWidth = messageContainer.Width;
                var containerHeight = messageContainer.Height;
                var navBarHeight = navBar.Height;

                var animationDown = new Animation(s => navBar.HeightRequest = s, 80, 360);
                var animationUp = new Animation(s => navBar.HeightRequest = s, 360, 80);

                //var animationIncreaseMessageContainerWidthSize = new Animation(s => messageContainer.WidthRequest = s, containerWidth, navBar.Width);
                //var animationIncreaseMessageContainerHeightSize = new Animation(s => messageContainer.HeightRequest = s, containerHeight, 150);

                var animationNavBarResizeHeightSize = new Animation(s => navBar.HeightRequest = s, 80, 0);

                if (arrowIcon.Text == Icons.AngleDown)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        //animationNavBarResizeHeightSize.Commit(navBar, "navBarAnimationResizeHeight", 16, 250, Easing.SinOut, (d,b)=>
                        //{
                        //    navBar.HeightRequest = 1;
                        //}, ()=> false);

                        await navBar.TranslateTo(0, -500, 200, Easing.CubicInOut);

                        //animationNavBarResizeHeightSize.Commit(navBar, "navBarAnimationResizeHeight", 16, 250, Easing.Linear, (d, b) =>
                        //{
                        //    navBar.HeightRequest = navBarHeight;
                        //}, () => false);

                        await navBar.TranslateTo(0, 0, 200, Easing.Linear);

                        animationDown.Commit(navBar, "navBarAnimationDown", 16, 250, Easing.BounceOut, (d, b) =>
                        {
                            navBar.HeightRequest = 360;
                        }, () => false);

                        //await messageContainer.TranslateTo(0, 80);

                        //animationIncreaseMessageContainerWidthSize.Commit(messageContainer, "messageContainerWidth", 16, 250, Easing.Linear, (d, b) =>
                        //{
                        //    messageContainer.WidthRequest = navBar.Width;
                        //}, ()=> false);

                        //animationIncreaseMessageContainerHeightSize.Commit(messageContainer, "messageContainerHeight", 16, 250, Easing.Linear, (d, b) =>
                        //{
                        //    messageContainer.HeightRequest = 150;
                        //}, ()=> false);

                        arrow.Text = Icons.AngleUp;
                    });
                    return;
                }

                if (arrowIcon.Text == Icons.AngleUp)
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        animationUp.Commit(navBar, "navBarAnimationUp", 16, 250, Easing.Linear, (d, b) =>
                        {
                            navBar.HeightRequest = 80;
                        }, () => false);
                    });
                    arrow.Text = Icons.AngleDown;
                    return;
                }
            }
        }
    }
}
