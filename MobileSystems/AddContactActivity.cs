using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;
using static Android.Provider.ContactsContract;
using System.Collections.Generic;

namespace MobileSystems
{
    [Activity(Label = "@string/ApplicationName")]
    public class AddContactActivity : Activity
    {
        private EditText nameControl;
        EditText numberControl;
        Button addContactButton;
        Button navigateToContactsButton;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
           

            SetContentView(Resource.Layout.AddContact);            
            addContactButton = FindViewById<Button>(Resource.Id.AddContact);
            nameControl = FindViewById<EditText>(Resource.Id.Name);
            numberControl = FindViewById<EditText>(Resource.Id.Number);
            navigateToContactsButton = FindViewById<Button>(Resource.Id.NavigateToContacts);
            addContactButton.Click += AddContactClick;
            navigateToContactsButton.Click += delegate {
                Intent intent = new Intent(Intent.ActionDefault,
Android.Provider.ContactsContract.Contacts.ContentUri);
                StartActivity(intent); };
            
        }
        
        private void AddContactClick(Object sender, EventArgs e)
        {
            string name = nameControl.Text;
            string number = numberControl.Text;
            if (NameIsNotValid(name))
                return;
            if (NumberIsNotValid(number))
                return;            

            AddContact(name, number);
            Toast.MakeText(this, string.Format("Dodałem kontakt : \n {0} \n {1} ", name, number), ToastLength.Long).Show();
            nameControl.Text = string.Empty;
            numberControl.Text = string.Empty;
        }

        private bool NameIsNotValid(string input)
        {
            if(string.IsNullOrEmpty(input))
            {
                
                ShowToast("Nazwa kontaktu nie może być pusta");
                return true;
            }
            return false;
        }

        private void ShowToast(string message)
        {
            Toast.MakeText(this, message, ToastLength.Short).Show();
        }

        private bool NumberIsNotValid(string input)
        {
            if(string.IsNullOrEmpty(input))
            {
                ShowToast("Numer nie może być pusty.");
                return true;
            }
            if(input.Length > 14 || input.Length < 9)
            {
                ShowToast("Numer powinien mieć od 9 do 14 znaków");
                return true;
            }

            return false;
        }

        private void AddContact(string name, string number)
        {
            List<ContentProviderOperation> ops = new List<ContentProviderOperation>();
            int rawContactInsertIndex = ops.Count;
            ops.Add(ContentProviderOperation.NewInsert(Android.Provider.ContactsContract.RawContacts.ContentUri)
                .WithValue(Android.Provider.ContactsContract.RawContacts.InterfaceConsts.AccountType, null)
                .WithValue(Android.Provider.ContactsContract.RawContacts.InterfaceConsts.AccountName, null).Build());
            ops.Add(ContentProviderOperation
                .NewInsert(Android.Provider.ContactsContract.Data.ContentUri)
                .WithValueBackReference(Android.Provider.ContactsContract.Data.InterfaceConsts.RawContactId, rawContactInsertIndex)
                .WithValue(Android.Provider.ContactsContract.Data.InterfaceConsts.Mimetype, Android.Provider.ContactsContract.CommonDataKinds.StructuredName.ContentItemType)
                .WithValue(Android.Provider.ContactsContract.CommonDataKinds.StructuredName.DisplayName, name) 
                .Build());
            ops.Add(ContentProviderOperation
                .NewInsert(Android.Provider.ContactsContract.Data.ContentUri)
                .WithValueBackReference(
                    ContactsContract.Data.InterfaceConsts.RawContactId, rawContactInsertIndex)
                .WithValue(Android.Provider.ContactsContract.Data.InterfaceConsts.Mimetype, Android.Provider.ContactsContract.CommonDataKinds.Phone.ContentItemType)
                .WithValue(Android.Provider.ContactsContract.CommonDataKinds.Phone.Number, number) 
                .WithValue(Android.Provider.ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Type, "mobile").Build()); 

                           
            try
            {
                ContentResolver.ApplyBatch(ContactsContract.Authority, ops);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Exception: " + ex.Message, ToastLength.Long).Show();
            }
        }
    }
}

