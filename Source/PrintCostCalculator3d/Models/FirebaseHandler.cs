using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndreasReitberger.Models;
using AndreasReitberger.Models.MaterialAdditions;
using Firebase.Database;
using Firebase.Database.Query;
using PrintCostCalculator3d.Models.Settings;

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
        protected string authKey = "VYtufjvl7JrSJB9wKO9NZ3R5AzA3Kf5fiXXODreq";
        readonly string baseUri = "https://d-print-cost-calculator-app.firebaseio.com";
        #endregion

        #region Instance
        static FirebaseHandler _instance;
        public static FirebaseHandler Instance
        {
            get => _instance;
            set
            {
                if (_instance == value) return;
                _instance = value;
            }
        }
        #endregion

        #region Constructor
        public FirebaseHandler(string authenticationKey)
        {
            authKey = authenticationKey;
            Instance = this;
        }
        #endregion

        #region Public
        public async Task CheckForNewConfig()
        {
            ObservableCollection<Manufacturer> result = new();
            try
            {
                using FirebaseClient client = new(baseUri, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authKey)
                });
                ObservableCollection<Material3dType> materialTypes = await GetDefaultMaterialTypes();
            }
            catch (Exception)
            {

            }
        }

        public async Task<ObservableCollection<Material3dType>> GetDefaultMaterialTypes()
        {
            ObservableCollection<Material3dType> result = new();
            try
            {
                using FirebaseClient client = new(baseUri, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authKey)
                });
                IReadOnlyCollection<FirebaseObject<Material3dType>> config = await client.Child("Configuration").Child("MaterialTypes").OrderByKey().OnceAsync<Material3dType>();
                if (config != null)
                {
                    /**/
                    result = new ObservableCollection<Material3dType>(
                        config.Select(item => item.Object)
                        );
                }
            }
            catch (Exception)
            {
                return new ObservableCollection<Material3dType>();
            }
            return result;
        }

        public async Task AddMaterialTypes(ObservableCollection<Material3dType> materialTypes)
        {
            try
            {
                using FirebaseClient client = new(baseUri, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authKey)
                });
                foreach (Material3dType type in materialTypes)
                {
                    try
                    {
                        await client.Child("Configuration").Child("MaterialTypes").Child(type.Material.Replace(".", " ")).PutAsync(type);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                }
            }
            catch (Exception)
            {

            }
        }

        public async Task AddDefaultManufacturer(Manufacturer manufacturer)
        {
            try
            {
                using FirebaseClient client = new(baseUri, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authKey)
                });
                await client.Child("Manufacturers").Child(manufacturer.Name).PutAsync(manufacturer);
            }
            catch (Exception)
            {

            }
        }
        public async Task<ObservableCollection<Manufacturer>> GetDefaultManufacturers()
        {
            ObservableCollection<Manufacturer> result = new();
            try
            {
                using FirebaseClient client = new(baseUri, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authKey)
                });
                IReadOnlyCollection<FirebaseObject<Manufacturer>> manufacturers = await client.Child("Manufacturers").OrderByKey().OnceAsync<Manufacturer>();
                if (manufacturers != null)
                {
                    result = new ObservableCollection<Manufacturer>(
                       manufacturers.Select(manufacturer => manufacturer.Object)
                       );
                }
            }
            catch(Exception)
            {

            }
            return result;
        }

        public async Task AddDefaultMaterial(Material3d material)
        {
            try
            {
                using FirebaseClient client = new(baseUri, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authKey)
                });
                await client.Child("Materials").Child(material.Name).PutAsync(material);
            }
            catch (Exception)
            {

            }
        }
        public async Task<ObservableCollection<Material3d>> GetDefaultMaterials()
        {
            ObservableCollection<Material3d> result = new();
            try
            {
                using FirebaseClient client = new(baseUri, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authKey)
                });
                IReadOnlyCollection<FirebaseObject<Material3d>> list = await client.Child("Materials").OrderByKey().OnceAsync<Material3d>();
                if (list != null)
                {
                    result = new ObservableCollection<Material3d>(
                       list.Select(item => item.Object)
                       );
                }
            }
            catch(Exception)
            {

            }
            return result;
        }

        public async Task AddDefaultPrinter(Printer3d printer)
        {
            try
            {
                using FirebaseClient client = new(baseUri, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authKey)
                });
                await client.Child("Printers").Child(printer.Name).PutAsync(printer);
            }
            catch (Exception)
            {

            }
        }
        public async Task<ObservableCollection<Printer3d>> GetDefaultPrinters()
        {
            ObservableCollection<Printer3d> result = new();
            try
            {
                using FirebaseClient client = new(baseUri, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authKey)
                });
                IReadOnlyCollection<FirebaseObject<Printer3d>> list = await client.Child("Printers").OrderByKey().OnceAsync<Printer3d>();
                if (list != null)
                {
                    result = new ObservableCollection<Printer3d>(
                       list.Select(item => item.Object)
                       );
                }
            }
            catch(Exception)
            {

            }
            return result;
        }
        #endregion
    }
}
