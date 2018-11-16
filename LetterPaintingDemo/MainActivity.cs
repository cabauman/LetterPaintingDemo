using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace LetterPaintingDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var drawTest = new CustomView(this, null);
            var metrics = Resources.DisplayMetrics;
            var width = metrics.WidthPixels;

            SetContentView(Resource.Layout.activity_main);
            AddContentView(drawTest, new ViewGroup.LayoutParams(width, width));

            var halfHeight = metrics.HeightPixels / 2;
            drawTest.SetY(halfHeight - (width / 2));

            var btn = FindViewById<Button>(Resource.Id.ResetButton);
            btn.Click += delegate { drawTest.Reset(); };
        }

        private void Test(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_main);

            var metrics = Resources.DisplayMetrics;
            var widthInDp = ConvertPixelsToDp(metrics.WidthPixels);
            var heightInDp = ConvertPixelsToDp(metrics.HeightPixels);

            //FindViewById<TextView>(Resource.Id.screenWidthDp).Text = "Screen Width: " + widthInDp + " dp.";
            //FindViewById<TextView>(Resource.Id.screenHeightDp).Text = "Screen Height: " + heightInDp + " dp.";
        }

        private int ConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }
    }
}

