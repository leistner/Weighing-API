using Hbm.Weighing.Api;
using Hbm.Weighing.Api.WTX.Jet;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hbm.Weighing.Api.Data
{
    class DataScaleInfo
    {
        #region =============== constructors & destructors =================
        public DataScaleInfo(INetConnection Connection)
        {
        }
        #endregion

        #region ==================== events & delegates ====================
        /// <summary>
        /// Updates & converts the values from buffer
        /// </summary>
        /// <param name="sender">Connection class</param>
        /// <param name="e">EventArgs, Event argument</param>
        public void UpdateFillerExtendedData(object sender, EventArgs e)
        {
        }
        #endregion

        #region ======================== properties ========================
        int ScaleSupplyNominalVoltage { get; }
        int ScaleSupplyMinimumVoltage { get; }
        int ScaleSupplyMaximumVoltage { get; }
        int ScaleAccuracyClass { get; set; }
        int ScaleMinimumDeadLoad { get; }  
        int ScaleMaximumNumberVerificationInterval { get; }
        int ScaleApportionmentFactor { get; }
        int ScaleSafeLoadLimit { get; }
        int ScaleOperationNominalTemperature { get; }
        int ScaleOperationMinimumTemperature { get; }
        int ScaleOperationMaximumTemperature { get; }
        int ScaleRelativeMinimumLoadCellVerficationInterval { get; }
        int ImplementedProfileSpecification { get; }
        int LcCapability { get; }
        int Alarms { get; }
        string OimlCertificationInformation { get; }
        string NtepCertificationInformation { get; }
        #endregion
    }
}
