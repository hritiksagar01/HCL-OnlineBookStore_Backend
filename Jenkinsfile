pipeline {
    agent any

    stages {
        stage('Clone') {
            steps {
                // Ensure we are pulling the correct branch
                git branch: 'main', url: 'https://github.com/hritiksagar01/HCL-OnlineBookStore_Backend.git'
            }
        }

        stage('Build') {
            steps {
                echo 'Building the application...'
                bat 'dotnet build'
            }
        }

        stage('Test') {
            steps {
                echo 'Running Unit Tests...'
                // Use 'continue' logic if you don't want the build to fail on test errors
                bat 'dotnet test'
            }
        }

        stage('Publish') {
            steps {
                echo 'Publishing the project...'
                bat 'dotnet publish -c Release -o publish'
            }
        }
    }
}