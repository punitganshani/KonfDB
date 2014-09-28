#region License and Product Information

// 
//     This file 'ConfigurationDataStore.cs' is part of KonfDB application - 
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
using System.Linq;
using KonfDB.Engine.Database.EntityFramework;
using KonfDB.Engine.Database.ObjectMappers;
using KonfDB.Infrastructure.Database.Abstracts;
using KonfDB.Infrastructure.Database.Entities.Account;
using KonfDB.Infrastructure.Database.Entities.Configuration;
using KonfDB.Infrastructure.Database.Enums;
using KonfDB.Infrastructure.Encryption;
using KonfDB.Infrastructure.Exceptions;
using KonfDB.Infrastructure.Extensions;
using Environment = KonfDB.Engine.Database.EntityFramework.Environment;

namespace KonfDB.Engine.Database.Stores
{
    public class ConfigurationDataStore : IConfigurationDataStore
    {
        private readonly string _connectionString;

        public ConfigurationDataStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Suite

        public List<SuiteModel> GetSuites(long userId)
        {
            var output = new List<SuiteModel>();
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                // get suites-id of a user
                var userSuitesInDB = unitOfWork.Context.SuiteUsers.Where(x => x.UserId == userId).Select(x => x.SuiteId);
                // get suites for each suite-id
                var suitesInDB = unitOfWork.Context.Suites.Where(x => userSuitesInDB.Contains(x.SuiteId));

                var suites = suitesInDB.ToList();

                output.AddRange(suites.ToList().Select(suite => new SuiteModel
                {
                    IsActive = suite.IsActive,
                    PrivateKey = suite.PrivateKey,
                    PublicKey = suite.PublicKey,
                    SuiteId = suite.SuiteId,
                    SuiteName = suite.SuiteName,
                    UsesSysEncryption = suite.UsesSysEncryption,
                    Applications = suite.Applications.ToModel(),
                    Environments = suite.Environments.ToModel(),
                    Regions = suite.Regions.ToModel(),
                    Servers = suite.Servers.ToModel(),
                    Users = suite.SuiteUsers.ToModel()
                }));
            }

            return output;
        }

        public SuiteModel AddSuite(SuiteCreateModel suiteModel)
        {
            Suite suiteReturn;
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                if (unitOfWork.Context.Suites.Any(x => x.SuiteName == suiteModel.SuiteName))
                    throw new InvalidOperationException("Suite already exists with name:" + suiteModel.SuiteName);

                var suiteDB = suiteModel.ToNewDbObject();
                unitOfWork.Add(suiteDB);

                suiteReturn = suiteDB;

                //if (unitOfWork.Context.SuiteUsers.Any(x => x.UserId == suiteModel.UserId && x.SuiteId == suiteDB.SuiteId)) return;

                //var userMapping = new SuiteUser()
                //{
                //    SuiteId = suiteDB.SuiteId,
                //    UserId = suiteModel.UserId
                //};

                //unitOfWork.Add(userMapping);

                foreach (var environment in suiteModel.Environments)
                {
                    environment.SuiteId = suiteDB.SuiteId;
                    environment.IsActive = true;

                    unitOfWork.Add(environment.ToNewDbObject());
                }

                foreach (var application in suiteModel.Applications)
                {
                    application.SuiteId = suiteDB.SuiteId;
                    application.IsActive = true;

                    unitOfWork.Add(application.ToNewDbObject());
                }

                foreach (var region in suiteModel.Regions)
                {
                    region.SuiteId = suiteDB.SuiteId;
                    region.IsActive = true;

                    unitOfWork.Add(region.ToNewDbObject());
                }

                foreach (var server in suiteModel.Servers)
                {
                    server.SuiteId = suiteDB.SuiteId;
                    server.IsActive = true;

                    unitOfWork.Add(server.ToNewDbObject());
                }

                unitOfWork.Add(new SuiteUser
                {
                    SuiteId = suiteReturn.SuiteId,
                    UserId = (int)suiteModel.UserId,
                    RoleId = (int)RoleType.Admin
                });
            }

