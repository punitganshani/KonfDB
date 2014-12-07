#region License and Product Information

// 
//     This file 'CertificateExtensions.cs' is part of KonfDB application - 
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
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using KonfDB.Infrastructure.Configuration.Interfaces;
using KonfDB.Infrastructure.Exceptions;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Infrastructure.Extensions
{
    public static class CertificateExtensions
    {
        public static X509Certificate2 GetX509Certificate(this ICertificateConfiguration certificate)
        {
            var certStore = new X509Store(certificate.StoreName, certificate.StoreLocation);
            certStore.Open(OpenFlags.ReadOnly);

            var certCollection = certStore.Certificates.Find(certificate.FindBy, certificate.Value, false);
            if (certCollection.Count == 0)
            {
                throw new InvalidConfigurationException(
                    string.Format("Could not find the SSL certicate in the store: {0}, location: {1} and {2}: {3}",
                        certificate.StoreName, certificate.StoreLocation, certificate.FindBy, certificate.Value));
            }

            certStore.Close();

            CurrentContext.Default.Log.Debug("Using Certificate: " + certCollection[0].Thumbprint);

            return certCollection[0];
        }

        public static void BindCertificateToPort(this ICertificateConfiguration certificate, string port)
        {
            if (certificate == null) return;

            var storeCertificate = certificate.GetX509Certificate();

            const string netshGrantAccess = "http add urlacl url=https://+:{0}/ user=EVERYONE";
            const string netshAddCertificate =
                "http add sslcert ipport=0.0.0.0:{0} certhash={1} appid={{{2}}} clientcertnegotiation=enable";

            var grantAccessProcess = new Process
            {
                StartInfo =
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "netsh.exe"),
                    Arguments = string.Format(netshGrantAccess, port)
                }
            };

            var bindPortToCertificateProcess = new Process
            {
                StartInfo =
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "netsh.exe"),
                    Arguments = string.Format(netshAddCertificate, port, storeCertificate.Thumbprint, Guid.NewGuid())
                }
            };

            grantAccessProcess.StartInfo.UseShellExecute = false;
            grantAccessProcess.StartInfo.RedirectStandardOutput = true;
            bindPortToCertificateProcess.StartInfo.UseShellExecute = false;
            bindPortToCertificateProcess.StartInfo.RedirectStandardOutput = true;

            grantAccessProcess.Start();
            CurrentContext.Default.Log.Debug("netsh " + grantAccessProcess.StartInfo.Arguments + " >> " +
                                             grantAccessProcess.StandardOutput.ReadToEnd());
            grantAccessProcess.WaitForExit();


            bindPortToCertificateProcess.Start();
            CurrentContext.Default.Log.Debug("netsh " + bindPortToCertificateProcess.StartInfo.Arguments + " >> " +
                                             bindPortToCertificateProcess.StandardOutput.ReadToEnd());
            bindPortToCertificateProcess.WaitForExit();
        }

        public static void BindCertificateToPort(this X509Certificate2 storeCertificate, string port)
        {
            if (storeCertificate == null) return;
            const string netsh =
                "http add sslcert ipport=0.0.0.0:{0} certhash={1} appid={{{2}}} clientcertnegotiation=enable";
            // netsh http add sslcert ipport=0.0.0.0:<port> certhash={<thumbprint>} appid={<some GUID>}
            var bindPortToCertificate = new Process
            {
                StartInfo =
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "netsh.exe"),
                    Arguments = string.Format(netsh, port, storeCertificate.Thumbprint, Guid.NewGuid())
                }
            };

            bindPortToCertificate.StartInfo.UseShellExecute = false;
            bindPortToCertificate.StartInfo.RedirectStandardOutput = true;

            bindPortToCertificate.Start();
            string output = bindPortToCertificate.StandardOutput.ReadToEnd();
            CurrentContext.Default.Log.Debug("Executed : netsh " + bindPortToCertificate.StartInfo.Arguments +
                                             " with output: " +
                                             output);
            bindPortToCertificate.WaitForExit();
        }
    }
}