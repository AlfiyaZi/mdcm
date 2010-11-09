// mDCM: A C# DICOM library
//
// Copyright (c) 2006-2008  Colby Dillion
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Contributing Author:
//    Mahesh Dubey (mdubey82@gmail.com)

using System;
using Dicom.Data;
namespace Dicom.Network.Client.NPrint
{
    /// <summary>
    /// C.13.9 Printer Module
    /// Table C.13-9
    /// PRINTER MODULE ATTRIBUTES
    /// </summary>
    public class PrinterStatusIOD
    {
        #region Private Field
        private DcmDataset _dataset = null;
        private DateTime _defaultDateTime = DateTime.MaxValue ;
        #endregion

        #region C'tor
        /// <summary>
        /// Initialize the new instance of <see cref="PrinterStatusIOD"/> class.
        /// </summary>
        /// <param name="dataset"></param>
        public PrinterStatusIOD(DcmDataset dataset)
        {
            _dataset = dataset;
        }
        #endregion

        #region Public Properties
        /// <summary>
        ///Printer device status. Enumerated Values:
        ///NORMAL
        ///WARNING
        ///FAILURE
        /// </summary>
        public PrinterStatus PrinterStatus
        {
            get
            {
                return (PrinterStatus)Enum.Parse(typeof(PrinterStatus), _dataset.GetString(DicomTags.PrinterStatus, "None"), true);

            }

        }

        /// <summary>
        ///Additional information about Printer Status(2110,0010).
        ///Defined Terms when the Printer Status is equal to
        ///NORMAL:
        ///      NORMAL
        ///See Section C.13.9.1 for Defined Terms when the
        ///Printer Status is equal to WARNING or FAILURE
        /// </summary>
        public string PrinterStatusInfo
        {
            get
            {
                return _dataset.GetString(DicomTags.PrinterStatusInfo, string.Empty);
            }

        }

        /// <summary>
        /// User defined name identifying the printer.
        /// </summary>
        public string PrinterName
        {
            get
            {
                return _dataset.GetString(DicomTags.PrinterName, string.Empty);

            }

        }
        /// <summary>
        /// Manufacturer of the printer.
        /// </summary>
        public string Manufacturer
        {
            get
            {
                return _dataset.GetString(DicomTags.Manufacturer, string.Empty);

            }

        }

        /// <summary>
        /// Manufacturer's model number of the printer.
        /// </summary>
        public string ManufacturersModelName
        {
            get
            {
                return _dataset.GetString(DicomTags.ManufacturersModelName, string.Empty);

            }

        }

        /// <summary>
        /// Manufacturer's serial number of the printer.
        /// </summary>
        public string DeviceSerialNumber
        {
            get
            {
                return _dataset.GetString(DicomTags.DeviceSerialNumber, string.Empty);

            }

        }
        /// <summary>
        /// Manufacturer's designation of software version of the printer.
        /// </summary>
        public string SoftwareVersions
        {
            get
            {
                return _dataset.GetString(DicomTags.SoftwareVersions, string.Empty);

            }

        }
        /// <summary>
        /// Date when the printer was last calibrated.
        /// </summary>
        public DateTime? DateOfLastCalibration
        {
            get
            {
                var result = _dataset.GetDateTime(DicomTags.DateOfLastCalibration, 0, _defaultDateTime);
                if (result == _defaultDateTime)
                    return null;
                else
                    return result;

            }
            
        }
        #endregion

        #region Internal Static Method
        /// <summary>
        /// Creates the array of attributes for N-Get request.
        /// </summary>
        /// <returns>Arrays of dicom tags</returns>
        internal static DicomTag[] RequestAttributes()
        {
            DicomTag[] tags = new DicomTag[] { DicomTags.PrinterStatus, DicomTags.PrinterStatusInfo, 
                                               DicomTags.PrinterName, DicomTags.Manufacturer, 
                                               DicomTags.ManufacturersModelName, DicomTags.DeviceSerialNumber,
                                               DicomTags.SoftwareVersions, DicomTags.DateOfLastCalibration, 
                                               DicomTags.TimeOfLastCalibration };
            return tags;
        }
        #endregion

        #region Override Method
        /// <summary>
        /// Returns the result in string format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("DateOfLastCalibration : {0}\nDeviceSerialNumber : {1}\nManufacturer : {2}\nManufacturersModelName : {3}\nPrinterName : {4}\nPrinterStatusInfo : {5}\nSoftwareVersions : {6} ", this.DateOfLastCalibration, this.DeviceSerialNumber, this.Manufacturer, this.ManufacturersModelName, this.PrinterName, this.PrinterStatus, this.PrinterStatusInfo, this.SoftwareVersions);

        }
        #endregion
    }
    
    #region Printer Status Enum
    /// <summary>
    ///Printer device status. Enumerated Values:
    ///None : If the status is empty. 
    ///NORMAL
    ///WARNING
    ///FAILURE
    /// </summary>
    public enum PrinterStatus
    {
        None,

        Normal,

        Warning,

        Failure
    }
    #endregion
}
