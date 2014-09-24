#region License and Product Information

// 
//     This file 'ModelToDBConverter.cs' is part of KonfDB application - 
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
using KonfDB.Engine.Database.EntityFramework;
using KonfDB.Infrastructure.Database.Entities.Configuration;
using Environment = KonfDB.Engine.Database.EntityFramework.Environment;

namespace KonfDB.Engine.Database.ObjectMappers
{
    public static class ModelToDBConverter
    {
        public static Suite ToNewDbObject(this SuiteModel suiteModel)
        {
            return new Suite
            {
                IsActive = suiteModel.IsActive,
                PrivateKey = suiteModel.PrivateKey,
                PublicKey = suiteModel.PublicKey,
                SuiteName = suiteModel.SuiteName,
                UsesSysEncryption = suiteModel.UsesSysEncryption,
                SuiteType = EnumConverter.ConvertToInt(suiteModel.SuiteType),
                CreatedDate = DateTime.Now.ToUniversalTime(),
            };
        }

        public static Application ToNewDbObject(this ApplicationModel application)
        {
            return new Application
            {
                ApplicationName = application.ApplicationName,
                Description = application.Description,
                IsActive = application.IsActive,
                SuiteId = application.SuiteId
            };
        }

        public static Region ToNewDbObject(this RegionModel region)
        {
            return new Region
            {
                RegionName = region.RegionName,
                Description = region.Description,
                IsActive = region.IsActive,
                SuiteId = region.SuiteId
            };
        }

        public static Server ToNewDbObject(this ServerModel server)
        {
            return new Server
            {
                ServerName = server.ServerName,
                Description = server.Description,
                IsActive = server.IsActive,
                SuiteId = server.SuiteId
            };
        }

        public static Application UpdateDbObject(this Application dbObject, ApplicationModel application,
            bool recursive = false)
        {
            dbObject.ApplicationName = application.ApplicationName;
            dbObject.Description = application.Description;
            dbObject.IsActive = application.IsActive;
            dbObject.SuiteId = application.SuiteId;

            if (recursive)
            {
                throw new NotImplementedException("TE081701: UpdateDbObject - Application recursive not implemented");
            }

            return dbObject;
        }

        public static Environment ToNewDbObject(this EnvironmentModel environment)
        {
            return new Environment
            {
                EnvironmentName = environment.EnvironmentName,
                EnvironmentTypeId = (int) environment.EnvironmentType,
                IsActive = environment.IsActive,
                SuiteId = environment.SuiteId
            };
        }

        public static Parameter ToNewDbObject(this ParameterModel parameter)
        {
            return new Parameter
            {
                ParameterName = parameter.ParameterName,
                ParameterValue = parameter.ParameterValue,
                IsActive = parameter.IsActive,
                SuiteId = parameter.SuiteId,
                IsEncrypted = parameter.IsEncrypted,
            };
        }

        public static Mapping ToNewDbObject(this MappingModel mapping)
        {
            return new Mapping
            {
                ApplicationId = mapping.ApplicationId,
                EnvironmentId = mapping.EnvironmentId,
                ParameterId = mapping.ParameterId,
                RegionId = mapping.RegionId,
                ServerId = mapping.ServerId,
                SuiteId = mapping.SuiteId,
                MappingId = mapping.MappingId
            };
        }

        public static Audit ToNewDbObject(this AuditRecordModel mapping)
        {
            return new Audit
            {
                ActionAtUtc = DateTime.UtcNow,
                AuditAreaId = (int) mapping.Area,
                AuditIdentity = mapping.Key,
                Message = mapping.Message,
                UserId = (int) mapping.UserId,
                Metadata1 = mapping.Metadata1,
                Metadata2 = mapping.Metadata2,
                Reason = mapping.Reason.ToString()
            };
        }
    }
}