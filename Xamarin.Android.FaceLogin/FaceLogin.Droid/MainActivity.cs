using Android.App;
using Android.Widget;
using Android.OS;
using System.IO;
using System;

namespace FaceLogin.Droid
{
    [Activity(Label = "FaceLogin", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            Button buttonLogin = FindViewById<Button>(Resource.Id.loginButton);
            buttonLogin.Click += (arg, e) =>
            {
                // ログイン画面へ
                StartActivity(typeof(LoginActivity));
            };
        }
    }
}

