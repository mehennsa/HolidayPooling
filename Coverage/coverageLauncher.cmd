"..\HolidayPooling\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" -target:testLauncher.cmd -register:user
"..\HolidayPooling\packages\ReportGenerator.2.5.2\tools\ReportGenerator.exe" -reports:results.xml -targetdir:CoverageResult
"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" "CoverageResult\index.htm"