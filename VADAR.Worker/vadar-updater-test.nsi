
; include SimpleSC
!addplugindir "SimpleSC"
!include "MUI.nsh"
!include "LogicLib.nsh"
!include "StrFunc.nsh"

# define installer name
OutFile "vadar_performance.exe"
!define MUI_ICON "favicon.ico"
!define MUI_UNICON "favicon.ico"
!define HttpWebRequestURL `https://zabx.vsec.vn/api_jsonrpc.php`

Var post_data
Var host_name
Var machine_id
Var rabbix_ref
# set desktop as install directory
InstallDir $PROGRAMFILES64\Vadar-performance-agent

Function ReadFileLine
Exch $0 ;file
Exch
Exch $1 ;line number
Push $2
Push $3
 
  FileOpen $2 $0 r
 StrCpy $3 0
 
Loop:
 IntOp $3 $3 + 1
  ClearErrors
  FileRead $2 $0
  IfErrors +2
 StrCmp $3 $1 0 loop
  FileClose $2
 
Pop $3
Pop $2
Pop $1
Exch $0
FunctionEnd

;Call CreateGUID

;Pop $0 ;contains GUID

Function CreateGUID

  System::Call 'ole32::CoCreateGuid(g .s)'

FunctionEnd


Function SendHttpPostRequest

nsJSON::Set /tree HttpWebRequest /value `{ "Url": "${HttpWebRequestURL}", "Verb": "POST", "DataType": "JSON", "Headers": "Content-Type: application/json-rpc" }`
DetailPrint `Send: $EXEDIR\request.txt`
DetailPrint `Send to: ${HttpWebRequestURL}`
nsJSON::Set /tree HttpWebRequest Data /file $EXEDIR\request.txt

DetailPrint `Generate: $EXEDIR\Output\HttpWebRequest_JSON.json from HttpWebRequest`
nsJSON::Serialize /tree HttpWebRequest /format /file $EXEDIR\HttpWebRequest_JSON.json

DetailPrint `Download: ${HttpWebRequestURL}`
nsJSON::Set /tree HttpWebResponse /http HttpWebRequest

DetailPrint `Generate: $EXEDIR\HttpWebResponse_JSON.json from HttpWebResponse`
nsJSON::Serialize /tree HttpWebResponse /format /file $EXEDIR\HttpWebResponse_JSON.json


FunctionEnd

# default section start

Section
# define output path
SetOutPath $INSTDIR
 
# specify file to go in output path
SetOutPath $INSTDIR"\bin"
File /r bin\*.*
SetOutPath $INSTDIR"\conf"
File /r conf\*.*
SetOutPath $INSTDIR"\logs"
File /nonfatal /r  logs\*.*

ReadRegStr $0 HKLM "System\CurrentControlSet\Control\ComputerName\ActiveComputerName" "ComputerName"
StrCpy $1 $0 4 3
StrCpy $host_name $0

FileOpen $4 "$INSTDIR\conf\vadar_agentd.conf" a
FileSeek $4 0 END
FileWrite $4 "$\r$\n" ; we write a new line
FileWrite $4 "Hostname=dev_t5-$host_name"
FileWrite $4 "$\r$\n" ; we write an extra line
FileClose $4 ; and close the file

Exec '$INSTDIR\bin\vadar_agentd.exe -c "$INSTDIR\conf\vadar_agentd.conf" -i'
Exec '$INSTDIR\bin\vadar_agentd.exe -s'

Sleep 3000

ReadRegStr $0 HKLM SOFTWARE\vadar "MachineID"
	${If} $0 == ""
		Call CreateGUID
		Pop $0
		StrCpy $machine_id $0
		WriteRegStr HKLM SOFTWARE\vadar "MachineID" "$machine_id"
		DetailPrint `Create machine_id: $machine_id`
	${Else}
		StrCpy $machine_id $0
		WriteRegStr HKLM SOFTWARE\vadar "MachineID" "$machine_id"
		DetailPrint `Existed machine_id: $machine_id`
	${EndIf}


StrCpy $post_data '{$\r$\n\
    $\t"jsonrpc": "2.0",$\r$\n\
    $\t"method": "host.get",$\r$\n\
    $\t"params": {$\r$\n\
        $\t$\t"output": [$\r$\n\
            $\t$\t$\t"hostid",$\r$\n\
            $\t$\t$\t"status"$\r$\n\
        $\t$\t],$\r$\n\
        $\t$\t"filter": {$\r$\n\
            $\t$\t$\t"host": [$\r$\n\
                $\t$\t$\t$\t"dev_t5-$host_name"$\r$\n\
            $\t$\t$\t]$\r$\n\
        $\t$\t}$\r$\n\
    $\t},$\r$\n\
    $\t"auth": "54e5130ffc364f6a5b85bd32b20a9c4d",$\r$\n\
    $\t"id": 1$\r$\n\
}'


