using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FaceLogin.Droid
{
    [Activity(Label = "LoginSuccess")]
    public class SuccessActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // ŽŸ‚Ì‰æ–Ê‚ðŽw’è
            SetContentView(Resource.Layout.Success);
        }
    }
}