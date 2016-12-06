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
using Android.Util;
using Android.Views;
using Android.Widget;

using Android.Gms.Vision;

namespace FaceLogin.Droid
{
    public class CameraPreview : SurfaceView, ISurfaceHolderCallback, CameraSource.IShutterCallback, CameraSource.IPictureCallback
    {
        Context _context;
        CameraSource _cameraSource;

        public event EventHandler<byte[]> OnTakePicture;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attrs"></param>
        public CameraPreview(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            _context = context;
            base.Holder.AddCallback(this);
        }

        /// <summary>
        /// �N��
        /// </summary>
        /// <param name="cameraSource"></param>
        public void Start(CameraSource cameraSource)
        {
            _cameraSource = cameraSource;
        }

        /// <summary>
        /// �J�����N��
        /// </summary>
        void Start()
        {
            _cameraSource.Start(base.Holder);
        }

        /// <summary>
        /// ��~
        /// </summary>
        public void Stop()
        {
            _cameraSource?.Stop();
        }

        /// <summary>
        /// ���\�[�X���
        /// </summary>
        public void Release()
        {
            _cameraSource.Release();
            _cameraSource = null;
        }

        /// <summary>
        /// �ʐ^���B��
        /// </summary>
        public void TakePicture()
        {
            _cameraSource.TakePicture(this, this);
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {

        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            // �J�����J�n
            Start();
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            Stop();
        }

        public void OnShutter()
        {
        }

        public void OnPictureTaken(byte[] data)
        {
            OnTakePicture?.Invoke(_context, data);
        }


        /// <summary>
        /// �v���r���[�摜���擾
        /// </summary>
        /// <returns></returns>
        public Bitmap GetPreviewImage()
        {
            Canvas orgCanvas = null;
            // �ۑ��pBitmap�p��
            var bmp = Bitmap.CreateBitmap(base.Width, base.Height, Bitmap.Config.Argb8888);
            try
            {
                orgCanvas = base.Holder.LockCanvas();
                // Canvas��Bitmap��ݒ�
                Canvas canvas = new Canvas(bmp);
                // Canvas��`��
                base.Draw(canvas);
            }
            finally
            {
                if (orgCanvas != null)
                    base.Holder.UnlockCanvasAndPost(orgCanvas);
            }
            // Bitmap��Ԃ�
            return bmp;
        }

        /// <summary>
        /// �v���r���[�摜����Stream���擾
        /// </summary>
        /// <returns></returns>
        public Stream GetPreviewImageStream()
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                GetPreviewImage().Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
            }
            return stream;
        }

        public void SaveBitmapToSd()
        {
            try
            {
                var sdPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                var filePath = System.IO.Path.Combine(sdPath, "test.jpg");
                var stream = new FileStream(filePath, FileMode.Create);
                GetPreviewImage().Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                stream.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
            }

        }

    }
}