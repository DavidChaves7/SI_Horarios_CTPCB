[CmdletBinding(DefaultParameterSetName = 'Name')]
    param(
        [string] $Name)

    
	$ProjectName = 'Application'
	$StartUpProjectName = 'API'
	
	if ($Name -eq $null -or $Name -eq '') {
		throw "Migration Name is Required"
	}

dotnet ef migrations add $Name --startup-project=$StartUpProjectName --project=$ProjectName