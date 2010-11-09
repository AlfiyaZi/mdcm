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
using System.Runtime.Remoting.Messaging;
using Dicom.Data;
namespace Dicom.Network.Client.NPrint
{
    public delegate void PrinterStatusErrorCallback(Exception ex);

    /// <summary>
    /// Scu for getting printer status.
    /// </summary>
    /// <example>
    /// <para>
    /// <code>
    ///  PrinterStatusClient printerStatusSCU = new PrinterStatusClient()
    ///  printerStatusSCU.BeginGetPrinterStatus("localhost", 7104, DcmSocketType.TCP, new AsyncCallback(EndGetPrinterStatus), printerStatusSCU);
    ///  private void EndGetPrinterStatus(IAsyncResult result)
    ///   {
    ///     PrinterStatusClient printerStatusSCU = (PrinterStatusClient)result.AsyncState;
    ///     var printerStatusResult = printerStatusSCU.EndGetPrinterStatus(result);
    ///     if (printerStatusResult != null)
    ///     {
    ///        MessageBox.Show(printerStatusResult.ToString(), "Printer Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
    ///     }
    ///  }
    /// </code>
    /// </para>
    /// </example>
    public class PrinterStatusClient :DcmClientBase
    {
        #region Private Delegate,Field,Property
        private delegate PrinterStatusIOD GetPrinterStatusCallback(string host, int port, DcmSocketType socketType);
        private DcmDataset _results;

        private PrinterStatusIOD PrinterStatusResult
        {
            get
            {
                if (_results != null)
                {
                    return new PrinterStatusIOD(_results);
                }
                return null;
            }
        }
        #endregion

        #region Public C'tor, Event
        public PrinterStatusErrorCallback OnPrinterStatusError;

        /// <summary>
        /// Initialize the new instance of <see cref="PrinterStatusClient"/> class.
        /// </summary>
        public PrinterStatusClient()
            : base()
        {
            LogID = "PrinterStatusScu";
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Established asynchronous connection with peer entity.
        /// </summary>
        /// <param name="host">IP address of peer entity</param>
        /// <param name="port">Port of the peer entity</param>
        /// <param name="socketType"></param>
        /// <param name="callback"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public IAsyncResult BeginGetPrinterStatus(string host, int port, DcmSocketType socketType, AsyncCallback callback, object asyncState)
        {

            GetPrinterStatusCallback getPrinterStatusDelegate = new GetPrinterStatusCallback(this.GetPrinterStatusAsync);
            return getPrinterStatusDelegate.BeginInvoke(host, port, socketType, callback, asyncState);

        }
        /// <summary>
        /// End the asynchronous operation and retrive the result.
        /// </summary>
        /// <param name="ar"></param>
        /// <returns>The status of the printer</returns>
        public PrinterStatusIOD EndGetPrinterStatus(IAsyncResult ar)
        {
            GetPrinterStatusCallback getPrinterStatusDelegate = ((AsyncResult)ar).AsyncDelegate as GetPrinterStatusCallback;
            if (getPrinterStatusDelegate != null)
            {
                return getPrinterStatusDelegate.EndInvoke(ar) as PrinterStatusIOD;

            }
            else
                throw new InvalidOperationException("Asynchresult is null");
        }
        #endregion

        #region Private Method
        private PrinterStatusIOD GetPrinterStatusAsync(string host, int port, DcmSocketType socketType)
        {
            this._results = null;
            Connect(host, port, socketType);
            if (!base.Wait())
            {
                if (OnPrinterStatusError != null)
                {
                    OnPrinterStatusError(new Exception(ErrorMessage));
                }
                Close();
            }

            return PrinterStatusResult;

        }
        /// <summary>
        /// Send the N-Get request to the print SCP asking for its status. 
        /// </summary>
        /// <param name="association"></param>
        private void SendRequest()
        {
            byte pcid = Associate.FindAbstractSyntax(DicomUID.PrinterSOPClass);
            var requestedSOPClass = Associate.GetAbstractSyntax(pcid);
            var requstedSOPInstance = DicomUID.PrinterSOPInstance;
            DicomTag[] attributes = PrinterStatusIOD.RequestAttributes();
            SendNGetRequest(pcid, NextMessageID(), requestedSOPClass, requstedSOPInstance, attributes);

        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Send the association request to the peer entity.
        /// </summary>
        protected override void OnConnected()
        {

            DcmAssociate associate = new DcmAssociate()
            {
                CalledAE = this.CalledAE,
                CallingAE = this.CallingAE,
                MaximumPduLength = this.MaxPduSize,

            };

            byte pcid = associate.AddPresentationContext(DicomUID.PrinterSOPClass);
            associate.AddTransferSyntax(pcid, DicomTransferSyntax.ExplicitVRLittleEndian);
            associate.AddTransferSyntax(pcid, DicomTransferSyntax.ImplicitVRLittleEndian);
            SendAssociateRequest(associate);
        }

        protected override void OnReceiveAssociateAccept(DcmAssociate association)
        {

            byte pcid = association.FindAbstractSyntax(DicomUID.PrinterSOPClass);
            if (association.GetPresentationContextResult(pcid) == DcmPresContextResult.Accept)
            {
                SendRequest();
            }
            else//Is it necessary ??
            {
                var msg = Dicom.Utility.Format.AddSpaces(association.GetPresentationContextResult(pcid).ToString());
                OnPrinterStatusError(new Exception(msg));
                Close();
            }
        }

        protected override void OnReceiveNGetResponse(byte presentationID, ushort messageIdRespondedTo, DicomUID affectedClass, DicomUID affectedInstance, DcmDataset dataset, DcmStatus status)
        {
            this._results = dataset;
            SendReleaseRequest();

        }
        #endregion
    }
}
