@echo off
sc config NetTcpActivator start= auto
sc config NetTcpPortSharing start= auto
net start NetTcpActivator
net start NetTcpPortSharing
exit /b 0