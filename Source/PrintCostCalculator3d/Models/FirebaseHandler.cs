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

        #region Variablse
        string authKey = "";
        string baseUri = "";
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
