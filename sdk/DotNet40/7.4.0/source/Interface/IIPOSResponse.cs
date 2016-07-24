using System;
//using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Paymentech.COM
{

    /// <summary>
    /// 
    /// </summary>
    [Guid("06059E42-748E-4f9b-9DA4-6B62CE5E6ADC")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IIPOSResponse
    {
        /// <summary>
        /// 
        /// </summary>
        int ErrorCode { get; }
        /// <summary>
        /// 
        /// </summary>
        string ErrorString { get; }
        /// <summary>
        /// 
        /// </summary>
        int HTTPError { get; }
        /// <summary>
        /// 
        /// </summary>
        int GatewayError { get; }
        /// <summary>
        /// 
        /// </summary>
        bool Error { get; }
        /// <summary>
        /// 
        /// </summary>
        byte[] Data { get; }
    }
}
