using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Tocapagar.Helpers
{
    public static class Extensions
    {
        #region GRID
        public static Grid RaiseView(this Grid grid, View element)
        {
            grid.RaiseChild(element);
            return grid;
        }

        public static void LowView(this Grid grid, View element)
        {
            grid.LowerChild(element);
        }
        #endregion

        #region View
        public static Task AnimateView(this View view, Rectangle rectDest, uint time = 250, Easing easing = null)
        {
            return view.LayoutTo(rectDest, time, easing);
        }
        #endregion
    }
}
