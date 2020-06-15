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
        bool _hasEntered { get; set; } = false;

        public HomePage()
        {
            InitializeComponent();
        }

        void TouchEff_Completed(Xamarin.Forms.VisualElement sender, TouchEffect.EventArgs.TouchCompletedEventArgs args)
        {
            Shell.Current.FlyoutIsPresented = true;
        }

        void SaveMessageContainerCoordinates()
        {
            Preferences.Set("f", MessageContainer.Bounds.ToString());
            Preferences.Set("MC.X", MessageContainer.X);
            Preferences.Set("MC.Y", MessageContainer.Y);
            Preferences.Set("MC.Width", MessageContainer.Width);
            Preferences.Set("MC.Height", MessageContainer.Height);
        }

        async void NavArrowTapped(Xamarin.Forms.VisualElement sender, TouchEffect.EventArgs.TouchCompletedEventArgs args)
        {
            if(!_hasEntered)
                SaveMessageContainerCoordinates();

            var NB = GR;
            var MC = MessageContainer;
            var SIZE = 150;
          
            if (sender is Label)
                if (ToggleArrow.RotationX == 0)
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        NavGrid.RaiseChild(MessageContainer);
                        MCTitle.Text = "Recuerda que pagar a tiempo hace que tu vida financiera sea mejor.";
                        MCTitle.FontSize = 20;
                        var tsk1 = ToggleArrow.RotateXTo(180);
                        var tsk2 = NB.LayoutTo(new Rectangle(NB.Bounds.X, NB.Bounds.Y, NB.Bounds.Width, NB.Bounds.Height + SIZE), easing: Easing.SinOut);
                        var tsk3 = MC.LayoutTo(new Rectangle(NB.Bounds.X, MC.Bounds.Y + 60, NavBar.Bounds.Width - 32 - 14, MC.Bounds.Height + 80), easing: Easing.SinOut);
                        await Task.WhenAll(tsk1, tsk2, tsk3);
                        return;
                    });
                else if (ToggleArrow.RotationX == 180)
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        var x = Preferences.Get("MC.X", 0.0);
                        var y = Preferences.Get("MC.Y", 0.0);
                        var w = Preferences.Get("MC.Width", 0.0);
                        var h = Preferences.Get("MC.Height", 0.0);
                        MCTitle.Text = "Hey Sergio ...";
                        MCTitle.FontSize = 15;
                        var tsk1 = ToggleArrow.RotateXTo(0);
                        var tsk2 = NB.LayoutTo(new Rectangle(NB.Bounds.X, NB.Bounds.Y, NB.Bounds.Width, NB.Bounds.Height - SIZE), easing: Easing.SinIn);
                        var tsk3 = MC.LayoutTo(new Rectangle(x, y, w, h), easing: Easing.SinOut);
                        await Task.WhenAll(tsk1, tsk2, tsk3);
                        return;
                    });

            _hasEntered = true;
        }

        public static async Task AnimateViewInAbsoluteLayout(View view, Rectangle rectDest, uint time, Easing easing = null)
        {
            await view.LayoutTo(rectDest, time, easing);
            AbsoluteLayout.SetLayoutBounds(view, rectDest);
            AbsoluteLayout.SetLayoutFlags(view, AbsoluteLayoutFlags.WidthProportional);
        }
    }
}
