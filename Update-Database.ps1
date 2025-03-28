   
	$ProjectName = 'Application'
	$StartUpProjectName = 'API'

dotnet ef database update --startup-project=$StartUpProjectName --project=$ProjectName