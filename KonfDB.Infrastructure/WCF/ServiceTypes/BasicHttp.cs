#region License and Product Information

// 
//     This file 'BasicHttp.cs' is part of KonfDB application - 
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

namespace KonfDB.Infrastructure.WCF.ServiceTypes
{
    public class BasicHttp : BasicHttpBinding
    {
        public BasicHttp()
        {
            this.MaxReceivedMessageSize = 5242880; //int.MaxValue;//655360;

            this.MaxBufferPoolSize = 5242880; //int.MaxValue;//524288;
            this.MaxBufferSize = 5242880; //int.MaxValue;
            this.ReceiveTimeout = TimeSpan.MaxValue;
            this.SendTimeout = TimeSpan.MaxValue;
            this.CloseTimeout = TimeSpan.MaxValue;
            this.OpenTimeout = TimeSpan.MaxValue;

            //// default time outs
            //this.CloseTimeout = GetfromConfig("CloseTimeout", new TimeSpan(0, 1, 0));
            //this.ReceiveTimeout = GetfromConfig("ReceiveTimeout", new TimeSpan(0, 10, 0));
            //this.SendTimeout = GetfromConfig("SendTimeout", new TimeSpan(0, 10, 0));
        }

        //private TimeSpan GetfromConfig(string key, TimeSpan defaultValue)
        //{
        //    TimeSpan returnValue;

        //    if (string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
        //        return defaultValue;
        //    else
        //    {
        //        try
        //        {
        //            return if(TimeSpan.TryParse(ConfigurationManager.AppSettings[key]);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Error(key + " is not in valid format'00:00:00'");
        //            throw ex;
        //        }
        //    }
        //}

        //private long GetfromConfig(string key, long defaultValue)
        //{
        //    if (string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
        //        return defaultValue;
        //    else
        //        return Convert.ToInt64(ConfigurationManager.AppSettings[key]);
        //}
    }
}