using AndreasReitberger.Enums;
using AndreasReitberger.Models;
using AndreasReitberger.Models.PrinterAdditions;
using log4net;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.ViewModels._3dPrinting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintCostCalculator3d.Utilities
{
    public static class InstanceConverter
    {
        /*
         * 
         *
         */    
        #region Variables
        static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Material
        public static Material3d GetMaterialFromInstance(NewMaterialViewModel instance, Material3d material = null)
        {
            Material3d temp = material ?? new Material3d();
            try
            {
                temp.Id = instance.Id;
                temp.Name = instance.Name;
                temp.SKU = instance.SKU;
                temp.UnitPrice = instance.Price;
                temp.Unit = instance.Unit;
                temp.Uri = instance.LinkToReorder;
                temp.PackageSize = instance.PackageSize;
                //Supplier = instance.Supplier;
                temp.Manufacturer = instance.Manufacturer;
                temp.Density = instance.Density;
                temp.FactorLToKg = instance.FactorLiterToKg;
                temp.TypeOfMaterial = instance.TypeOfMaterial;
                temp.MaterialFamily = instance.MaterialFamily;
                temp.Attributes = instance.Attributes.ToList();

                temp.ProcedureAttributes = new List<AndreasReitberger.Models.MaterialAdditions.Material3dProcedureAttribute>();
                if (temp.MaterialFamily == Material3dFamily.Powder)
                {
                    temp.ProcedureAttributes.Add(
                        new AndreasReitberger.Models.MaterialAdditions.Material3dProcedureAttribute()
                        {
                            Family = instance.MaterialFamily,
                            //Procedure = Printer3dType.SLS,
                            Attribute = ProcedureAttribute.MaterialRefreshingRatio,
                            Value = instance.RefreshingRate,
                        });
                }

            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.DialogExceptionFormatedContent, exc.Message, exc.TargetSite);
            }
            return temp;
        }
        #endregion

        #region Printer
        public static Printer3d GetPrinterFromInstance(New3DPrinterViewModel instance, Printer3d printer = null)
        {
            Printer3d temp = printer ?? new Printer3d();
            try
            {
                temp.Id = instance.Id;
                temp.Price = instance.Price;
                temp.Type = instance.Type;
                //temp.Supplier = instance.Supplier;
                temp.Manufacturer = instance.Manufacturer;
                temp.MaterialType = instance.MaterialFamily;
                temp.Model = instance.Model;
                temp.UseFixedMachineHourRating = instance.UseFixedMachineHourRating;
                if (temp.UseFixedMachineHourRating)
                    temp.HourlyMachineRate = new HourlyMachineRate() { FixMachineHourRate = instance.MachineHourRate };
                else
                    temp.HourlyMachineRate = instance.MachineHourRateCalculation;
                temp.BuildVolume = instance.BuildVolume;
                temp.PowerConsumption = instance.PowerConsumption;
                temp.Uri = instance.LinkToReorder;
                temp.Attributes = instance.Attributes.ToList();
                // Not implemented yet
                temp.Maintenances = new ObservableCollection<Maintenance3d>();
                temp.SlicerConfig = new Printer3dSlicerConfig()
                {
                    AMax_e = instance.AMax_e,
                    AMax_xy = instance.AMax_xy,
                    AMax_z = instance.AMax_z,
                    AMax_eExtrude = instance.AMax_eExtrude,
                    AMax_eRetract = instance.AMax_eRetract,
                    PrintDurationCorrection = instance.PrintDurationCorrection,
                };
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.DialogExceptionFormatedContent, exc.Message, exc.TargetSite);
            }
            return temp;
        }
        #endregion

        #region Workstep
        public static Workstep GetWorkstepFromInstance(NewWorkstepViewModel instance, Workstep workstep = null)
        {
            Workstep temp = workstep ?? new Workstep();
            try
            {
                temp.Id = instance.Id;
                temp.Name = instance.Name;
                temp.Price = instance.Amount;
                temp.CalculationType = instance.CalculationType;
                temp.Type = instance.Type;
                temp.Category = instance.Category;
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.DialogExceptionFormatedContent, exc.Message, exc.TargetSite);
            }
            return temp;
        }
        #endregion
    }
}
