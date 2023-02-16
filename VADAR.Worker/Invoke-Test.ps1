Start-Sleep -Seconds 10
$FilePath = "${env:UserProfile}\powershell.msi.test.log"            

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
[string]$os = [environment]::OSVersion.Version

$keyPath = $scriptPath + "\client.keys"

$key  = Get-Content -Path $keyPath

$wazuh_ref = $key.Split(" ")[0]

#$wazuh_ref | Out-File -Encoding ASCII -FilePath $

$machine_id = (Get-ItemProperty -Path Registry::HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\vadar -Name "MachineID").MachineID
if (!$machine_id){
    $machine_id = (Get-ItemProperty -Path Registry::HKEY_LOCAL_MACHINE\SOFTWARE\vadar -Name "MachineID").MachineID
    
    if(!$machine_id){
        $machine_id = '{'+[guid]::NewGuid().ToString()+'}'
		New-Item -Path HKLM:\SOFTWARE\WOW6432Node\vadar -Value "default value"
        New-ItemProperty -Path "HKLM:\SOFTWARE\WOW6432Node\vadar" -Name "MachineID" -Value $machine_id  -PropertyType "String"
    }
}

[string]$Agent_name = $env:computername

$url = "https://fb1ffb16f77afe091b4502f1783aadc3.m.pipedream.net"
$url_vadar = "https://dev.api.vadar.vn/api/Host"

  
[string]$data ="{
  `"name`": `"dev_t5-$Agent_name`",
  `"description`": `"`",
  `"os`": `"Microsoft Windows $os`",
  `"status`": 1,
  `"tokenWorkspace`": `"b13514d5d59334b8bd8094871ffba50b`",
  `"MACHINE_ID`": `"$machine_id`",
  `"wazuhRef`": `"$wazuh_ref`"
  }"
  
Out-File -FilePath .\Process.txt -InputObject $data -Encoding ASCII

$curl_path = $scriptPath + "\curl.exe"
$output_path = $scriptPath + "\output.txt"

& $curl_path --request POST $url_vadar --header "Content-Type: application/json" -d "@Process.txt" | Out-File -FilePath $output_path
& $curl_path --request POST $url_vadar --header "Content-Type: application/json" -d "@Process.txt" | Out-File -FilePath $output_path
& $curl_path --request POST $url_vadar --header "Content-Type: application/json" -d "@Process.txt" | Out-File -FilePath $output_path
& $curl_path --request POST $url_vadar --header "Content-Type: application/json" -d "@Process.txt" | Out-File -FilePath $output_path

#Start-Service OssecSvc
#Write-Output $Agent_name
