#region License and Product Information

// 
//     This file 'ConfigReader.cs' is part of KonfDB application - 
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
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Infrastructure.Common
{
    public class ConfigReader
    {
        private static string applicationName;
        private static string parentDirectory;
        private static string logDirectory;

        public static string GetApplicationName()
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;

                //application name
                if (!string.IsNullOrEmpty(appSettings["ApplicationName"]))
                {
                    applicationName = appSettings["ApplicationName"];
                }

                if (string.IsNullOrEmpty(applicationName))
                {
                    // Get the name of the current executing application
                    AppDomain appDomain = AppDomain.CurrentDomain;
                    string fullName = appDomain.FriendlyName;
                    applicationName = fullName;

                    int lastDot = fullName.LastIndexOf(".", StringComparison.Ordinal);
                    if (lastDot != -1)
                        applicationName = fullName.Substring(0, lastDot);
                }
            }
            return applicationName;
        }

        public static string GetApplicationPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string GetParentConfigDirectory()
        {
            if (String.IsNullOrEmpty(parentDirectory))
            {
                string Path = GetApplicationPath();

                CurrentContext.Default.Log.Info(Path);

                DirectoryInfo dir = new DirectoryInfo(Path);

                if (dir.Exists)
                {
                    parentDirectory = dir.Parent.FullName + "\\Config\\";
                }
            }
            return parentDirectory;
        }

        public static string GetConfigInfo(string FileName)
        {
            if (File.Exists(parentDirectory))
            {
                CurrentContext.Default.Log.Debug("Will read file from : " + parentDirectory + FileName);
                // read in the Xml from the config file
                StringBuilder configFileContents;
                using (StreamReader reader = new StreamReader(parentDirectory + FileName))
                {
                    string xmlSnippet;
                    configFileContents = new StringBuilder(512);
                    while ((xmlSnippet = reader.ReadLine()) != null)
                    {
                        configFileContents.Append(xmlSnippet);
                    }
                }

                return configFileContents.ToString();
            }
            throw new FileNotFoundException("Config File Not Found");
        }

        public static string GetLogPath()
        {
            if (string.IsNullOrEmpty(logDirectory))
            {
                string logPath = string.Empty;
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                //location to store log files
                logPath = !string.IsNullOrEmpty(appSettings["LogPath"]) ? appSettings["LogPath"] : "C:\\logs\\";
                string appName = GetApplicationName();
                Directory.CreateDirectory(logPath + appName);
                return logPath + appName;
            }
            return logDirectory;
        }
    }
}