            return GetSuite(suiteModel.UserId, suiteReturn.SuiteId);
        }

        public SuiteModel GetSuite(long loggedInUserId, string suiteName)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                // get suites-id of a user
                bool userAuthorized = UserAuthorizedToAccessSuite(unitOfWork, loggedInUserId, suiteName, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorized)
                    throw new UnauthorizedUserException("User does not have enough privileges to perform this action");

                // get suite from suite-id
                var suitesInDB = unitOfWork.Context.Suites.FirstOrDefault(x => x.SuiteName == suiteName);

                if (suitesInDB != null)
                {
                    var suiteModels = new SuiteModel
                    {
                        IsActive = suitesInDB.IsActive,
                        PrivateKey = suitesInDB.PrivateKey,
                        PublicKey = suitesInDB.PublicKey,
                        SuiteId = suitesInDB.SuiteId,
                        SuiteName = suitesInDB.SuiteName,
                        UserId = loggedInUserId,
                        UsesSysEncryption = suitesInDB.UsesSysEncryption,
                        Applications = suitesInDB.Applications.ToModel(),
                        Environments = suitesInDB.Environments.ToModel(),
                        Regions = suitesInDB.Regions.ToModel(),
                        Servers = suitesInDB.Servers.ToModel(),
                        Users = suitesInDB.SuiteUsers.ToModel()
                    };

                    return suiteModels;
                }

                // If it has come here that means suite id is not existing, which is -ve case
                return default(SuiteModel);
            }
        }

        public SuiteModel GetSuite(long loggedInUserId, long suiteId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                // get suites-id of a user
                bool userAuthorized = UserAuthorizedToAccessSuite(unitOfWork, loggedInUserId, suiteId, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorized)
                    throw new UnauthorizedUserException("User does not have enough privileges to perform this action");

                // get suite from suite-id
                var suitesInDB = unitOfWork.Context.Suites.FirstOrDefault(x => x.SuiteId == suiteId);

                if (suitesInDB != null)
                {
                    var suiteModels = new SuiteModel
                    {
                        IsActive = suitesInDB.IsActive,
                        PrivateKey = suitesInDB.PrivateKey,
                        PublicKey = suitesInDB.PublicKey,
                        SuiteId = suitesInDB.SuiteId,
                        SuiteName = suitesInDB.SuiteName,
                        UserId = loggedInUserId,
                        UsesSysEncryption = suitesInDB.UsesSysEncryption,
                        Applications = suitesInDB.Applications.ToModel(),
                        Environments = suitesInDB.Environments.ToModel(),
                        Regions = suitesInDB.Regions.ToModel(),
                        Servers = suitesInDB.Servers.ToModel(),
                        Users = suitesInDB.SuiteUsers.ToModel()
                    };

                    return suiteModels;
                }

                // If it has come here that means suite id is not existing, which is -ve case
                return default(SuiteModel);
            }
        }

        public void UpdateSuite(SuiteModel suiteModel)
        {
            if (suiteModel == null) return;

            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessSuite(unitOfWork, suiteModel.UserId, suiteModel.SuiteId, new[] { RoleType.Admin });
                if (!userAuthorized)
                    throw new UnauthorizedUserException("User does not have enough privileges to perform this action");

                Suite suiteInDB = unitOfWork.Context.Suites.FirstOrDefault(x => x.SuiteId == suiteModel.SuiteId);

                if (suiteInDB == null)
                    throw new UnauthorizedUserException("User does not have enough privileges to perform this action");

                suiteInDB.IsActive = suiteModel.IsActive;
                suiteInDB.PrivateKey = suiteModel.PrivateKey;
                suiteInDB.SuiteName = suiteModel.SuiteName;
                suiteInDB.UsesSysEncryption = suiteModel.UsesSysEncryption;

                unitOfWork.Update(suiteInDB);

                //unitOfWork.Update<Application, ApplicationModel>(suiteInDB.Applications,
                //    suiteModel.Applications,
                //    application => application.ApplicationId,
                //    userModel => userModel.ApplicationId.GetValueOrDefault(-1),
                //    userModel =>
                //    {
                //        var dbObject = userModel.ToNewDbObject();

                //        dbObject.SuiteId = suiteInDB.SuiteId;
                //        dbObject.IsActive = true;
                //        return dbObject;
                //    });
            }
        }

        public bool DeleteSuite(SuiteModel suiteModel)
        {
            if (suiteModel == null) return false;

            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                var userSuitesInDB =
                    unitOfWork.Context.SuiteUsers.FirstOrDefault(
                        x => x.UserId == suiteModel.UserId && x.SuiteId == suiteModel.SuiteId);
                if (userSuitesInDB == null)
                    throw new UnauthorizedUserException(
                        "Invalid SuiteId or User does not have enough privileges to perform this action");

                unitOfWork.Delete(userSuitesInDB);

                return true;
                //TODO: Validate if all mappings etc get deleted too.
            }
        }

        public RoleType[] GetUserSuiteRole(long userId, long suiteId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                if (!unitOfWork.Context.SuiteUsers.Any(x => x.UserId == userId && x.SuiteId == suiteId))
                    throw new UnauthorizedUserException(
                      "Invalid SuiteId or User does not have enough privileges to perform this action");


                var roles = unitOfWork.Context.SuiteUsers.Where(x => x.UserId == userId && x.SuiteId == suiteId).ToList().Select(x => x.RoleId != null ?
                    (RoleType)x.RoleId : (RoleType)200);

                return roles.ToArray();
            }
        }
        #endregion

        #region Environment

        public EnvironmentModel GetEnvironment(long userId, long environmentId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                if (!UserAuthorizedToAccessEnvironment(unitOfWork, userId, environmentId, new[] { RoleType.Admin, RoleType.ReadOnly }))
                    throw new UnauthorizedUserException("User does not have enough privileges to perform this action");

                return unitOfWork.Context.Environments.FirstOrDefault(x => x.EnvironmentId == environmentId).ToModel();
            }
        }

        public EnvironmentModel GetEnvironment(long userId, string environmentName)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessEnvironment(unitOfWork, userId, environmentName, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorized)
                    throw new UnauthorizedUserException(
                        "User does not have access to retrieve Environment information: " + environmentName);

                return
                    unitOfWork.Context.Environments.FirstOrDefault(x => x.EnvironmentName == environmentName).ToModel();
            }
        }

        public EnvironmentModel AddEnvironment(EnvironmentModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            Environment environmentToReturn = null;

            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessSuite(unitOfWork, model.UserId, model.SuiteId, new[] { RoleType.Admin });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + model.SuiteId);

                environmentToReturn = model.ToNewDbObject();
                unitOfWork.Add(environmentToReturn);
            }

            return environmentToReturn.ToModel();
        }

        public EnvironmentModel UpdateEnvironment(EnvironmentModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            Environment environmentInDb;

            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessSuite(unitOfWork, model.UserId, model.SuiteId, new[] { RoleType.Admin });
                if (!userAuthorized)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite:" + model.SuiteId);

                environmentInDb =
                    unitOfWork.Context.Environments.FirstOrDefault(
                        x => x.EnvironmentId == model.EnvironmentId && x.SuiteId == model.SuiteId);
                if (environmentInDb != null)
                {
                    environmentInDb.EnvironmentTypeId = (int)model.EnvironmentType;
                    environmentInDb.EnvironmentName = model.EnvironmentName;

                    unitOfWork.Update(environmentInDb);
                }
                else
                {
                    throw new InvalidOperationException("No environment exists with id:" + model.EnvironmentId +
                                                        " and suite id: " + model.SuiteId);
                }
            }

            return environmentInDb.ToModel();
        }

        public long GetSuiteForEnvironmentId(long userId, long environmentId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessEnvironment(unitOfWork, userId, environmentId, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorized)
                    throw new UnauthorizedUserException("User does not have access to environment id:" + environmentId);

                var environment = unitOfWork.Context.Environments.FirstOrDefault(x => x.EnvironmentId == environmentId);
                if (environment != null)
                    return environment.SuiteId;

                return -1;
            }
        }

        public bool DeleteEnvironment(long userId, long environmentId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessEnvironment(unitOfWork, userId, environmentId, new[] { RoleType.Admin });
                if (!userAuthorized)
                    throw new UnauthorizedUserException("User does not have enough privileges to perform this action");

                var environmentFromDB =
                    unitOfWork.Context.Environments.FirstOrDefault(x => x.EnvironmentId == environmentId);

                // check for user again
                if (UserAuthorizedToAccessSuite(unitOfWork, userId, environmentFromDB.SuiteId, new[] { RoleType.Admin }))
                {
                    //TODO: Delete all mappings
                    unitOfWork.Delete(environmentFromDB);
                    return true;
                }

                return false;
            }
        }

        public List<EnvironmentModel> GetEnvironments(long userId, long suiteId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessEnvironment(unitOfWork, userId, suiteId, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + suiteId);

                return unitOfWork.Context.Environments.Where(x => x.SuiteId == suiteId).ToList().ToModel();
            }
        }

        #endregion

        #region Application

        public ApplicationModel GetApplication(long userId, long applicationId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessApplication(unitOfWork, userId, applicationId, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorized)
                    throw new UnauthorizedUserException(
                        "User does not have access to retrieve application information: " + applicationId);

                var output = unitOfWork.Context.Applications.FirstOrDefault(x => x.ApplicationId == applicationId);
                if (output == null) throw new InvalidOperationException("No such application exists: " + applicationId);
                return
                    output.ToModel();
            }
        }

        public ApplicationModel GetApplication(long userId, string applicationName)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessApplication(unitOfWork, userId, applicationName, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorized)
                    throw new UnauthorizedUserException(
                        "User does not have access to retrieve application information: " + applicationName);

                var output = unitOfWork.Context.Applications.FirstOrDefault(x => x.ApplicationName == applicationName);
                if (output == null)
                    throw new InvalidOperationException("No such application exists: " + applicationName);
                return output.ToModel();
            }
        }

        public ApplicationModel AddApplication(ApplicationModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            Application appToReturn;

            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessSuite(unitOfWork, model.UserId, model.SuiteId, new[] { RoleType.Admin });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + model.SuiteId);

                appToReturn = model.ToNewDbObject();
                unitOfWork.Add(appToReturn);
            }

            if (appToReturn != null)
                return appToReturn.ToModel();
            return null;
        }

        public ApplicationModel UpdateApplication(ApplicationModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            Application applicationInDb;

            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessSuite(unitOfWork, model.UserId, model.SuiteId, new[] { RoleType.Admin });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + model.SuiteId);


                applicationInDb =
                    unitOfWork.Context.Applications.FirstOrDefault(
                        x => x.ApplicationId == model.ApplicationId && x.SuiteId == model.SuiteId);
                if (applicationInDb != null)
                {
                    applicationInDb.Description = model.Description;
                    applicationInDb.ApplicationName = model.ApplicationName;

                    unitOfWork.Update(applicationInDb);
                }
                else
                {
                    throw new InvalidOperationException("No application exists with id:" + model.ApplicationId +
                                                        " and suite id: " + model.SuiteId);
                }
            }

            return applicationInDb.ToModel();
        }

        public long GetSuiteForApplicationId(long userId, long applicationId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessApplication(unitOfWork, userId, applicationId, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorized)
                    throw new UnauthorizedUserException(
                        "User does not have access to retrieve application information: " + applicationId);

                var application = unitOfWork.Context.Applications.FirstOrDefault(x => x.ApplicationId == applicationId);
                if (application != null)
                    return application.SuiteId;

                return -1;
            }
        }

        public List<ApplicationModel> GetApplications(long userId, long suiteId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessSuite(unitOfWork, userId, suiteId, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + suiteId);

                return unitOfWork.Context.Applications.Where(x => x.SuiteId == suiteId).ToList().ToModel();
            }
        }

        public bool DeleteApplication(long userId, long applicationId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessApplication(unitOfWork, userId, applicationId, new[] { RoleType.Admin });
                if (!userAuthorised)
                    throw new UnauthorizedUserException(
                        "Invalid ApplicationId or User does not have access to delete application: " + applicationId);

                var applicationFromDB =
                    unitOfWork.Context.Applications.FirstOrDefault(x => x.ApplicationId == applicationId);

                if (applicationFromDB != null &&
                    UserAuthorizedToAccessSuite(unitOfWork, userId, applicationFromDB.SuiteId, new[] { RoleType.Admin }))
                {
                    //TODO: Delete all mappings
                    unitOfWork.Delete(applicationFromDB);
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region Parameter

        public ParameterModel GetParameter(long userId, long parameterId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessParameter(unitOfWork, userId, parameterId, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorized)
                    throw new UnauthorizedUserException(
                        "User does not have access to retrieve Parameter information: " + parameterId);

                var output = unitOfWork.Context.Parameters.FirstOrDefault(x => x.ParameterId == parameterId);
                if (output == null) throw new InvalidOperationException("No such parameter exists: " + parameterId);
                return
                    output.ToModel();
            }
        }

        public ParameterModel GetParameter(long userId, string parameterName)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessParameter(unitOfWork, userId, parameterName, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorized)
                    throw new UnauthorizedUserException(
                        "User does not have access to retrieve Parameter information: " + parameterName);

                var output = unitOfWork.Context.Parameters.FirstOrDefault(x => x.ParameterName == parameterName);
                if (output == null) throw new InvalidOperationException("No such parameter exists: " + parameterName);
                return
                    output.ToModel();
            }
        }

        public ParameterModel AddParameter(ParameterModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            Parameter appToReturn;

            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessSuite(unitOfWork, model.UserId, model.SuiteId, new[] { RoleType.Admin });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + model.SuiteId);

                appToReturn = model.ToNewDbObject();
                unitOfWork.Add(appToReturn);
            }

            if (appToReturn != null)
                return appToReturn.ToModel();
            return null;
        }

        public ParameterModel UpdateParameter(ParameterModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            Parameter ParameterInDb;

            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessSuite(unitOfWork, model.UserId, model.SuiteId, new[] { RoleType.Admin });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + model.SuiteId);


                ParameterInDb =
                    unitOfWork.Context.Parameters.FirstOrDefault(
                        x => x.ParameterId == model.ParameterId && x.SuiteId == model.SuiteId);
                if (ParameterInDb != null)
                {
                    ParameterInDb.ParameterValue = model.ParameterValue;
                    ParameterInDb.ParameterName = model.ParameterName;

                    unitOfWork.Update(ParameterInDb);
                }
                else
                {
                    throw new InvalidOperationException("No Parameter exists with id:" + model.ParameterId +
                                                        " and suite id: " + model.SuiteId);
                }
            }

            return ParameterInDb.ToModel();
        }

        public long GetSuiteForParameterId(long userId, long parameterId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessParameter(unitOfWork, userId, parameterId, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorized)
                    throw new UnauthorizedUserException(
                        "User does not have access to retrieve Parameter information: " + parameterId);

                var Parameter = unitOfWork.Context.Parameters.FirstOrDefault(x => x.ParameterId == parameterId);
                if (Parameter != null)
                    return Parameter.SuiteId;

                return -1;
            }
        }

        public List<ParameterModel> GetParameters(long userId, long suiteId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessSuite(unitOfWork, userId, suiteId, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + suiteId);

                var paramsInDb = unitOfWork.Context.Mappings.Where(x => x.SuiteId == suiteId).ToList();

                return paramsInDb.Select(map => map.Parameter.ToModel()).ToList();
            }
        }

        public bool DeleteParameter(long userId, long parameterId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessParameter(unitOfWork, userId, parameterId, new[] { RoleType.Admin });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access to delete Parameter: " + parameterId);

                var ParameterFromDB = unitOfWork.Context.Parameters.FirstOrDefault(x => x.ParameterId == parameterId);

                if (ParameterFromDB != null && UserAuthorizedToAccessSuite(unitOfWork, userId, ParameterFromDB.SuiteId, new[] { RoleType.Admin }))
                {
                    //TODO: Delete all mappings
                    unitOfWork.Delete(ParameterFromDB);
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region Region

        public RegionModel GetRegion(long userId, long regionId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessRegion(unitOfWork, userId, regionId, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorized)
                    throw new UnauthorizedUserException("User does not have access to retrieve Region information: " +
                                                        regionId);

                var output = unitOfWork.Context.Regions.FirstOrDefault(x => x.RegionId == regionId);
                if (output == null) throw new InvalidOperationException("No such region exists: " + regionId);
                return
                    output.ToModel();
            }
        }

        public RegionModel GetRegion(long userId, string regionName)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessRegion(unitOfWork, userId, regionName, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorized)
                    throw new UnauthorizedUserException("User does not have access to retrieve Region information: " +
                                                        regionName);

                var output = unitOfWork.Context.Regions.FirstOrDefault(x => x.RegionName == regionName);
                if (output == null) throw new InvalidOperationException("No such region exists: " + regionName);
                return
                    output.ToModel();
            }
        }

        public RegionModel AddRegion(RegionModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            Region appToReturn;

            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessSuite(unitOfWork, model.UserId, model.SuiteId, new[] { RoleType.Admin });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + model.SuiteId);

                appToReturn = model.ToNewDbObject();
                unitOfWork.Add(appToReturn);
            }

            if (appToReturn != null)
                return appToReturn.ToModel();
            return null;
        }

        public RegionModel UpdateRegion(RegionModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            Region RegionInDb;

            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessSuite(unitOfWork, model.UserId, model.SuiteId, new[] { RoleType.Admin });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + model.SuiteId);


                RegionInDb =
                    unitOfWork.Context.Regions.FirstOrDefault(
                        x => x.RegionId == model.RegionId && x.SuiteId == model.SuiteId);
                if (RegionInDb != null)
                {
                    RegionInDb.Description = model.Description;
                    RegionInDb.RegionName = model.RegionName;

                    unitOfWork.Update(RegionInDb);
                }
                else
                {
                    throw new InvalidOperationException("No Region exists with id:" + model.RegionId + " and suite id: " +
                                                        model.SuiteId);
                }
            }

            return RegionInDb.ToModel();
        }

        public long GetSuiteForRegionId(long userId, long regionId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessRegion(unitOfWork, userId, regionId, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorized)
                    throw new UnauthorizedUserException("User does not have access to retrieve Region information: " +
                                                        regionId);

                var Region = unitOfWork.Context.Regions.FirstOrDefault(x => x.RegionId == regionId);
                if (Region != null)
                    return Region.SuiteId;

                return -1;
            }
        }

        public List<RegionModel> GetRegions(long userId, long suiteId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessSuite(unitOfWork, userId, suiteId, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + suiteId);

                return unitOfWork.Context.Regions.Where(x => x.SuiteId == suiteId).ToList().ToModel();
            }
        }

        public bool DeleteRegion(long userId, long regionId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessRegion(unitOfWork, userId, regionId, new[] { RoleType.Admin });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access to delete Region: " + regionId);

                var regionFromDB = unitOfWork.Context.Regions.FirstOrDefault(x => x.RegionId == regionId);

                if (regionFromDB != null && UserAuthorizedToAccessSuite(unitOfWork, userId, regionFromDB.SuiteId, new[] { RoleType.Admin }))
                {
                    //TODO: Delete all mappings
                    unitOfWork.Delete(regionFromDB);
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region Server

        public ServerModel GetServer(long userId, long serverId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessServer(unitOfWork, userId, serverId, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorized)
                    throw new UnauthorizedUserException("User does not have access to retrieve Server information: " +
                                                        serverId);

                var output = unitOfWork.Context.Servers.FirstOrDefault(x => x.ServerId == serverId);
                if (output == null) throw new InvalidOperationException("No such server exists: " + serverId);
                return
                    output.ToModel();
            }
        }

        public ServerModel GetServer(long userId, string serverName)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessServer(unitOfWork, userId, serverName, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorized)
                    throw new UnauthorizedUserException("User does not have access to retrieve Server information: " +
                                                        serverName);

                var output = unitOfWork.Context.Servers.FirstOrDefault(x => x.ServerName == serverName);
                if (output == null) throw new InvalidOperationException("No such server exists: " + serverName);
                return
                    output.ToModel();
            }
        }

        public ServerModel AddServer(ServerModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            Server appToReturn;

            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessSuite(unitOfWork, model.UserId, model.SuiteId, new[] { RoleType.Admin });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + model.SuiteId);

                appToReturn = model.ToNewDbObject();
                unitOfWork.Add(appToReturn);
            }

            if (appToReturn != null)
                return appToReturn.ToModel();
            return null;
        }

        public ServerModel UpdateServer(ServerModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            Server ServerInDb;

            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessSuite(unitOfWork, model.UserId, model.SuiteId, new[] { RoleType.Admin });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + model.SuiteId);


                ServerInDb =
                    unitOfWork.Context.Servers.FirstOrDefault(
                        x => x.ServerId == model.ServerId && x.SuiteId == model.SuiteId);
                if (ServerInDb != null)
                {
                    ServerInDb.Description = model.Description;
                    ServerInDb.ServerName = model.ServerName;

                    unitOfWork.Update(ServerInDb);
                }
                else
                {
                    throw new InvalidOperationException("No Server exists with id:" + model.ServerId + " and suite id: " +
                                                        model.SuiteId);
                }
            }

            return ServerInDb.ToModel();
        }

        public long GetSuiteForServerId(long userId, long serverId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorized = UserAuthorizedToAccessServer(unitOfWork, userId, serverId, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorized)
                    throw new UnauthorizedUserException("User does not have access to retrieve Server information: " +
                                                        serverId);

                var Server = unitOfWork.Context.Servers.FirstOrDefault(x => x.ServerId == serverId);
                if (Server != null)
                    return Server.SuiteId;

                return -1;
            }
        }

        public List<ServerModel> GetServers(long userId, long suiteId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessSuite(unitOfWork, userId, suiteId, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + suiteId);

                return unitOfWork.Context.Servers.Where(x => x.SuiteId == suiteId).ToList().ToModel();
            }
        }

        public bool DeleteServer(long userId, long serverId)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessServer(unitOfWork, userId, serverId, new[] { RoleType.Admin });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access to delete Server: " + serverId);

                var ServerFromDB = unitOfWork.Context.Servers.FirstOrDefault(x => x.ServerId == serverId);

                if (ServerFromDB != null && UserAuthorizedToAccessSuite(unitOfWork, userId, ServerFromDB.SuiteId, new[] { RoleType.Admin, RoleType.ReadOnly }))
                {
                    //TODO: Delete all mappings
                    unitOfWork.Delete(ServerFromDB);
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region Parameter Special - Do not delete

        public List<ParameterModel> GetParametersLike(long userId, long suiteId, string term)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                if (!UserAuthorizedToAccessSuite(unitOfWork, userId, suiteId, new[] { RoleType.Admin, RoleType.ReadOnly }))
                    return null;

                var parmetersMatching =
                    unitOfWork.Context.Parameters.Where(x => x.SuiteId == suiteId && x.ParameterName.Contains(term))
                        .ToList();

                return parmetersMatching.ToModel();
            }
        }

        #endregion

        #region UserAccess

        private bool UserAuthorizedToAccessSuite(UnitOfWork unitOfWork, long loggedInUserId, long suiteId, RoleType[] roleTypes)
        {
            return unitOfWork.Context.SuiteUsers.Any(x => x.UserId == loggedInUserId && x.SuiteId == suiteId && roleTypes.Contains((RoleType)x.Role.RoleId));
        }

        private bool UserAuthorizedToAccessSuite(UnitOfWork unitOfWork, long loggedInUserId, string suiteName, RoleType[] roleTypes)
        {
            return unitOfWork.Context.SuiteUsers.Any(x => x.UserId == loggedInUserId && x.Suite.SuiteName == suiteName && roleTypes.Contains((RoleType)x.Role.RoleId));
        }

        public bool UserAuthorizedToAccessApplication(UnitOfWork unitOfWork, long userId, long applicationId, RoleType[] roleTypes)
        {
            return
                unitOfWork.Context.Applications.Any(
                    x => x.ApplicationId == applicationId && x.Suite.SuiteUsers.Any(y => y.UserId == userId && roleTypes.Contains((RoleType)y.Role.RoleId)));
        }

        public bool UserAuthorizedToAccessApplication(UnitOfWork unitOfWork, long userId, string applicationName, RoleType[] roleTypes)
        {
            return
                unitOfWork.Context.Applications.Any(
                    x => x.ApplicationName == applicationName && x.Suite.SuiteUsers.Any(y => y.UserId == userId && roleTypes.Contains((RoleType)y.Role.RoleId)));
        }

        private bool UserAuthorizedToAccessRegion(UnitOfWork unitOfWork, long userId, long regionId, RoleType[] roleTypes)
        {
            return
                unitOfWork.Context.Regions.Any(
                    x => x.RegionId == regionId && x.Suite.SuiteUsers.Any(y => y.UserId == userId && roleTypes.Contains((RoleType)y.Role.RoleId)));
        }

        private bool UserAuthorizedToAccessRegion(UnitOfWork unitOfWork, long userId, string region, RoleType[] roleTypes)
        {
            return
                unitOfWork.Context.Regions.Any(
                    x => x.RegionName == region && x.Suite.SuiteUsers.Any(y => y.UserId == userId && roleTypes.Contains((RoleType)y.Role.RoleId)));
        }

        private bool UserAuthorizedToAccessServer(UnitOfWork unitOfWork, long userId, long serverId, RoleType[] roleTypes)
        {
            return
                unitOfWork.Context.Servers.Any(
                    x => x.ServerId == serverId && x.Suite.SuiteUsers.Any(y => y.UserId == userId && roleTypes.Contains((RoleType)y.Role.RoleId)));
        }

        private bool UserAuthorizedToAccessServer(UnitOfWork unitOfWork, long userId, string server, RoleType[] roleTypes)
        {
            return
                unitOfWork.Context.Servers.Any(
                    x => x.ServerName == server && x.Suite.SuiteUsers.Any(y => y.UserId == userId && roleTypes.Contains((RoleType)y.Role.RoleId)));
        }

        private bool UserAuthorizedToAccessParameter(UnitOfWork unitOfWork, long userId, long parameterId, RoleType[] roleTypes)
        {
            return
                unitOfWork.Context.Parameters.Any(
                    x => x.ParameterId == parameterId && x.Suite.SuiteUsers.Any(y => y.UserId == userId && roleTypes.Contains((RoleType)y.Role.RoleId)));
        }

        private bool UserAuthorizedToAccessParameter(UnitOfWork unitOfWork, long userId, string parameter, RoleType[] roleTypes)
        {
            return
                unitOfWork.Context.Parameters.Any(
                    x => x.ParameterName == parameter && x.Suite.SuiteUsers.Any(y => y.UserId == userId && roleTypes.Contains((RoleType)y.Role.RoleId)));
        }

        private bool UserAuthorizedToAccessEnvironment(UnitOfWork unitOfWork, long userId, long environmentId, RoleType[] roleTypes)
        {
            return
                unitOfWork.Context.Environments.Any(
                    x => x.EnvironmentId == environmentId && x.Suite.SuiteUsers.Any(y => y.UserId == userId && roleTypes.Contains((RoleType)y.Role.RoleId)));
        }


        private bool UserAuthorizedToAccessEnvironment(UnitOfWork unitOfWork, long userId, string environment, RoleType[] roleTypes)
        {
            return
                unitOfWork.Context.Environments.Any(
                    x => x.EnvironmentName == environment && x.Suite.SuiteUsers.Any(y => y.UserId == userId && roleTypes.Contains((RoleType)y.Role.RoleId)));
        }

        #endregion

        #region Mapping

        public MappingModel AddMapping(MappingModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            Mapping mapToReturn;

            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessSuite(unitOfWork, model.UserId, model.SuiteId, new[] { RoleType.Admin });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + model.SuiteId);

                if (
                    !unitOfWork.Context.Parameters.Any(
                        x => x.ParameterId == model.ParameterId && x.SuiteId == model.SuiteId))
                    throw new InvalidOperationException(
                        "Mismatch between parameter id and suite id. Can not add mapping!");

                if (!unitOfWork.Context.Servers.Any(x => x.ServerId == model.ServerId && x.SuiteId == model.SuiteId))
                    throw new InvalidOperationException("Mismatch between server id and suite id. Can not add mapping!");

                if (!unitOfWork.Context.Regions.Any(x => x.RegionId == model.RegionId && x.SuiteId == model.SuiteId))
                    throw new InvalidOperationException("Mismatch between region id and suite id. Can not add mapping!");

                if (
                    !unitOfWork.Context.Environments.Any(
                        x => x.EnvironmentId == model.EnvironmentId && x.SuiteId == model.SuiteId))
                    throw new InvalidOperationException(
                        "Mismatch between environment id and suite id. Can not add mapping!");

                if (
                    !unitOfWork.Context.Applications.Any(
                        x => x.ApplicationId == model.ApplicationId && x.SuiteId == model.SuiteId))
                    throw new InvalidOperationException(
                        "Mismatch between application id and suite id. Can not add mapping!");

                mapToReturn = model.ToNewDbObject();
                unitOfWork.Add(mapToReturn);
            }

            if (mapToReturn != null)
                return mapToReturn.ToModel();
            return null;
        }

        public List<MappingModel> GetMapping(long userId, long suiteId)
        {
            List<MappingModel> mapToReturn = new List<MappingModel>();

            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessSuite(unitOfWork, userId, suiteId, new[] { RoleType.Admin, RoleType.ReadOnly });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + suiteId);

                mapToReturn.AddRange(
                    unitOfWork.Context.Mappings.Where(x => x.SuiteId == suiteId).ToList().Select(x => x.ToModel()));
            }

            return mapToReturn;
        }

        public bool DeleteMapping(long userId, long suiteId, long mappingId)
        {
            var mapToReturn = new List<MappingModel>();

            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                bool userAuthorised = UserAuthorizedToAccessSuite(unitOfWork, userId, suiteId, new[] { RoleType.Admin });
                if (!userAuthorised)
                    throw new UnauthorizedUserException("User does not have access or sufficient privileges for this action to suite: " + suiteId);

                var mapping =
                    unitOfWork.Context.Mappings.FirstOrDefault(x => x.SuiteId == suiteId && x.MappingId == mappingId);
                if (mappingId == null)
                    throw new InvalidOperationException("No such mapping exists in the suite");

                unitOfWork.Delete(mapping);
            }

            return true;
        }

        #endregion

        #region Configuration

        public List<ConfigurationModel> GetConfigurations(long userId, long appId, long serverId, long envId,
            long regionId, string publicKey)
        {
            var suiteId = GetSuiteForApplicationId(userId, appId);

            List<ConfigurationModel> mapToReturn = new List<ConfigurationModel>();
            bool decryptionRequired = !string.IsNullOrEmpty(publicKey);
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                var mappings = unitOfWork.Context.Mappings.Where(x => x.ApplicationId == appId
                                                                      && x.ServerId == serverId &&
                                                                      x.EnvironmentId == envId
                                                                      && x.RegionId == regionId).ToList();

                mapToReturn = mappings.Select(x => new ConfigurationModel
                {
                    ParameterName = x.Parameter.ParameterName,
                    ParameterValue = x.Parameter.ParameterValue,
                    SuiteId = x.SuiteId,
                    MatchProfile =
                        String.Format("ParamId:{0}|App:{1}|Server:{2}|Env:{3}|Region:{4}|Suite:{5}",
                            x.Parameter.ParameterId, x.ApplicationId,
                            x.ServerId, x.EnvironmentId,
                            x.RegionId, x.SuiteId),
                    IsEncrypted = x.Parameter.IsEncrypted
                }).ToList();
            }

            return mapToReturn;
        }

        #endregion

        #region UserManagament

        private string GetEncryptedPasswordForUserCredentials(string username, string password, string salt)
        {
            return EncryptionEngine.Get<SHA256Encryption>().Encrypt("@#" + username.ToUpperInvariant() + password + ".!@", salt);
        }

        public AuthenticationModel GetAuthenticatedInfo(string username, string password, string getHash)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                var userInfo = unitOfWork.Context.Users.FirstOrDefault(x => x.UserName == username);
                if (userInfo != null)
                {
                    int userId = userInfo.UserId;
                    var membershipInfo = unitOfWork.Context.Memberships.FirstOrDefault(x => x.UserId == userId);
                    if (membershipInfo != null)
                    {
                        var hashedPassword = GetEncryptedPasswordForUserCredentials(username, password,
                            membershipInfo.PasswordSalt);
                        if (!unitOfWork.Context.Memberships.Any(x => x.UserId == userId && x.Password == hashedPassword))
                            throw new UnauthorizedUserException("0ADSGAI03: Invalid username or password:" + username);

                        return new AuthenticationModel { UserId = userId, IsAuthenticated = true };
                    }
                    throw new UnauthorizedUserException("0ADSGAI02: Invalid username or password:" + username);
                }
                throw new UnauthorizedUserException("0ADSGAI01: Invalid username or password:" + username);
            }
        }

        public RegisterModel AddUser(string username, string password, string randomSalt)
        {
            User userInDb = null;

            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                if (unitOfWork.Context.Users.Any(x => x.UserName == username))
                    throw new UserAlreadyExistsException(username);

                userInDb = unitOfWork.Context.Users.Add(new User
                {
                    UserName = username
                });

                // Add Membership
                unitOfWork.Context.Memberships.Add(new Membership
                {
                    UserId = userInDb.UserId,
                    Password = GetEncryptedPasswordForUserCredentials(username, password, randomSalt),
                    PasswordSalt = randomSalt
                });
            }

            return new RegisterModel
            {
                UserId = userInDb.UserId,
                UserName = userInDb.UserName
            };
        }

        public bool GrantRoleAccessToSuite(long suiteId, long loggedInUserId, string username, RoleType role)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                if (!UserAuthorizedToAccessSuite(unitOfWork, loggedInUserId, suiteId, new[] { RoleType.Admin }))
                    throw new UnauthorizedUserException("Insufficient privileges to add user to suite");

                var user =
                    unitOfWork.Context.Users.FirstOrDefault(
                        x => x.UserName.Equals(username, StringComparison.InvariantCulture));
                if (user == null)
                    throw new InvalidOperationException("No user exists with name:" + username);

                unitOfWork.Context.SuiteUsers.Add(new SuiteUser
                {
                    SuiteId = suiteId,
                    UserId = user.UserId,
                    RoleId = (int)role
                });

                return true;
            }
        }

        public bool RevokeRoleAccessToSuite(long suiteId, long loggedInUserId, string username)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                if (!UserAuthorizedToAccessSuite(unitOfWork, loggedInUserId, suiteId, new[] { RoleType.Admin }))
                    throw new UnauthorizedUserException("Insufficient privileges to revoke user access from suite");

                var user =
                    unitOfWork.Context.Users.FirstOrDefault(
                        x => x.UserName.Equals(username, StringComparison.InvariantCulture));
                if (user == null)
                    throw new InvalidOperationException("No user exists with name:" + username);

                var suiteUsers =
                    unitOfWork.Context.SuiteUsers.Where(x => x.SuiteId == suiteId && x.UserId == user.UserId);
                foreach (var suiteUser in suiteUsers)
                {
                    unitOfWork.Context.SuiteUsers.Remove(suiteUser);
                }

                return true;
            }
        }

        #endregion

        #region Audit

        public void AddAuditRecord(AuditRecordModel auditRecord)
        {
            using (var unitOfWork = new UnitOfWork(_connectionString))
            {
                unitOfWork.Add(auditRecord.ToNewDbObject());
            }
        }

        #endregion


    }
}