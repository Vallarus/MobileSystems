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
using Android.Locations;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Common;

using System.Net;
using System.IO;
using System.Json;
using System.Threading.Tasks;

namespace MobileSystems
{
    [Activity(Label = "Pozycja i Pogoda")]
    public class GPSAndRestActivity : Activity,GoogleApiClient.IConnectionCallbacks,GoogleApiClient.IOnConnectionFailedListener
    {
        private GoogleApiClient googleApiClient;
        private string lati;
        private string longi;
        const string restApiUrlFormat = "http://api.geonames.org/findNearByWeatherJSON?lat={0}&lng={1}&username=przemekjezewski";

        public async void OnConnected(Bundle connectionHint)
        {
            EditText latitude = FindViewById<EditText>(Resource.Id.latText);
            EditText longitude = FindViewById<EditText>(Resource.Id.longText);
            var lastLocation = LocationServices.FusedLocationApi.GetLastLocation(googleApiClient);
            if (lastLocation != null)
            {
                latitude.Text = lastLocation.Latitude.ToString();
                longitude.Text = lastLocation.Longitude.ToString();
            }
            else
            {
                Toast.MakeText(this, "Nie mo¿na pobraæ lokalizacji. U¿ywam lokalizacji dla Warszawy.", ToastLength.Long).Show();
                latitude.Text =GetString(Resource.String.WarsawLatitiude);
                longitude.Text = GetString(Resource.String.WarsawLongitiude);
            }
            string url = string.Format(restApiUrlFormat, latitude.Text, longitude.Text);
            JsonValue json = await FetchWeatherAsync(url);
            ParseAndDisplay(json);

        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            throw new NotImplementedException();
        }

        public void OnConnectionSuspended(int cause)
        {
            throw new NotImplementedException();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.GPSandRest);


            googleApiClient = new GoogleApiClient.Builder(this)
                .AddApi(Android.Gms.Location.LocationServices.API)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .Build();
            googleApiClient.Connect();
            



        }
        private async Task<JsonValue> FetchWeatherAsync(string url)
        {
           
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            
            using (WebResponse response = await request.GetResponseAsync())
            {
                
                using (Stream stream = response.GetResponseStream())
                {
                    
                    JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
                    Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

                  
                    return jsonDoc;
                }
            }
        }

       
        private void ParseAndDisplay(JsonValue json)
        {
            
            TextView location = FindViewById<TextView>(Resource.Id.locationText);
            TextView temperature = FindViewById<TextView>(Resource.Id.tempText);
            TextView humidity = FindViewById<TextView>(Resource.Id.humidText);
            TextView conditions = FindViewById<TextView>(Resource.Id.condText);

         
            JsonValue weatherResults = json["weatherObservation"];

           
            location.Text = weatherResults["stationName"];

            
            double temp = weatherResults["temperature"];
         
         
            temperature.Text = String.Format("{0:F1}", temp) + "° C";

            
            double humidPercent = weatherResults["humidity"];
            humidity.Text = humidPercent.ToString() + "%";

     
            string cloudy = weatherResults["clouds"];
            if (cloudy.Equals("n/a"))
                cloudy = "";
            string cond = weatherResults["weatherCondition"];
            if (cond.Equals("n/a"))
                cond = "";

           
            conditions.Text = cloudy + " " + cond;
        }

    }
}