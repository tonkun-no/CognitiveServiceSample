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
        /// コンストラクタ
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
        /// 起動
        /// </summary>
        /// <param name="cameraSource"></param>
        public void Start(CameraSource cameraSource)
        {
            _cameraSource = cameraSource;
        }

        /// <summary>
        /// カメラ起動
        /// </summary>
        void Start()
        {
            _cameraSource.Start(base.Holder);
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            _cameraSource?.Stop();
        }

        /// <summary>
        /// リソース解放
        /// </summary>
        public void Release()
        {
            _cameraSource.Release();
            _cameraSource = null;
        }

        /// <summary>
        /// 写真を撮る
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
            // カメラ開始
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
        /// プレビュー画像を取得
        /// </summary>
        /// <returns></returns>
        public Bitmap GetPreviewImage()
        {
            Canvas orgCanvas = null;
            // 保存用Bitmap用意
            var bmp = Bitmap.CreateBitmap(base.Width, base.Height, Bitmap.Config.Argb8888);
            try
            {
                orgCanvas = base.Holder.LockCanvas();
                // CanvasにBitmapを設定
                Canvas canvas = new Canvas(bmp);
                // Canvasを描画
                base.Draw(canvas);
            }
            finally
            {
                if (orgCanvas != null)
                    base.Holder.UnlockCanvasAndPost(orgCanvas);
            }
            // Bitmapを返す
            return bmp;
        }

        /// <summary>
        /// プレビュー画像からStreamを取得
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