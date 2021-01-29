using System;
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
        private double iOSPerDensityScreenHeight;

        object _locker { get; set; }
        bool _isRunning { get; set; }
        public ICommand MessageContainerCommand { get; set; }
        TouchEffect MenuEffect { get; set; }
        TouchEffect ToggleEff { get; set; }
        double DragThreshold { get; set; } //Android 22 - iOS 50
        public double PerDensityScreenHeight { get; private set; }
        public double AndroidPerDensityScreenHeight { get; private set; }

        public Home()
        {
            _isRunning = false;
            InitializeComponent();
        }

        void SetUp()
        {
            BindingContext = this;

            _locker = new object();

            if (DeviceInfo.Platform == DevicePlatform.Android)
                DragThreshold = -90;
            else
                DragThreshold = -150;

            MessageContainerCommand = new Command(() => NavArrowTapped(null, null));

            MenuEffect = new TouchEffect();
            MenuEffect.Completed += MenuIconTapped;
            Menu.Effects.Add(MenuEffect);

            ToggleEff = new TouchEffect();
            ToggleEff.Completed += NavArrowTapped;
            ToggleArrow.Effects.Add(ToggleEff);

            SetNavHeight();
            SetFooterTranslationY();
        }

        void SetFooterTranslationY()
        {
            PerDensityScreenHeight = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                AndroidPerDensityScreenHeight = PerDensityScreenHeight - 100.94f;
                Footer.TranslationY = AndroidPerDensityScreenHeight;
            }
            else
            {
                iOSPerDensityScreenHeight = PerDensityScreenHeight * 70.96f;
                Footer.TranslationY = iOSPerDensityScreenHeight;
            }
        }

        void CleanUp()
        {
            BindingContext = null;

            _locker = null;

            MessageContainerCommand = null;

            MenuEffect = null;
            MenuEffect.Completed -= MenuIconTapped;
            Menu.Effects.Clear();

            ToggleEff = null;
            ToggleEff.Completed -= NavArrowTapped;
            ToggleArrow.Effects.Clear();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetUp();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            CleanUp();
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
            lock (_locker)
            {
                if (_isRunning)
                    return;
                else
                {
                    _isRunning = true;
                }
            }
            if (ToggleArrow.RotationX == 0)
            {
                await MainThread.InvokeOnMainThreadAsync(async () => await OpenDropDown(HiddenMessageContainer));
            }
            else if (ToggleArrow.RotationX == 180)
            {
                await MainThread.InvokeOnMainThreadAsync(async () => await CloseDropDown(HiddenMessageContainer));
            }
            _isRunning = false;
        }

        async Task CloseDropDown(VisualElement hiddenMessageContainer)
        {
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

        async void HomeScrollView_Scrolled(System.Object sender, Xamarin.Forms.ScrolledEventArgs e)
        {
            if (ToggleArrow.RotationX == 180)
                await CloseDropDown(HiddenMessageContainer);
        }

        double y { get; set; }
        double totalDragY { get; set; } = 0.0;
        void FooterDragged(System.Object sender, Xamarin.Forms.PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    totalDragY = Math.Max(Math.Min(0, y + e.TotalY), -Math.Abs((Height * .25) - Height));
                    Footer.TranslateTo(Footer.X, totalDragY, 20);
                    Debug.WriteLine($"OS: {DeviceInfo.Platform} - Dragging - Footer Value: {totalDragY} TotalY Value{e.TotalY}");
                    break;
                case GestureStatus.Completed:
                    // Store the translation applied during the pan
                    y = Footer.TranslationY;

                    //at the end of the event - snap to the closest location
                    var finalTranslation = Math.Max(Math.Min(0, -1000), -Math.Abs(GetClosestLockState(e.TotalY + y)));

                    //depending on Swipe Up or Down - change the snapping animation
                    if (IsSwipeUp(e))
                    {
                        Footer.TranslateTo(Footer.X, finalTranslation, 250, Easing.SpringIn);
                    }
                    else
                    {
                        Footer.TranslateTo(Footer.X, finalTranslation, 250, Easing.SpringOut);
                    }

                    y = Footer.TranslationY;
                    break;
            }
            Debug.WriteLine($"OS: {DeviceInfo.Platform} Dragging: {y}");
        }

        bool IsSwipeUp(PanUpdatedEventArgs e) => e.TotalY < 0 ? true : false;

        double GetClosestLockState(double TranslationY)
        {
            //Play with these values to adjust the locking motions - this will change depending on the amount of content on a page
            var lockStates = new double[] { 0, .2};

            //get the current proportion of the sheet in relation to the screen
            var distance = Math.Abs(TranslationY);
            var currentProportion = distance / Height;

            //calculate which lockstate it's the closest to
            var smallestDistance = 10000.0;
            var closestIndex = 0;
            for (var i = 0; i < lockStates.Length; i++)
            {
                var state = lockStates[i];
                var absoluteDistance = Math.Abs(state - currentProportion);
                if (absoluteDistance < smallestDistance)
                {
                    smallestDistance = absoluteDistance;
                    closestIndex = i;
                }
            }

            var selectedLockState = lockStates[closestIndex];
            var TranslateToLockState = GetProportionCoordinate(selectedLockState);

            return TranslateToLockState;
        }

        double GetProportionCoordinate(double proportion) =>
            proportion * Height;
    }
}
