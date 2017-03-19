pipeline {
    agent any

    stages {
        stage('Build') {
            steps {
                bat '"C:\\Program Files (x86)\\MSBuild\\14.0\\Bin\\MSBuild.exe" "E:\\Project\\HolidayPooling\\HolidayPooling" /t:Rebuild'
            }
        }
    }
}