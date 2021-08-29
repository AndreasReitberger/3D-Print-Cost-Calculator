using AndreasReitberger.Models;
using log4net;
using PrintCostCalculator3d.Models._3dprinting;
using PrintCostCalculator3d.Resources.Localization;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace PrintCostCalculator3d.Models.Exporter
{
    public class CalculationFile
    {
        static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static DESCryptoServiceProvider key = new DESCryptoServiceProvider();
        static string secString = "U4fRwU^K#.fA+$8y";
        public static bool Save(Calculation3d calc, string path)
        {
            try
            {
                string appFolder = System.AppDomain.CurrentDomain.BaseDirectory;
                XmlSerializer x = new XmlSerializer(typeof(Calculation3d));
                DirectoryInfo tempDir = new DirectoryInfo(path);
                Directory.CreateDirectory(tempDir.Parent.FullName);
                TextWriter writer = new StreamWriter(tempDir.FullName);
                x.Serialize(writer, calc);
                writer.Close();
                return true;
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                throw exc;
                //logger.Error(ex.Message);
            }
        }

        public static bool Save(Calculation3d[] calcs, string path)
        {
            try
            {
                string appFolder = System.AppDomain.CurrentDomain.BaseDirectory;
                XmlSerializer x = new XmlSerializer(typeof(Calculation3d[]));
                DirectoryInfo tempDir = new DirectoryInfo(path);
                Directory.CreateDirectory(tempDir.Parent.FullName);
                TextWriter writer = new StreamWriter(tempDir.FullName);
                x.Serialize(writer, calcs);
                writer.Close();
                return true;
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                throw exc;
                //logger.Error(ex.Message);
            }
        }
        public static bool Load(string path, out Calculation3d calc)
        {
            try
            {
                // Construct an instance of the XmlSerializer with the type  
                // of object that is being deserialized.  
                XmlSerializer mySerializer =
                new XmlSerializer(typeof(Calculation3d));
                // To read the file, create a FileStream.  

                FileStream myFileStream = new FileStream(path, FileMode.Open);
                // Call the Deserialize method and cast to the object type.  
                Calculation3d retval = (Calculation3d)mySerializer.Deserialize(myFileStream);
                myFileStream.Close();
                calc = retval;
                return true;
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                throw exc;
                //logger.Error(ex.Message);
            }
        }
        public static bool Load(string path, out Calculation3d[] calcs)
        {
            try
            {
                // Construct an instance of the XmlSerializer with the type  
                // of object that is being deserialized.  
                XmlSerializer mySerializer =
                new XmlSerializer(typeof(Calculation3d[]));
                // To read the file, create a FileStream.  

                FileStream myFileStream = new FileStream(path, FileMode.Open);
                // Call the Deserialize method and cast to the object type.  
                Calculation3d[] retval = (Calculation3d[])mySerializer.Deserialize(myFileStream);
                myFileStream.Close();
                calcs = retval;
                return true;
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                throw exc;
            }
        }

        // https://stackoverflow.com/questions/965042/c-sharp-serializing-deserializing-a-des-encrypted-file-from-a-stream
        public static bool EncryptAndSerialize(string filename, Calculation3d obj)
        {
            try
            {
                var key = new DESCryptoServiceProvider();
                var e = key.CreateEncryptor(Encoding.ASCII.GetBytes("64bitPas"), Encoding.ASCII.GetBytes(secString));
                using (FileStream fs = File.Open(filename, FileMode.Create))
                {
                    using (CryptoStream cs = new CryptoStream(fs, e, CryptoStreamMode.Write))
                    {
                        XmlSerializer xmlser = new XmlSerializer(typeof(Calculation3d));
                        xmlser.Serialize(cs, obj);
                        return true;
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return false;
            }
        }
        public static bool EncryptAndSerialize(string filename, Calculation3d[] objs)
        {
            try
            {
                var key = new DESCryptoServiceProvider();
                var e = key.CreateEncryptor(Encoding.ASCII.GetBytes("64bitPas"), Encoding.ASCII.GetBytes(secString));
                using (FileStream fs = File.Open(filename, FileMode.Create))
                {
                    using (CryptoStream cs = new CryptoStream(fs, e, CryptoStreamMode.Write))
                    {
                        XmlSerializer xmlser = new XmlSerializer(typeof(Calculation3d[]));
                        xmlser.Serialize(cs, objs);
                        return true;
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return false;
            }
        }

        public static Calculation3d DecryptAndDeserialize(string filename)
        {
            try
            {
                var key = new DESCryptoServiceProvider();
                var d = key.CreateDecryptor(Encoding.ASCII.GetBytes("64bitPas"), Encoding.ASCII.GetBytes(secString));
                using (FileStream fs = File.Open(filename, FileMode.Open))
                {
                    using (CryptoStream cs = new CryptoStream(fs, d, CryptoStreamMode.Read))
                    {
                        XmlSerializer xmlser = new XmlSerializer(typeof(Calculation3d));
                        return (Calculation3d)xmlser.Deserialize(cs);
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return null;
            }
        }
        public static Calculation3d[] DecryptAndDeserializeArray(string filename)
        {
            try
            {
                var key = new DESCryptoServiceProvider();
                var d = key.CreateDecryptor(Encoding.ASCII.GetBytes("64bitPas"), Encoding.ASCII.GetBytes(secString));
                using (FileStream fs = File.Open(filename, FileMode.Open))
                {
                    using (CryptoStream cs = new CryptoStream(fs, d, CryptoStreamMode.Read))
                    {
                        XmlSerializer xmlser = new XmlSerializer(typeof(Calculation3d[]));
                        return (Calculation3d[])xmlser.Deserialize(cs);
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return null;
            }
        }
    }
}
