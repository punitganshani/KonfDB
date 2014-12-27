#region License and Product Information

// 
//     This file 'CommandArgsTest.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KonfDB.Tests.Utilities
{
    [TestClass]
    public class CommandArgsTest
    {
        private const string StringWithQuotes = "\"Punit Ganshani\"";

        [TestMethod]
        public void TestSplitOnStartupArgs()
        {
            CommandArgs args = new CommandArgs("-console -port:8080 -dbconfig:dbproviders.json -dbprovider:mydb");
            Assert.AreEqual("console", args[0]);
            Assert.AreEqual("8080", args["port"]);
            Assert.AreEqual("dbproviders.json", args["dbconfig"]);
            Assert.AreEqual("mydb", args["dbprovider"]);
        }

        [TestMethod]
        public void TestSplitOnCreateSuiteWithQuotes()
        {
            CommandArgs args = new CommandArgs(String.Format("NewSuite /name={0}", StringWithQuotes));
            StringAssert.Contains(StringWithQuotes, args["name"]);
        }

        [TestMethod]
        public void TestSplitOnCreateSuite()
        {
            CommandArgs args = new CommandArgs("NewSuite /name=Suite");
            Assert.AreEqual("Suite", args["name"]);
        }

        [TestMethod]
        public void TestSplitOnGetSuite()
        {
            CommandArgs args = new CommandArgs("GetSuite /name=Suite");
            Assert.AreEqual("Suite", args["name"]);
        }

        [TestMethod]
        public void TestSplitOnGetSuiteWithQuotes()
        {
            CommandArgs args = new CommandArgs(String.Format("GetSuite /name={0}", StringWithQuotes));
            Assert.AreEqual(StringWithQuotes, args["name"]);
        }

        [TestMethod]
        public void TestSplitOnGetSuiteWithQuotesDifferentSeparator()
        {
            CommandArgs args = new CommandArgs(String.Format("GetSuite /name:{0}", StringWithQuotes));
            Assert.AreEqual(StringWithQuotes, args["name"]);
        }

        [TestMethod]
        public void TestAzureUserConnectionString()
        {
            var args = new CommandArgs("-username=myuser -password=pwd");
            Assert.AreEqual("myuser", args["username"]);
            Assert.AreEqual("pwd", args["password"]);
        }

        [TestMethod]
        public void TestAzureDatabaseConnectionString()
        {
            var args =
                new CommandArgs(
                    "-providerType=AzureSqlProvider -host=tcp:host.database.windows.net -port=1433 -instanceName=konfdb -username=userid@host -password=dBPassword");

            Assert.AreEqual("AzureSqlProvider", args["providerType"]);
            Assert.AreEqual("tcp:host.database.windows.net", args["host"]);
            Assert.AreEqual("1433", args["port"]);
            Assert.AreEqual("konfdb", args["instanceName"]);
            Assert.AreEqual("userid@host", args["username"]);
            Assert.AreEqual("dBPassword", args["password"]);
        }
    }
}