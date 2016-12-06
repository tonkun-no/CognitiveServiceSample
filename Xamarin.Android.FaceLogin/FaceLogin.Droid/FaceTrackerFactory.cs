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

using Android.Gms.Vision;

namespace FaceLogin.Droid
{
    public class FaceTrackerFactory
        : Java.Lang.Object, MultiProcessor.IFactory
    {
        Action _faceTrackedCreatedHandler;
        public FaceTrackerFactory(Action faceTrackedCreatedHandler)
        {
            _faceTrackedCreatedHandler = faceTrackedCreatedHandler;
        }

        public Tracker Create(Java.Lang.Object item)
        {
            return new FaceTracker(_faceTrackedCreatedHandler);
        }
    }

    internal class FaceTracker
        : Tracker
    {
        Action _onNewItemHandler;
        public FaceTracker(Action onNewItemAction)
        {
            _onNewItemHandler = onNewItemAction;
        }

        public override void OnNewItem(int id, Java.Lang.Object item)
        {
            Console.WriteLine("OnNewItem");
            // äÁÇîFéØÇµÇΩç€ÇÃèàóùÇèëÇ≠
            _onNewItemHandler();
        }

        public override void OnUpdate(Detector.Detections detections, Java.Lang.Object item)
        {
        }

        public override void OnMissing(Detector.Detections detections)
        {
        }

        public override void OnDone()
        {
        }

    }
}