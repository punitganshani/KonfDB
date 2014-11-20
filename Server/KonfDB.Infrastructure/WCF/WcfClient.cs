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
        private readonly string _serverName;

        public string ServerName
        {
            get { return _serverName; }
        }

        private readonly string _port;

        public string Port
        {
            get { return _port; }
        }

        private readonly string _serviceName;

        public string ServiceName
        {
            get { return _serviceName; }
        }


        private readonly ServiceType _type;

        public ServiceType Type
        {
            get { return _type; }
        }

        public T Contract
        {
            get { return _contract; }
            private set
            {
                _contract = value;
                Channel = ((ICommunicationObject) value);
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

        private WcfClient(T contract, ServiceType type, string serverName, string port, string serviceName)
        {
            _type = type;
            _serverName = serverName;
            _port = port;
            _serviceName = serviceName;

            Contract = contract;
        }

        public static WcfClient<T> Create(ServiceType type, string serverName,
            string port, string serviceName, ServiceSecurityMode mode = ServiceSecurityMode.None)
        {
            var address = new AddressInfo(type, serverName, port, serviceName, mode);
            var obj = address.CreateChannel<T>();
            return new WcfClient<T>(obj, type, serverName, port, serviceName);
        }

        public static List<WcfClient<T>> Create(List<AddressInfo> serviceInfo)
        {
            var channels = new List<WcfClient<T>>();
            Parallel.For(0, serviceInfo.Count, i =>
            {
                try
                {
                    var obj = serviceInfo[i].CreateChannel<T>();
                    channels.Add(new WcfClient<T>(obj, serviceInfo[i].Type, serviceInfo[i].ServerName,
                        serviceInfo[i].Port, serviceInfo[i].ServiceName));
                }
                catch (Exception ex)
                {
                    CurrentContext.Default.Log.SvcInfo("An exception occured", ex);
                    throw;
                }
            });

            return channels;
        }
    }
}