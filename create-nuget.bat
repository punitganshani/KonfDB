@echo off
SET HOME=%cd%

cd "%HOME%"
call buildall.bat release-net4.0
cd "%HOME%"
call buildall.bat release-net4.5
cd "%HOME%\Client\KonfDBCF"
NuGet Pack

cd "%HOME%\Server\KonfDBHost"
NuGet Pack

cd "%HOME%"