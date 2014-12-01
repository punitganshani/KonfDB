REM Build KonfDB
SET CONFIGURATION=%1

set PATH_SOURCE_KONFDBH="%cd%\Server\KonfDBHost\KonfDBHost.csproj"
set PATH_SOURCE_SLN="%cd%\KonfDB.sln"

set buildpath=%cd%\Build\%CONFIGURATION%\
md "%buildpath%"


MSBuild %PATH_SOURCE_SLN% /p:Configuration=%CONFIGURATION% /p:BaseOutputPath="%buildpath%"

REM MSBuild %PATH_SOURCE_KONFDBH% /p:Configuration=Release /p:BaseOutputPath="%buildpath%"
