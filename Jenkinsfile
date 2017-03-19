pipeline {
    agent any

    stages {
	
		stage('Restore') {
            steps {
                bat '"C:\Users\PC-Samy\Documents\Nuget\Nuget.exe" restore ".\\HolidayPooling\\HolidayPooling.sln"'
            }
        }
        stage('Build') {
            steps {
                bat '"C:\\Program Files (x86)\\MSBuild\\14.0\\Bin\\MSBuild.exe"  ".\\HolidayPooling\\HolidayPooling.sln" /t:Rebuild'
            }
        }
    }
}