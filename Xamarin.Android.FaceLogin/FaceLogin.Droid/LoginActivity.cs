using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Gms.Vision;
using Android.Gms.Vision.Faces;

using FaceLogin.Core;

namespace FaceLogin.Droid
{
    [Activity(Label = "Login")]
    public class LoginActivity : Activity
    {
        CameraPreview _cameraPreview;
        CameraSource _cameraSource;

        CognitiveServiceHelper _cognitiveServiceHelper = new CognitiveServiceHelper();
        bool isAuthenticating = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // ログイン画面を指定
            SetContentView(Resource.Layout.Login);

            _cameraPreview = FindViewById<CameraPreview>(Resource.Id.cameraPreview);
            _cameraPreview.OnTakePicture += async (sender, data) =>
            {
                // 写真撮影時の処理
                ProgressDialog dialog = null;
                try
                {
                    isAuthenticating = true;
                    // ダイアログ表示
                    dialog = ProgressDialog.Show(this, "Please wait", "Now Authenticating...");

                    // stream作成
                    using (Stream stream = new MemoryStream(data))
                    {
                        // 顔認証
                        var result = await _cognitiveServiceHelper.FaceLogin(stream);
                        if (result)
                        {
                            // ログイン成功画面へ
                            Toast.MakeText(this, "認証に成功しました。", ToastLength.Long).Show();
                            StartActivity(typeof(SuccessActivity));
                        }
                        else
                        {
                            // ログイン失敗
                            Toast.MakeText(this, "認証に失敗しました。", ToastLength.Long).Show();
                        }
                    }
                }
                finally
                {
                    dialog.Dismiss();
                    isAuthenticating = false;
                }
            };

            // FaceDetectorを設定
            var detector = new FaceDetector.Builder(this)
                .SetLandmarkType(LandmarkDetectionType.All)
                .SetMode(FaceDetectionMode.Accurate)
                .Build();
            detector.SetProcessor(
                new MultiProcessor.Builder(new FaceTrackerFactory(OnFaceDetectedHandler)).Build());

            // CameraSourceを設定
            _cameraSource = new CameraSource.Builder(this, detector)
                .SetAutoFocusEnabled(true)
                //.SetRequestedPreviewSize(640,480)
                .SetFacing(CameraFacing.Back)
                .SetRequestedFps(30.0f)
                .Build();
        }

        protected override void OnResume()
        {
            base.OnResume();

            // カメラの開始
            _cameraPreview.Start(_cameraSource);
        }

        protected override void OnPause()
        {
            base.OnPause();

            // カメラの停止
            _cameraPreview.Stop();
        }

        protected override void OnDestroy()
        {
            // カメラリソースのの解放
            _cameraPreview.Release();
            base.OnDestroy();
        }

        public void OnFaceDetectedHandler()
        {
            if (isAuthenticating)
                return;
            
            // カメラで撮影する
            _cameraPreview.TakePicture();
        }
    }
}