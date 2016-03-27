# client vm setup script - run on client vm within a private cluster

function trace() {
    param(
    [Parameter(
        Position=0,
        Mandatory=$true,
        ValueFromPipeline=$true)
    ]
    [String[]]$log
    )

    filter timestamp {"$(Get-Date -Format 'yyyy/MM/dd HH:mm:ss.fff'): $_"}
    if((Test-Path variable:logfile) -eq $false)
    {
        $datetimestr = (Get-Date).ToString('yyyy-MM-dd-HH-mm-ss')
        $script:logfile = "$env:windir\Temp\MDCSLog-$datetimestr.log"
    }
    $log | timestamp | Out-File -Confirm:$false -FilePath $script:logfile -Append
}

echo "client start" | trace
$MyInvocation | Out-String | trace

# create folder
mkdir "C:\Program Files\MATLAB\R2015aSP1\licenses"

# touch license file
$myString = @"
<?xml version="1.0" encoding="UTF-8"?>
<root>
    <ActivationEntry hostname="*" idnumber="1"
        matlabroot="*" user="*">
        <licmode>online</licmode>
    </ActivationEntry>
</root>
"@
$myString | Out-File -Encoding ascii "C:\Program Files\MATLAB\R2015aSP1\licenses\license_info.xml"

#create shortcut
$TargetFile = "C:\Program Files\MATLAB\R2015aSP1\bin\matlab.exe"
$ShortcutFile = "C:\Users\Public\Desktop\MATLAB.lnk"
$WScriptShell = New-Object -ComObject WScript.Shell
$Shortcut = $WScriptShell.CreateShortcut($ShortcutFile)
$Shortcut.TargetPath = $TargetFile
$Shortcut.Save()

mkdir "c:\mdcsshare"
net share mdcsshare=c:\mdcsshare /remark:"mdcs cluster data staging share" /grant:everyone,FULL
