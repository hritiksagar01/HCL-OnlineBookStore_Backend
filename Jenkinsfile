pipeline {
    agent any

    stages {
        stage('Clone Code') {
            steps {
                // Using 'main' as the default branch to avoid revision errors
                git branch: 'main', url: 'https://github.com/hritiksagar01/HCL-OnlineBookStore_Backend.git'
            }
        }

        stage('Build') {
            steps {
                echo 'Building the .NET Project...'
                bat 'dotnet build'
            }
        }

        stage('Test') {
            steps {
                echo 'Running Unit Tests...'
                // This will fail the build if tests fail (Brute Force approach)
                bat 'dotnet test'
            }
        }

        stage('Publish') {
            steps {
                echo 'Publishing the Artifacts...'
                // Optimized: Release configuration with output to a specific folder
                bat 'dotnet publish -c Release -o publish'
            }
        }

        stage('Archive') {
            steps {
                echo 'Archiving build artifacts...'
                // This saves the published files so you can download them from Jenkins
                archiveArtifacts artifacts: 'publish/**', fingerprint: true
            }
        }
    }

    post {
        always {
            echo 'Pipeline execution finished.'
        }
        success {
            echo 'Build and Test passed successfully!'
        }
        failure {
            echo 'Build failed. Check the console output above for errors.'
        }
    }
}
