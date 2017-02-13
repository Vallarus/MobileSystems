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
using Android.Hardware;

namespace MobileSystems
{
    [Activity(Label = "Akcelerometr")]
    public class AcceloremeterActivity : Activity, ISensorEventListener
    {
        static readonly object _syncLock = new object();
        SensorManager _sensorManager;
        EditText xControl, yControl, zControl, accControl;





        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Accelerometer);
            _sensorManager = (SensorManager)GetSystemService(Context.SensorService);
            xControl= FindViewById<EditText>(Resource.Id.accx);
            yControl = FindViewById<EditText>(Resource.Id.accy);
            zControl = FindViewById<EditText>(Resource.Id.accz);
            accControl = FindViewById<EditText>(Resource.Id.accTotal);


        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            throw new NotImplementedException();
        }

        public void OnSensorChanged(SensorEvent e)
        {
            const double g=9.81;
            lock (_syncLock)
            {

                float x, y, z;
                    double acc;
                x = e.Values[0];
                y = e.Values[1];
                z = e.Values[2];
                acc = Math.Sqrt(x * x + y * y + z * z) ;
                double overWeight = acc / g;
                xControl.Text = string.Format("{0} m/s²", x.ToString("0.000"));
                yControl.Text = string.Format("{0} m/s²", y.ToString("0.000"));
                zControl.Text = string.Format("{0} m/s²", z.ToString("0.000"));
                accControl.Text = string.Format("{0} g", overWeight.ToString("0.000"));
            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            _sensorManager.RegisterListener(this,
                                            _sensorManager.GetDefaultSensor(SensorType.Accelerometer),
                                            SensorDelay.Ui);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _sensorManager.UnregisterListener(this);
        }
    }
}