﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tocapagar.Helpers;
using Xamarin.Forms.PancakeView;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using Sharpnado.Tasks;
using Tocapagar.Helpers.Icons;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Effects;
using System.Diagnostics;

namespace Tocapagar.Views
{
    public partial class Home : ContentPage
    {
        object Locker { get; set; }
        bool IsRunning { get; set; }
        double DragThreshold { get; set; }
        public double PerDensityScreenHeight { get; private set; }
        double iOSPerDensityScreenHeight;
        double AndroidPerDensityScreenHeight { get; set; }
        double PageHeight { get; set; }
        double FooterInitialTranslationY { get; set; }
        double FooterOpenedValue { get; set; }
        double LastVelocity { get; set; }
        double PreviousPositionY { get; set; }

        public ICommand MessageContainerCommand { get; set; }

        TouchEffect MenuEffect { get; set; }
        TouchEffect ToggleEff { get; set; }
        AnimationStateMachine AnimationStateMachine { get; set; }

        public Home()
        {
            IsRunning = false;
            InitializeComponent();
        }

        void SetUp()
        {
            BindingContext = this;

            Locker = new object();

            AnimationStateMachine = new AnimationStateMachine();

            DragThreshold = 200;

            MessageContainerCommand = new Command(() => NavArrowTapped(null, null));

            MenuEffect = new TouchEffect();
            MenuEffect.Completed += MenuIconTapped;
            Menu.Effects.Add(MenuEffect);

            ToggleEff = new TouchEffect();
            ToggleEff.Completed += NavArrowTapped;
            ToggleArrow.Effects.Add(ToggleEff);

            MainGrid.LowerChild(Footer);

            SetNavHeight();
            SetFooterTranslationY();
            SetFooterHeight();
        }

        void SetFooterHeight()
        {
            Footer.HeightRequest = PerDensityScreenHeight * 0.76;
        }

        void SetFooterTranslationY()
        {
            PerDensityScreenHeight = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                //Emulator-fixed-val: 280.48 | -percentage-val: | Device-fixed-val: 229.05 | -percentage-val: (1st iteration: 306) (2nd iteration: 162 ) (3rd iteration: 234)  |
                AndroidPerDensityScreenHeight = PerDensityScreenHeight - (PerDensityScreenHeight * 0.36837648273f) /*130.94f*/;
                FooterInitialTranslationY = AndroidPerDensityScreenHeight + (AndroidPerDensityScreenHeight * 0.12);
                Footer.TranslationY = FooterInitialTranslationY;
            }
            else
            {
                //iP11-fixed-val: 283.039993286133 | -percentage-val: 261.492131710052| Device-fixed-val: 229.05 | -percentage-val: (1st iteration: 306) (2nd iteration: 162 ) (3rd iteration: 234)  |
                iOSPerDensityScreenHeight = PerDensityScreenHeight - (PerDensityScreenHeight * 0.36837648273f) /*130.96f*/;
                FooterInitialTranslationY = iOSPerDensityScreenHeight + (iOSPerDensityScreenHeight * 0.20);
                Footer.TranslationY = FooterInitialTranslationY;
            }
        }

        void CleanUp()
        {
            BindingContext = null;

            Locker = null;

            MessageContainerCommand = null;

            AnimationStateMachine = null;

            MenuEffect.Completed -= MenuIconTapped;
            MenuEffect = null;
            Menu.Effects.Clear();

            ToggleEff.Completed -= NavArrowTapped;
            ToggleEff = null;
            ToggleArrow.Effects.Clear();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            PageHeight = height;
            SetupStates();
        }

