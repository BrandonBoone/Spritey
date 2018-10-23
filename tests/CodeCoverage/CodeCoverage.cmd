@echo off

nuget restore packages.config -PackagesDirectory .

cd ..
cd ..

dotnet restore Spritey.sln
rem Clean the solution to force a rebuild with /p:codecov=true
dotnet clean Spritey.sln -c Release
rem The -threshold options prevents this taking ages...
tests\CodeCoverage\OpenCover.4.6.519\tools\OpenCover.Console.exe -target:"dotnet.exe" -targetargs:"test tests\Spritey.Test\Spritey.Test.csproj -c Release -f netcoreapp2.1 /p:codecov=true" -register:user -threshold:10 -oldStyle -safemode:off -output:.\Spritey.Coverage.xml -hideskipped:All -returntargetcode

if %errorlevel% neq 0 exit /b %errorlevel%

tests\CodeCoverage\Codecov.1.1.0\tools\codecov.exe -f Spritey.Coverage.xml -t %CODECOV_TOKEN%
