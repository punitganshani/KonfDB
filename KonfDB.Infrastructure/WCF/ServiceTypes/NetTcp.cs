#region License and Product Information

// 
//     This file 'NetTcp.cs' is part of KonfDB application - 
//     a project perceived and developed by Punit Ganshani.
// 
//     KonfDB is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     KonfDB is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with KonfDB.  If not, see <http://www.gnu.org/licenses/>.
// 
//     You can also view the documentation and progress of this project 'KonfDB'
//     on the project website, <http://www.konfdb.com> or on 
//     <http://www.ganshani.com/applications/konfdb>

#endregion

using System;
using System.ServiceModel;
using System.Xml;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Infrastructure.WCF.ServiceTypes
{
    public class NetTcp : NetTcpBinding
    {
        private readonly TimeSpan INFINITE_TIME = TimeSpan.MaxValue;

        public NetTcp()
        {
            CurrentContext.Default.Log.SvcInfo(
                "NetTcp is used on Port-Sharing basis. Ensure port sharing is enabled. Execute: sc.exe config NetTcpPortSharing start= demand");
            this.PortSharingEnabled = true;
            CurrentContext.Default.Log.SvcInfo("Setting security mode.");
            //this.Security.Mode = SecurityMode.None;

            // Buffer size
            this.MaxReceivedMessageSize = GetfromConfig("MaxReceivedMessageSize", 6553600);
            this.MaxBufferPoolSize = GetfromConfig("MaxBufferPoolSize", 5242880);

            //// default time outs
            this.CloseTimeout = INFINITE_TIME;
            this.SendTimeout = INFINITE_TIME;
            //this.SendTimeout = GetfromConfig("SendTimeout", new TimeSpan(0, 10, 0)); 

            this.ReceiveTimeout = INFINITE_TIME;
            this.ReaderQuotas = XmlDictionaryReaderQuotas.Max;
        }

        /// <summary>
        ///     TODO: For future extension
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private TimeSpan GetfromConfig(string key, TimeSpan defaultValue)
        {
            return defaultValue;
        }

        /// <summary>
        ///     TODO: For future extension
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private long GetfromConfig(string key, long defaultValue)
        {
            return defaultValue;
        }
    }
}