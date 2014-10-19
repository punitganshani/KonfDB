#region License and Product Information

// 
//     This file 'DbToModelConverter.cs' is part of KonfDB application - 
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

/*
 * LINQ to Entities does not recognize the method --> Use AsEnumerable() on collection of DB data object
 * 
 * 
 */
using System.Collections.Generic;
using System.Linq;
using KonfDB.Engine.Database.EntityFramework;
using KonfDB.Infrastructure.Database.Entities.Configuration;
using EnvironmentType = KonfDB.Infrastructure.Database.Enums.EnvironmentType;


namespace KonfDB.Engine.Database.ObjectMappers
{
    public static class DbToModelConverter
    {
        public static ApplicationModel ToModel(this Application input)
        {
            return new ApplicationModel
            {
                ApplicationId = input.ApplicationId,
                ApplicationName = input.ApplicationName,
                IsActive = input.IsActive,
                SuiteId = input.SuiteId,
                Description = input.Description,
                Mappings = input.Mappings.ToList().ToModel(),
            };
        }

        public static List<ApplicationModel> ToModel(this ICollection<Application> input)
        {
            return input.AsEnumerable().Select(applicationModel => applicationModel.ToModel()).ToList();
        }

        public static EnvironmentModel ToModel(this Environment input)
        {
            return new EnvironmentModel
            {
                EnvironmentId = input.EnvironmentId,
                EnvironmentName = input.EnvironmentName,
                EnvironmentType = EnumConverter.Convert<EnvironmentType>(input.EnvironmentTypeId),
                IsActive = input.IsActive,
                SuiteId = input.SuiteId
            };
        }

        public static List<EnvironmentModel> ToModel(this ICollection<Environment> input)
        {
            return input.AsEnumerable().Select(applicationModel => applicationModel.ToModel()).ToList();
        }

        //internal SuiteModel ToModel(this Suite suite)
        //{
        //    return new SuiteModel()
        //        {
        //            Applications = suite.Applications.ToModel(),
        //            Environments = suite.Environments.ToModel(),
        //            SuiteId = 
        //        };
        //}

        public static RegionModel ToModel(this Region input)
        {
            return new RegionModel
            {
                RegionId = input.RegionId,
                RegionName = input.RegionName,
                IsActive = input.IsActive,
                SuiteId = input.SuiteId,
                Description = input.Description,
                Mappings = input.Mappings.ToList().ToModel()
            };
        }

        public static List<RegionModel> ToModel(this ICollection<Region> input)
        {
            return input.AsEnumerable().Select(RegionModel => RegionModel.ToModel()).ToList();
        }

        public static ServerModel ToModel(this Server input)
        {
            return new ServerModel
            {
                ServerId = input.ServerId,
                ServerName = input.ServerName,
                IsActive = input.IsActive,
                SuiteId = input.SuiteId,
                Description = input.Description,
                Mappings = input.Mappings.ToList().ToModel()
            };
        }

        public static List<ServerModel> ToModel(this ICollection<Server> input)
        {
            return input.AsEnumerable().Select(serverModel => serverModel.ToModel()).ToList();
        }

        public static ParameterModel ToModel(this Parameter input)
        {
            return new ParameterModel
            {
                ParameterId = input.ParameterId,
                ParameterName = input.ParameterName,
                IsActive = input.IsActive,
                SuiteId = input.SuiteId,
                ParameterValue = input.ParameterValue,
                IsEncrypted = input.IsEncrypted,
                Mappings = input.Mappings.ToList().ToModel()
            };
        }

        public static List<ParameterModel> ToModel(this ICollection<Parameter> input)
        {
            return input.AsEnumerable().Select(parameterModel => parameterModel.ToModel()).ToList();
        }

        //public static ParameterModel[] ToModel(this IEnumerable<Parameter> input)
        //{
        //    var parameters = input as Parameter[] ?? input.ToArray();
        //    if (parameters.Any())
        //        return parameters.Select(parameterModel => parameterModel.ToModel()).ToArray();
        //    else
        //        return new ParameterModel[0];
        //}

        public static MappingModel ToModel(this Mapping mapping)
        {
            return new MappingModel
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

        public static List<MappingModel> ToModel(this List<Mapping> input)
        {
            return input.AsEnumerable().Select(model => model.ToModel()).ToList();
        }

        public static SuiteUserModel ToModel(this SuiteUser input)
        {
            return new SuiteUserModel
            {
                SuiteId = input.SuiteId,
                Role = input.Role.RoleName,
                Username = input.User.UserName,
            };
        }

        public static List<SuiteUserModel> ToModel(this ICollection<SuiteUser> input)
        {
            return input.AsEnumerable().Select(model => model.ToModel()).ToList();
        }
    }
}