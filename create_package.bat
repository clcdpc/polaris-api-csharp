nuget pack src\polaris-api-csharp\polaris-api-csharp.csproj -OutputDirectory packages -Symbols
FOR /F "delims=|" %%I IN ('DIR "packages\*.nupkg" /B /O:D') DO SET NewPackage=%%I
nuget push packages\%NewPackage% -Source https://www.nuget.org/api/v2/package
pause