# OptimaValue
Run in powershell
Create service:
$params = @{
  Name = "Optima"
  BinaryPathName = '"C:\Program Files\Optima\Optima\OptimaValue.Service.exe"'
  DisplayName = "OptimaValue"
  StartupType = "Manual"
  Description = "Optima databatchloggning för Anläggningsdata"
}
New-Service @params
