dotnet pack .\Rampastring.XNAUI.csproj -c WindowsDXRelease
dotnet pack .\Rampastring.XNAUI.csproj -c UniversalGLRelease
dotnet pack .\Rampastring.XNAUI.csproj -c WindowsGLRelease
dotnet pack .\Rampastring.XNAUI.csproj -c WindowsXNARelease -p:PlatformTarget=x86

dotnet pack .\Rampastring.XNAUI.csproj -c WindowsDXDebug
dotnet pack .\Rampastring.XNAUI.csproj -c UniversalGLDebug
dotnet pack .\Rampastring.XNAUI.csproj -c WindowsGLDebug
dotnet pack .\Rampastring.XNAUI.csproj -c WindowsXNADebug -p:PlatformTarget=x86