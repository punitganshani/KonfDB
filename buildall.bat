REM Build KonfDB
SET CONFIGURATION=%1
set PATH_SOURCE_SLN="%cd%\KonfDB.sln"

if [%1]==[] (
  SET CONFIGURATION=debug-net4.6
)

MSBuild %PATH_SOURCE_SLN% /p:Configuration=%CONFIGURATION%
