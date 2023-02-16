SETLOCAL
SET PATH=%PATH%;C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin
SET PATH=%PATH%;C:\Program Files (x86)\WiX Toolset v3.11\bin

REM %1
set VERSION=1.0.0
REM %2
set REVISION=0

REM IF VERSION or REVISION are empty, ask for their value
REM IF [%VERSION%] == [] set /p VERSION=Enter the version of the Wazuh agent (x.y.z):
REM IF [%REVISION%] == [] set /p REVISION=Enter the revision of the Wazuh agent:

SET MSI_NAME=vadar_security.msi

candle.exe -nologo "wazuh-installer.wxs" -out "wazuh-installer.wixobj" -ext WixUtilExtension -ext WixUiExtension
light.exe "wazuh-installer.wixobj" -out "%MSI_NAME%"  -ext WixUtilExtension -ext WixUiExtension

signtool sign /a /tr http://rfc3161timestamp.globalsign.com/advanced /d "%MSI_NAME%" /td SHA256 "%MSI_NAME%"


