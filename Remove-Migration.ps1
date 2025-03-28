[CmdletBinding(DefaultParameterSetName = 'Name')]
    param(
        [string] $Name)

    
	$ProjectName = 'Application'
	$StartUpProjectName = 'API'

dotnet ef migrations remove --startup-project=$StartUpProjectName --project=$ProjectName