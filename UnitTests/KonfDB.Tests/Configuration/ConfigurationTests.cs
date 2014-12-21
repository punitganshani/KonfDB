#region License and Product Information

// 
//     This file 'ConfigurationTests.cs' is part of KonfDB application - 
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

using System.Security.Cryptography.X509Certificates;
using KonfDB.Infrastructure.Configuration;
using KonfDB.Infrastructure.Configuration.Interfaces;
using KonfDB.Infrastructure.Configuration.Providers.Certificate;
using KonfDB.Infrastructure.Configuration.Providers.Database;
using KonfDB.Infrastructure.Configuration.Providers.Types;
using KonfDB.Infrastructure.Configuration.Runtime;
using KonfDB.Infrastructure.Enums;
using KonfDB.Infrastructure.Extensions;
using KonfDBCF.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KonfDB.Tests.Configuration
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void TestJsonCreationForHost()
        {
            var config = new HostConfig
            {
                Runtime =
                {
                    Audit = true,
                    LogInfo = new LogElement
                    {
                        ProviderType = "KonfDB.Infrastructure.Logging.Logger, KonfDBC",
                        Parameters = @"-path:konfdb\log.txt"
                    },
                    ServiceSecurity = ServiceSecurityMode.None
                }
            };
            config.Runtime.Server.Add(new ServiceTypeConfiguration { Port = 8885, Type = EndPointType.TCP });
            config.Runtime.Server.Add(new ServiceTypeConfiguration { Port = 8880, Type = EndPointType.HTTP });
            config.Runtime.Server.Add(new ServiceTypeConfiguration { Port = 8890, Type = EndPointType.WSHTTP });
            config.Runtime.Server.Add(new ServiceTypeConfiguration { Port = 8882, Type = EndPointType.REST });
            config.Runtime.SuperUser.Username = "suser";
            config.Runtime.SuperUser.Password = "spwd";

            config.Caching.Enabled = false;
            config.Caching.DurationInSeconds = 30;
            config.Caching.Mode = CacheMode.Absolute;

            config.Providers.Certificate.DefaultKey = "testCert";
            config.Providers.Certificate.Certificates.Add(new CertificateProviderConfiguration
            {
                CertificateKey = "testCert",
                StoreLocation = StoreLocation.LocalMachine,
                StoreName = StoreName.My,
                FindBy = X509FindType.FindBySubjectName,
                Value = "localhost"
            });

            config.Providers.Database.DefaultKey = "localsql";
            config.Providers.Database.Databases.Add(new DatabaseProviderConfiguration
            {
                Key = "localsql",
                Host = @"localhost\sqlexpress",
                Port = 8080,
                InstanceName = "konf",
                Username = "sa",
                Password = "SomePwdStrong*0889",
                Location = @"c:\temp",
                ProviderType = "MsSql"
            });
            config.Providers.Database.Databases.Add(new DatabaseProviderConfiguration
            {
                Key = "azure",
                Host = @"tcp:lbxcft14aq.database.windows.net",
                Port = 1433,
                InstanceName = "dbName",
                Username = "dbName@lbxcft14aq",
                Password = "dBPassword",
                ProviderType = "AzureSql"
            });

            config.Providers.Types.Add(new ProviderTypeConfiguration
            {
                AssemblyPath = "KonfDB.Engine.Database.Providers.MsSql.MsSqlProvider, KonfDBE",
                Type = "MsSql"
            });
            config.Providers.Types.Add(new ProviderTypeConfiguration
            {
                AssemblyPath = "KonfDB.Engine.Database.Providers.AzureSql.AzureSqlProvider, KonfDBE",
                Type = "AzureSql"
            });

            var configJson = config.ToJson();
            var readBack = configJson.FromJsonToObject<HostConfig>();
            Assert.IsNotNull(readBack);
        }

        [TestMethod]
        public void TestJsonCreationForClient()
        {
            var config = new ClientConfig
            {
                Runtime =
                {
                    LogInfo = new LogElement
                    {
                        ProviderType = "KonfDB.Infrastructure.Logging.Logger, KonfDBC",
                        Parameters = @"-path:konfdb\log.txt"
                    },
                }
            };
            config.Runtime.Client.Host = "localhost";
            config.Runtime.Client.Port = 8085;
            config.Runtime.Client.Type = EndPointType.TCP;
            config.Runtime.User.Username = "suser";
            config.Runtime.User.Password = "spwd";

            config.Caching.Enabled = false;
            config.Caching.DurationInSeconds = 30;
            config.Caching.Mode = CacheMode.Absolute;


            var configJson = config.ToJson();
            var readBack = configJson.FromJsonToObject<ClientConfig>();
            Assert.IsNotNull(readBack);
        }
    }
}