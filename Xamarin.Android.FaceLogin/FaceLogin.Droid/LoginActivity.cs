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

            // ���O�C����ʂ��w��
            SetContentView(Resource.Layout.Login);

            _cameraPreview = FindViewById<CameraPreview>(Resource.Id.cameraPreview);
            _cameraPreview.OnTakePicture += async (sender, data) =>
            {
                // �ʐ^�B�e���̏���
                ProgressDialog dialog = null;
                try
                {
                    isAuthenticating = true;
                    // �_�C�A���O�\��
                    dialog = ProgressDialog.Show(this, "Please wait", "Now Authenticating...");

                    // stream�쐬
                    using (Stream stream = new MemoryStream(data))
                    {
                        // ��F��
                        var result = await _cognitiveServiceHelper.FaceLogin(stream);
                        if (result)
                        {
                            // ���O�C��������ʂ�
                            Toast.MakeText(this, "�F�؂ɐ������܂����B", ToastLength.Long).Show();
                            StartActivity(typeof(SuccessActivity));
                        }
                        else
                        {
                            // ���O�C�����s
                            Toast.MakeText(this, "�F�؂Ɏ��s���܂����B", ToastLength.Long).Show();
                        }
                    }
                }
                finally
                {
                    dialog.Dismiss();
                    isAuthenticating = false;
                }
            };

            // FaceDetector��ݒ�
            var detector = new FaceDetector.Builder(this)
                .SetLandmarkType(LandmarkDetectionType.All)
                .SetMode(FaceDetectionMode.Accurate)
                .Build();
            detector.SetProcessor(
                new MultiProcessor.Builder(new FaceTrackerFactory(OnFaceDetectedHandler)).Build());

            // CameraSource��ݒ�
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

            // �J�����̊J�n
            _cameraPreview.Start(_cameraSource);
        }

        protected override void OnPause()
        {
            base.OnPause();

            // �J�����̒�~
            _cameraPreview.Stop();
        }

        protected override void OnDestroy()
        {
            // �J�������\�[�X�̂̉��
            _cameraPreview.Release();
            base.OnDestroy();
        }

        public void OnFaceDetectedHandler()
        {
            if (isAuthenticating)
                return;
            
            // �J�����ŎB�e����
            _cameraPreview.TakePicture();
        }
    }
}