        private void SetupStates()
        {
            AnimationStateMachine = new AnimationStateMachine();

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                FooterOpenedValue = AndroidPerDensityScreenHeight * 0.12;
                AnimationStateMachine.Add(PageStates.Open, new ViewTransition[] {
                    new ViewTransition(Footer, AnimationType.TranslationY, FooterOpenedValue),
                    new ViewTransition(Footer, AnimationType.Opacity, 1)
                });
            }
            else
            {
                FooterOpenedValue = iOSPerDensityScreenHeight * 0.13;
                AnimationStateMachine.Add(PageStates.Open, new ViewTransition[] {
                    new ViewTransition(Footer, AnimationType.TranslationY, FooterOpenedValue),
                    new ViewTransition(Footer, AnimationType.Opacity, 1)
                });
            }
            AnimationStateMachine.Add(PageStates.Peek, new ViewTransition[] {
                new ViewTransition(Footer, AnimationType.TranslationY, FooterInitialTranslationY),
                new ViewTransition(Footer, AnimationType.Opacity, 0.7)
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetUp();
        }

        protected override void OnDisappearing()
        {
            CleanUp();
            base.OnDisappearing();
        }

        void SetNavHeight()
        {
            var height = DeviceDisplay.MainDisplayInfo.Width / (DeviceDisplay.MainDisplayInfo.Density * DeviceDisplay.MainDisplayInfo.Density * 5.5f);
            GR.HeightRequest = height;
        }

        void MenuIconTapped(object sender, EventArgs args)
        {
            Shell.Current.FlyoutIsPresented = true;
        }

        public async void NavArrowTapped(object sender, EventArgs args)
        {
            lock (Locker)
                if (IsRunning)
                    return;
                else
                    IsRunning = true;

            if (ToggleArrow.RotationX == 0)
                await MainThread.InvokeOnMainThreadAsync(async () => await OpenDropDown(HiddenMessageContainer));
            else if (ToggleArrow.RotationX == 180)
                await MainThread.InvokeOnMainThreadAsync(async () => await CloseDropDown(HiddenMessageContainer));

            IsRunning = false;
        }

        async Task CloseDropDown(VisualElement hiddenMessageContainer)
        {
            await Task.Yield();
            _ = hiddenMessageContainer.RotateXTo(-90, easing: Easing.SpringOut);
            await hiddenMessageContainer.FadeTo(0, easing: Easing.Linear);
            _ = ToggleArrow.RotateXTo(0);
            hiddenMessageContainer.IsVisible = false;
            Debug.WriteLine($"OS: {DeviceInfo.Platform} ClosingDropDown");
            return;
        }

        async Task OpenDropDown(VisualElement hiddenMessageContainer)
        {
            MainGrid.RaiseChild(HiddenMessageContainer);
            hiddenMessageContainer.RotationX = -90;
            hiddenMessageContainer.TranslationY = -GR.HeightRequest / 3.5;
            hiddenMessageContainer.IsVisible = true;
            await Task.WhenAll
            (
                ToggleArrow.RotateXTo(180),
                hiddenMessageContainer.FadeTo(1, easing: Easing.Linear),
                hiddenMessageContainer.RotateXTo(0, easing: Easing.SpringOut)
            );
            Debug.WriteLine($"OS: {DeviceInfo.Platform} OpeningDropDown");
            return;
        }

        async void HomeScrollView_Scrolled(object sender, Xamarin.Forms.ScrolledEventArgs e)
        {
            if (ToggleArrow.RotationX == 180)
                await CloseDropDown(HiddenMessageContainer);
        }

        async void FooterDragged(object sender, Xamarin.Forms.PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    PreviousPositionY = e.TotalY;
                    if (DeviceInfo.Platform == DevicePlatform.iOS)
                        PreviousPositionY += Footer.TranslationY;
                    break;

                case GestureStatus.Running:
                    var delta = PreviousPositionY + (e.TotalY - 0.5f);
                    if (DeviceInfo.Platform == DevicePlatform.Android)
                        delta += Footer.TranslationY;
                    if (IsValidDelta(delta))
                        await Task.WhenAll(UpdateFooterTranslationYAsync(delta), UpdateFooterOpacity(e));
#if DEBUG
                    Debug.WriteLine($"OS: {DeviceInfo.Platform} Dragging: {delta} Footer Y: {Footer.TranslationY} Footer Opacity: {Footer.Opacity}");
#endif
                    break;

                case GestureStatus.Completed:
                    if (Footer.TranslationY < DragThreshold)
                        AnimationStateMachine.Go(PageStates.Open);
                    else
                        AnimationStateMachine.Go(PageStates.Peek);
                    if (ToggleArrow.RotationX == 180)
                        _ = CloseDropDown(HiddenMessageContainer);
                    break;

                case GestureStatus.Canceled:
                    break;
            }
        }

        Task<bool> UpdateFooterOpacity(PanUpdatedEventArgs e) => (!IsSwipeUp(e)) ? Footer.FadeTo(0.7) : Footer.FadeTo(1);
        
        bool IsSwipeUp(PanUpdatedEventArgs e) => e.TotalY < 0.01;

        bool IsValidDelta(double delta) => delta > 15 && delta <= (FooterInitialTranslationY + 5);

        readonly Action<VisualElement, double> SetYTranslation = (footer, delta) => footer.TranslationY = delta;

        Task UpdateFooterTranslationYAsync(double delta)
            =>  (Math.Abs(delta) > 0.01)
            ?   MainThread.InvokeOnMainThreadAsync(()=> SetYTranslation(Footer,delta))
            :   Task.CompletedTask;
    }
}
