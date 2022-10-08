dotnet publish /p:NativeLib=Shared /p:SelfContained=true -r win-x64 -c Release

copy .\bin\Release\net7.0\win-x64\publish\* ..\ConsoleApp1\bin\Debug\net6.0\win-x64\