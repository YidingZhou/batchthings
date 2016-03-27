# Utility to setup/delete/list/pause/resume mdcs cluster

# Global configuration
$script:GITHUB_BASE_URL = "https://raw.githubusercontent.com/YidingZhou/batchthings/mdcs/matlab-cluster/"

function PrepAzureContext() {
  echo "Validating Azure logon..."
  while($true) {
    $script:current_subscriptions = (Get-AzureRmSubscription) # cached for create use, if necessary
    if(-not $?) {
      echo "No active login account, log in now..."
      Login-AzureRmAccount
    } else {
      break
    }
  }
}


function mdcs_create($p) {
  function readstring($info, $default) {
    $response = Read-Host -Prompt "$info [$default]"
    if(-not $response) {
      $response = $default
    }
    return $response
  }

  function parse_init($p) {
    $script:inifile_entries = @("ClusterName", "NumberWorkers", "ClientVmSize", "MJSVmSize", "WorkerVmSize", "VmUsername", "SubscriptionId", "Region", "BaseVmVhd", "ClusterVmVhdContainer", "SubscriptionId")

    # load static default config
    $script:config = @{}
    foreach ($entry in $inifile_entries) {
      $script:config[$entry] = ""
    }

    # parse args
    if($p.Count -gt 0) {
      echo "using conf file at $($p[0])"
      $script:config["inifile"] = $p[0]
    } else {
      echo "using default location for mdcsconf.ini"
      $script:config["inifile"] = "mdcsconf.ini"
    }

    # load config from ini file
    if(Test-Path $script:config["inifile"]) {
      echo "parsing init file"
      $iniContent = Get-IniContent $script:config["inifile"]
      if($iniContent.ContainsKey("config")) {
        foreach ($entry in $inifile_entries) {
          if($iniContent["config"].ContainsKey($entry)) {
            $script:config[$entry] = $iniContent["config"][$entry]
          }
        }
      }
    }
  }

  function Get-IniContent ($filePath)
  {
      $ini = @{}
      switch -regex -file $FilePath
      {
          "^\[(.+)\]" # Section
          {
              $section = $matches[1]
              $ini[$section] = @{}
              $CommentCount = 0
          }
          "^(;.*)$" # Comment
          {
              $value = $matches[1]
              $CommentCount = $CommentCount + 1
              $name = "Comment" + $CommentCount
              $ini[$section][$name] = $value
          }
          "(.+?)\s*=(.*)" # Key
          {
              $name,$value = $matches[1..2]
              $ini[$section][$name] = $value
          }
      }
      return $ini
  }

  parse_init($p)

  $subs = ($script:current_subscriptions | % {$_.SubscriptionId})
  if($subs -contains $script:config["SubscriptionId"]) {
    Set-AzureRmContext -SubscriptionId $script:config["SubscriptionId"]
  } else {
    echo "Configured subscription is not found in the subscription list of current account, exiting"
    #exit
  }

  echo "Downloading setup templates..."
  $datetimestr = (Get-Date).ToString('yyyy-MM-dd-HH-mm-ss')
  $template_uri = $script:GITHUB_BASE_URL + "azuredeploy.private.json"
  $template_param_uri = $script:GITHUB_BASE_URL + "azuredeploy.parameters.json"
  $template = "$env:TEMP\mdcs-$datetimestr.json"
  $template_param = "$env:TEMP\mdcs-param-$datetimestr.json"
  $updated_template_param = "$env:TEMP\mdcs-param-updated-$datetimestr.json"

  Invoke-WebRequest $template_uri -Out $template
  Invoke-WebRequest $template_param_uri -Out $template_param

  echo "a few questions..."
  $rgname = readstring "ResourceGroup" $script:config["ClusterName"]
  $script:config["ClusterName"] = $rgname

  $location = readstring "Location" $script:config["Region"]
  $script:config["Region"] = $location

  $ClientVmSize = readstring "Client VM Size" $script:config["ClientVmSize"]
  $MJSVmSize = readstring "MJS VM Size" $script:config["MJSVmSize"]
  $WorkerVmSize = readstring "Worker VM Size" $script:config["WorkerVmSize"]
  $NumberWorkers = readstring "Number of Workers" $script:config["NumberWorkers"]
  $VmUsername = readstring "Admin Username on all VMs" $script:config["VmUsername"]

  $dnsname = readstring "Unique DNS name" $rgname

  $imageuri = readstring "The URL to the disk image in blob that will be used to create all VMs" $script:config["BaseVmVhd"]
  if(-not ($imageuri -eq $script:config["BaseVmVhd"])) {
    $script:config["BaseVmVhd"] = $imageuri
    $casteduri = [System.Uri]$imageuri
    $defaultcontainer = ("{0}://{1}/{2}/" -f $casteduri.Scheme, $casteduri.Host, $dnsname)
  } else {
    $defaultcontainer = $script:config["ClusterVmVhdContainer"]
  }
  $vhdcontainer = readstring "The URL of the container that will hold all VHDs for the VMs" $defaultcontainer

  echo "updating parameters for template deployment"
  (Get-Content $template_param) `
    -replace '\[\[location\]\]', $location `
    -replace '\[\[dnsName\]\]', $dnsname `
    -replace '\[\[imageUri\]\]', $imageuri `
    -replace '\[\[scriptUri\]\]', $script:GITHUB_BASE_URL `
    -replace '\[\[vhdContainer\]\]', $vhdcontainer `
    -replace '\[\[scaleNumber\]\]', $NumberWorkers `
    -replace '\[\[vmSizeClient\]\]', $ClientVmSize `
    -replace '\[\[vmSizeMJS\]\]', $MJSVmSize `
    -replace '\[\[vmSizeWorker\]\]', $WorkerVmSize `
    -replace '\[\[adminUserName\]\]', $VmUsername `
    -replace 'second regex', 'second replacement' |
  Out-File $updated_template_param

  echo "Creating resource group"
  New-AzureRmResourceGroup -Name $rgname -Location $location
  echo "Deploying to resource group. When this step is done, you will have a running MDCS cluster"
  New-AzureRmResourceGroupDeployment -ResourceGroupName $rgname -TemplateFile $template -TemplateParameterFile $updated_template_param
}

function mdcs_list($p) {
  function ListDeployment($name) {
    $results = @()
    Get-AzureRmResourceGroup | % {
      if(($name -ne $null) -and (-not $_.ResourceGroupName.Contains($name))) {
        #echo 'ignore'
      } else {
        $deployment = (Get-AzureRmResourceGroupDeployment -ResourceGroupName $_.ResourceGroupName)
        $keys = $deployment.Parameters.Keys
        if(($keys -ne $null) -and $keys.Contains('dnsName') -and $keys.Contains('vmSizeMJS') -and $keys.Contains('vmSizeClient') -and $keys.Contains('vmSizeWorker') -and $keys.Contains('scaleNumber')) {
          # now retrieve vm state
          $totalworkers = 0
          $workerstates = @{}
          $clientstate = ''
          $masterstate = ''
          Get-AzureRmVM -ResourceGroupName $_.ResourceGroupName | % {
            $vm = (Get-AzureRmVM -ResourceGroupName $_.ResourceGroupName -Name $_.Name -Status)
            if($vm.Name -eq 'client') {
              $clientstate = (($vmstate.Statuses | % {$_.displaystatus}) -join ', ')
            } elseif ($vm.Name -eq 'master') {
              $masterstate = (($vmstate.Statuses | % {$_.displaystatus}) -join ', ')
            } else {
              $totalworkers += 1
              $vm.Statuses | % {
                $workerstates[$_.Code] += 1
              }
            }
          }
          $vmstatus = ''
          $workerstates.Keys | % { $vmstatus += ("${_}: " + $workerstates[$_] + ' ') }
          $deployment | Add-Member -NotePropertyName Client -NotePropertyValue $clientstate
          $deployment | Add-Member -NotePropertyName Master -NotePropertyValue $masterstate
          $deployment | Add-Member -NotePropertyName Workers -NotePropertyValue $totalworkers
          $deployment | Add-Member -NotePropertyName WorkerStates -NotePropertyValue $vmstatus
          $results += $deployment
        } else {
          #echo 'skip'
        }
      }
    }
    return $results
  }
  $results = (ListDeployment $p[0])
  $results | format-table @{Expression={$_.ResourceGroupName + "/" + $_.DeploymentName}; Label="Name"}, @{Expression={$_.Client}; Label="Client"}, @{Expression={$_.Master}; Label="Master"}, @{Expression={$_.Workers}; Label="Workers"}, @{Expression={$_.WorkerStates}; Label="Worker Status"}
}

function mdcs_pause($p) {
  $rg = Get-AzureRmResourceGroup -ResourceGroupName $p[0]

  $scriptblock = {
    Param($profile, $rgname, $name)
    echo ("launch stopping job for - " + $name)
    Select-AzureRMProfile -Path $profile
    Stop-AzureRmVM -ResourceGroupName $rgname -Name $name -Force
  }

  if($rg -eq $null) {
    echo "nothing found, exiting"
  } else {
    # one thing we need to do before deleting resource group - find disk images so we can delete after the arg is deleted
    $deployment = (Get-AzureRmResourceGroupDeployment -ResourceGroupName $rg.ResourceGroupName)
    $vhds = @()
    if($deployment -eq $null) {
      echo "there is no deployment under the group, exiting..."
    } else {
      echo "deployment found, pausing VMs"
      $datetimestr = (Get-Date).ToString('yyyy-MM-dd-HH-mm-ss')
      $profilepath = "$env:TEMP\profile-$datetimestr.json"
      Save-AzureRmProfile -Path $profilepath
      $jobs = @()
      Get-AzureRmVM -ResourceGroupName $rg.ResourceGroupName | % {
        $jobs += Start-Job -ScriptBlock $scriptblock -ArgumentList  $profilepath, $rg.ResourceGroupName, $_.Name
      }
      Wait-Job -Job $jobs
    }
  }
}

function mdcs_resume($p) {
  $rg = Get-AzureRmResourceGroup -ResourceGroupName $p[0]

  $scriptblock = {
    Param($profile, $rgname, $name)
    echo ("launch starting job for - " + $name)
    Select-AzureRMProfile -Path $profile
    Start-AzureRmVM -ResourceGroupName $rgname -Name $name
  }

  if($rg -eq $null) {
    echo "nothing found, exiting"
  } else {
    # one thing we need to do before deleting resource group - find disk images so we can delete after the arg is deleted
    $deployment = (Get-AzureRmResourceGroupDeployment -ResourceGroupName $rg.ResourceGroupName)
    $vhds = @()
    if($deployment -eq $null) {
      echo "there is no deployment under the group, exiting..."
    } else {
      echo "deployment found, starting VMs"
      $datetimestr = (Get-Date).ToString('yyyy-MM-dd-HH-mm-ss')
      $profilepath = "$env:TEMP\profile-$datetimestr.json"
      Save-AzureRmProfile -Path $profilepath
      $jobs = @()
      Get-AzureRmVM -ResourceGroupName $rg.ResourceGroupName | % {
        $jobs += Start-Job -ScriptBlock $scriptblock -ArgumentList  $profilepath, $rg.ResourceGroupName, $_.Name
      }
      Wait-Job -Job $jobs
    }
  }
}

function mdcs_delete($p) {
  $rg = Get-AzureRmResourceGroup -ResourceGroupName $Args[0]

  if($rg -eq $null) {
    echo "nothing found, exiting"
  } else {
    # one thing we need to do before deleting resource group - find disk images so we can delete after the arg is deleted
    $deployment = (Get-AzureRmResourceGroupDeployment -ResourceGroupName $rg.ResourceGroupName)
    $vhds = @()
    if($deployment -eq $null) {
      echo "there is no deployment under the group, deleting the group directly"
    } else {
      echo "deployment found, finding VHDs... this may take a while"
      Get-AzureRmVM -ResourceGroupName $rg.ResourceGroupName | % {
        $vhds += $_.StorageProfile.OsDisk.Vhd.Uri
      }
    }

    # now delete the arg
    echo "deleting resource group..."
    Remove-AzureRmResourceGroup -ResourceGroupName $rg.ResourceGroupName -Force

    # now wait until it's deleted
    while($null -ne (Get-AzureRmResourceGroup -ResourceGroupName $rg.ResourceGroupName)) {
      echo "pause 5 seconds and check again if the resource group has disappeared..."
      Start-Sleep 5
      #break
    }

    # now it's time to delete the VHDs
    if($vhds.Count -gt 0) {
      echo "now deleting vhds..."
      # finding storage accounts from the url
      $storageaccountname = ([System.Uri]$vhds[0]).Authority.Split('.')[0]
      # getting storage context for deleting
      $storageaccount = (Get-AzureRmStorageAccount | ? {$_.StorageAccountName -eq $storageaccountname})
      $storagekey = Get-AzureRmStorageAccountKey -ResourceGroupName $storageaccount.ResourceGroupName -Name $storageaccount.StorageAccountName
      $storagecontext = New-AzureStorageContext -StorageAccountName $storageaccount.StorageAccountName -StorageAccountKey $storagekey.Key1

      $vhds | % {
        $vhd = $_
        # $blobname = $vhd parsed results
        # $containername = $vhd parsed results
        $segs = ([System.Uri]$vhd).AbsolutePath.Split('/')
        $containername = $segs[1]
        $blobname = [string]::Join('/', $segs[2..($segs.length-1)])
        echo "$vhd"
        echo "deleting $blobname of $containername on account $storageaccountname"
        Remove-AzureStorageBlob -Blob $blobname -Container $containername -Context $storagecontext -Force
      }
    }
  }
}

function usage($p) {
  echo "tell me something..."
}

function parse_param($p) {
  if($p.Count -eq 0) {
    usage
    exit
  }
  $command = $p[0]
  if($p.length -gt 1) {
    $command_args = $p[1..($p.length-1)]
  }
  switch ($p[0]) {
    "create" { mdcs_create($command_args); break }
    "list" { mdcs_list($command_args); break }
    "pause" { mdcs_pause($command_args); break }
    "resume" { mdcs_resume($command_args); break }
    "delete" { mdcs_delete($command_args); break }
    default { usage; exit}
  }
}

# parse_param will parse the parameter and drive the workflow
parse_param($args)
