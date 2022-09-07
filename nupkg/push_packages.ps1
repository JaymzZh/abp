. ".\common.ps1"

$apiKey = $args[0]

# Get the version
[xml]$commonPropsXml = Get-Content (Join-Path $rootFolder "common.props")
$version = $commonPropsXml.Project.PropertyGroup.Version

# Publish all packages
Get-ChildItem $packFolder -Filter *.nupkg |
ForEach-Object {
    & dotnet nuget push $_.FullName -s http://172.16.0.142:2020/repository/nuget-hosted/ --api-key "$apiKey"
}
# foreach($project in $projects) {
#     $projectName = $project.Substring($project.LastIndexOf("/") + 1)
#     & dotnet nuget push ($projectName + "." + $version + ".nupkg") -s http://172.16.0.142:2020/repository/nuget-hosted/ --api-key "$apiKey"
# }

# Go back to the pack folder
Set-Location $packFolder
