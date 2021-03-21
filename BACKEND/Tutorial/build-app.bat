cd src\PublicApi
dotnet build -c Release -o out
dotnet publish --no-restore -c Release -o out
