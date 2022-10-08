@echo off

set CORECLR_ENABLE_PROFILING=1
set CORECLR_PROFILER={A2F114DC-F5ED-4AAE-B259-F5A3D9EF5C42}
set CORECLR_PROFILER_PATH=..\Profiler\bin\Release\net7.0\win-x64\publish\Profiler.dll

.\bin\Debug\net6.0\ConsoleApp1.exe
