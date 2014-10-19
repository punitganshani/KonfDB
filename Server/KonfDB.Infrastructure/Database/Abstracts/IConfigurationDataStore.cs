#region License and Product Information

// 
//     This file 'IConfigurationDataStore.cs' is part of KonfDB application - 
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

using System.Collections.Generic;
using KonfDB.Infrastructure.Database.Entities.Account;
using KonfDB.Infrastructure.Database.Entities.Configuration;
using KonfDB.Infrastructure.Database.Enums;

namespace KonfDB.Infrastructure.Database.Abstracts
{
    public interface IConfigurationDataStore : IDataStore
    {
        /* Suite */
        List<SuiteModel> GetSuites(long userId);
        SuiteModel AddSuite(SuiteCreateModel suiteModel);
        SuiteModel GetSuite(long loggedInUserId, long suiteId);
        SuiteModel GetSuite(long loggedInUserId, string suiteName);
        void UpdateSuite(SuiteModel model);
        bool DeleteSuite(SuiteModel model);
        RoleType[] GetUserSuiteRole(long userId, long suiteId);

        /* Environment */
        bool DeleteEnvironment(long userId, long environmentId);
        EnvironmentModel GetEnvironment(long userId, long environmentId);
        EnvironmentModel GetEnvironment(long userId, string environmentName);
        EnvironmentModel AddEnvironment(EnvironmentModel model);
        EnvironmentModel UpdateEnvironment(EnvironmentModel environment);
        long GetSuiteForEnvironmentId(long userId, long suiteId);
        List<EnvironmentModel> GetEnvironments(long userId, long suiteId);

        /* Applications */
        ApplicationModel GetApplication(long userId, long applicationId);
        ApplicationModel GetApplication(long userId, string applicationName);
        ApplicationModel AddApplication(ApplicationModel model);
        ApplicationModel UpdateApplication(ApplicationModel application);
        long GetSuiteForApplicationId(long userId, long ApplicationId);
        List<ApplicationModel> GetApplications(long userId, long suiteId);
        //TODO: Validate if the user actually belongs to this suite
        bool DeleteApplication(long userId, long applicationId);

        /* Regions */
        RegionModel GetRegion(long userId, long regionId);
        RegionModel GetRegion(long userId, string regionName);
        RegionModel AddRegion(RegionModel model);
        RegionModel UpdateRegion(RegionModel region);
        long GetSuiteForRegionId(long userId, long regionId);
        List<RegionModel> GetRegions(long userId, long suiteId);
        //TODO: Validate if the user actually belongs to this suite
        bool DeleteRegion(long userId, long regionId);

        /* Servers */
        ServerModel GetServer(long userId, long serverId);
        ServerModel GetServer(long userId, string serverName);
        ServerModel AddServer(ServerModel model);
        ServerModel UpdateServer(ServerModel server);
        long GetSuiteForServerId(long userId, long serverId);
        List<ServerModel> GetServers(long userId, long suiteId);
        //TODO: Validate if the user actually belongs to this suite
        bool DeleteServer(long userId, long serverId);

        /* Parameters */
        ParameterModel GetParameter(long userId, long parameterId);
        ParameterModel GetParameter(long userId, string parameterName);
        ParameterModel AddParameter(ParameterModel model);
        ParameterModel UpdateParameter(ParameterModel parameter);
        long GetSuiteForParameterId(long userId, long parameterId);
        List<ParameterModel> GetParameters(long userId, long suiteId);
        bool DeleteParameter(long userId, long parameterId);

        /* Parameters */
        List<ParameterModel> GetParametersLike(long userId, long suiteId, string term);
        //ParameterDetailModel GetParameterDetails(long userId, long parameterId);

        /* Mapping */
        MappingModel AddMapping(MappingModel model);
        List<MappingModel> GetMapping(long userId, long suiteId);

        List<ConfigurationModel> GetConfigurations(long userId, long appId, long serverId, long envId, long regionId,
            string publicKey);

        bool DeleteMapping(long userId, long suiteId, long mappingId);
        /* User management */
        AuthenticationModel GetAuthenticatedInfo(string username, string password, string getHash);
        RegisterModel AddUser(string username, string password, string randomSalt);
        bool GrantRoleAccessToSuite(long suiteId, long loggedInUserId, string username, RoleType role);
        bool RevokeRoleAccessToSuite(long suiteId, long loggedInUserId, string username);


        /* Audit */
        void AddAuditRecord(AuditRecordModel model);

        /* Internal */
        Dictionary<string, string> GetSettings(bool active, bool autoLoad);
    }
}

//NOTE: Parameter has ParameterValue instead of Description