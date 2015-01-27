#region License and Product Information

// 
//     This file 'WcfClient.cs' is part of KonfDB application - 
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
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using KonfDB.Infrastructure.Common;
using KonfDB.Infrastructure.Enums;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Infrastructure.WCF
{
    public sealed class WcfClient<T>
    {
        private T _contract;

        public string ServerName
        {
            get { return _addressInfo.ServerName; }
        }

        public string Port
        {
            get { return _addressInfo.Port; }
        }

        public string ServiceName
        {
            get { return _addressInfo.ServiceName; }
        }

        public ServiceType Type
        {
            get { return _addressInfo.Type; }
        }

        public T Contract
        {
            get { return _contract; }
            private set
            {
                _contract = value;
                Channel = ((ICommunicationObject)value);
                if (Channel != null)
                {
                    Channel.Faulted += Channel_Faulted;
                    Channel.Closing += Channel_Closing;
                    Channel.Closed += Channel_Closed;
                }
            }
        }

        private void Channel_Closing(object sender, EventArgs e)
        {
            CurrentContext.Default.Log.Debug("Channel Closing");
        }

        private void Channel_Closed(object sender, EventArgs e)
        {
            CurrentContext.Default.Log.Debug("Channel Closed");
        }

        public CommunicationState State
        {
            get { return Channel.State; }
        }

        public ICommunicationObject Channel { get; private set; }
        public event EventHandler<DataEventArgs<WcfClient<T>>> OnFaulted;


        private void Channel_Faulted(object sender, EventArgs e)
        {
            CurrentContext.Default.Log.Error("Channel is faulted, event raised for re-creation.");
            if (OnFaulted != null)
                OnFaulted(sender, new DataEventArgs<WcfClient<T>>(this));
        }

        private readonly AddressInfo _addressInfo;

        private WcfClient(AddressInfo addressInfo)
        {
            Contract = addressInfo.CreateChannel<T>();
            _addressInfo = addressInfo;
        }

        public static WcfClient<T> Create(ServiceType type, string serverName,
                                            string port, string serviceName, string folder = "api",
                                            ServiceSecurityMode mode = ServiceSecurityMode.None)
        {
            var address = new AddressInfo(type, serverName, port, serviceName, folder, mode);
            return new WcfClient<T>(address);
        }
    }
}