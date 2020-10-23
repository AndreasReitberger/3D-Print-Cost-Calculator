using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndreasReitberger.Models;
using Firebase.Database;
using Firebase.Database.Query;

namespace PrintCostCalculator3d.Models
{
    public class FirebaseHandler
    {
        /*
         * var firebaseConfig = {
            apiKey: "AIzaSyClcBzbn7i4O4ki4AwQNL4XxOEgQJFtXD8",
            authDomain: "d-print-cost-calculator-app.firebaseapp.com",
            databaseURL: "https://d-print-cost-calculator-app.firebaseio.com",
            projectId: "d-print-cost-calculator-app",
            storageBucket: "d-print-cost-calculator-app.appspot.com",
            messagingSenderId: "1054673104364",
            appId: "1:1054673104364:web:90a88c873e37920561e4c9"
          };
        */

        #region Variablse
        string authKey = "8R9Eac6MeET453bPR7UkPLxQbZNcIlaRDEGnYDFC";
        string baseUri = "https://d-print-cost-calculator-app.firebaseio.com";
        #endregion

        #region Constructor
        public FirebaseHandler(string authenticationKey)
        {
            authKey = authenticationKey;
        }
        #endregion

        #region Public
        public async Task AddDefaultManufacturer(Manufacturer manufacturer)
        {
            try
            {
                using (var client = new FirebaseClient(baseUri, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authKey)
                }))
                {
                    await client.Child("Manufacturers").Child(manufacturer.Name).PutAsync(manufacturer);
                }
            }
            catch (Exception exc)
            {

            }
        }
        public async Task<ObservableCollection<Manufacturer>> GetDefaultManufacturers()
        {
            ObservableCollection<Manufacturer> result = new ObservableCollection<Manufacturer>();
            try
            {
                using (var client = new FirebaseClient(baseUri, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authKey)
                }))
                {
                    var manufacturers = await client.Child("Manufacturers").OrderByKey().OnceAsync<Manufacturer>();
                    if(manufacturers != null)
                    {
                         result = new ObservableCollection<Manufacturer>(
                            manufacturers.Select(manufacturer => manufacturer.Object)
                            );
                    }
                }
            }
            catch(Exception exc)
            {

            }
            return result;
        }

        public async Task AddDefaultMaterial(Material3d material)
        {
            try
            {
                using (var client = new FirebaseClient(baseUri, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authKey)
                }))
                {
                    await client.Child("Materials").Child(material.Name).PutAsync(material);
                }
            }
            catch (Exception exc)
            {

            }
        }
        public async Task<ObservableCollection<Material3d>> GetDefaultMaterials()
        {
            ObservableCollection<Material3d> result = new ObservableCollection<Material3d>();
            try
            {
                using (var client = new FirebaseClient(baseUri, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authKey)
                }))
                {
                    var list = await client.Child("Materials").OrderByKey().OnceAsync<Material3d>();
                    if(list != null)
                    {
                         result = new ObservableCollection<Material3d>(
                            list.Select(item => item.Object)
                            );
                    }
                }
            }
            catch(Exception exc)
            {

            }
            return result;
        }

        public async Task AddDefaultPrinter(Printer3d printer)
        {
            try
            {
                using (var client = new FirebaseClient(baseUri, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authKey)
                }))
                {
                    await client.Child("Printers").Child(printer.Name).PutAsync(printer);
                }
            }
            catch (Exception exc)
            {

            }
        }
        public async Task<ObservableCollection<Printer3d>> GetDefaultPrinters()
        {
            ObservableCollection<Printer3d> result = new ObservableCollection<Printer3d>();
            try
            {
                using (var client = new FirebaseClient(baseUri, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authKey)
                }))
                {
                    var list = await client.Child("Printers").OrderByKey().OnceAsync<Printer3d>();
                    if(list != null)
                    {
                         result = new ObservableCollection<Printer3d>(
                            list.Select(item => item.Object)
                            );
                    }
                }
            }
            catch(Exception exc)
            {

            }
            return result;
        }
        #endregion
    }
}
