#region License and Product Information

// 
//     This file 'Extensions.cs' is part of KonfDB application - 
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

using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using KonfDB.Infrastructure.Shell;

public static class Extensions
{
    public static void RunService(this ServiceBase[] services, params string[] args)
    {
        services.ToList().ForEach(service =>
        {
            CurrentContext.Default.Log.Info("Starting Service: " + service.GetType().Name);

            var onstart = service.GetType().GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);
            onstart.Invoke(service, new object[] {args});

            CurrentContext.Default.Log.Info("Service started: " + service.GetType().Name);
        });
    }

    public static void StopService(this ServiceBase[] services, params string[] args)
    {
        services.ToList().ForEach(service =>
        {
            CurrentContext.Default.Log.Info("Stopping Service: " + service.GetType().Name);

            var onstop = service.GetType().GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);
            onstop.Invoke(service, null);

            CurrentContext.Default.Log.Info("Service stopped: " + service.GetType().Name);
        });
    }
}