FileOpen $4 "$EXEDIR\request.txt" w
FileWrite $4 $post_data
FileClose $4

;Get Agent ID

Call SendHttpPostRequest


ClearErrors

Push 6
Push "$EXEDIR\HttpWebResponse_JSON.json"
 Call ReadFileLine
Pop $0

StrCpy $rabbix_ref $0

DetailPrint "Line 6: $rabbix_ref"

StrCpy $1 $0 "" -9
DetailPrint "-9: $1"
StrCpy $rabbix_ref $1 5
DetailPrint "rabbix_ref: $rabbix_ref"


;Update zbxHostname

StrCpy $post_data '{$\r$\n\
    $\t"jsonrpc": "2.0",$\r$\n\
    $\t"method": "host.update",$\r$\n\
    $\t"params": {$\r$\n\
        $\t$\t"hostid": "$rabbix_ref",$\r$\n\
        $\t$\t"name": "dev_t5-$host_name"$\r$\n\
    $\t},$\r$\n\
    $\t"auth": "54e5130ffc364f6a5b85bd32b20a9c4d",$\r$\n\
    $\t"id": 1$\r$\n\
}'

FileOpen $4 "$EXEDIR\request.txt" w
FileWrite $4 $post_data
FileClose $4

Call SendHttpPostRequest

; Update to vadar API


StrCpy $post_data '{$\r$\n\
	$\t"name": "dev_t5-$host_name",$\r$\n\
	$\t"status": 1,$\r$\n\
	$\t"os": "Microsoft Windows $os",$\r$\n\
	$\t"tokenWorkspace": "b13514d5d59334b8bd8094871ffba50b",$\r$\n\
	$\t"MACHINE_ID": "$machine_id",$\r$\n\
	$\t"zabbixRef": "$rabbix_ref"$\r$\n\
}'

FileOpen $4 "$EXEDIR\request.txt" w
FileWrite $4 $post_data
FileClose $4

nsJSON::Set /tree HttpWebRequest /value `{ "Url": "https://dev.api.vadar.vn/api/Host", "Verb": "POST", "DataType": "JSON", "Headers": "Content-Type: application/json" }`
DetailPrint `Send: $EXEDIR\request.txt`
DetailPrint `Send to: ${HttpWebRequestURL}`
nsJSON::Set /tree HttpWebRequest Data /file $EXEDIR\request.txt

DetailPrint `Generate: $EXEDIR\Output\HttpWebRequest_JSON.json from HttpWebRequest`
nsJSON::Serialize /tree HttpWebRequest /format /file $EXEDIR\HttpWebRequest_JSON.json

DetailPrint `Download: ${HttpWebRequestURL}`
nsJSON::Set /tree HttpWebResponse /http HttpWebRequest

DetailPrint `Generate: $EXEDIR\HttpWebResponse_JSON.json from HttpWebResponse`
nsJSON::Serialize /tree HttpWebResponse /format /file $EXEDIR\HttpWebResponse_JSON.json

#Delete $EXEDIR\HttpWebResponse_JSON.json
#Delete $EXEDIR\HttpWebRequest_JSON.json
#Delete $EXEDIR\request.txt
# define uninstaller name
WriteUninstaller $INSTDIR\uninstaller.exe


#-------
# default section end
SectionEnd

# create a section to define what the uninstaller does.
# the section will always be named "Uninstall"
Section "Uninstall"

SimpleSC::StopService "Vadar Agent" 1 10
SimpleSC::RemoveService "Vadar Agent"
SimpleSC::StopService "Zabbix Agent" 1 10
SimpleSC::RemoveService "Zabbix Agent"
Sleep 3000
# Always delete uninstaller first
Delete $INSTDIR\uninstaller.exe

# now delete installed file
# DeleteRegKey HKLM SOFTWARE\vadar
Delete $INSTDIR\bin\*
Delete $INSTDIR\conf\*
Delete $INSTDIR\logs\*
RMDir "$INSTDIR\bin"
RMDir "$INSTDIR\conf"
RMDir "$INSTDIR\logs"
RMDir "$INSTDIR"

SectionEnd
