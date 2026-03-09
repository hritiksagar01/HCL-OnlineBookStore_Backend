agent any

    stages {
        stage('Clone') {
            steps {
                git branch: 'main', url: 'https://github.com/hritiksagar01/HCL-OnlineBookStore_Backend.git'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build'
            }
        }

        stage('Test') {
            steps {
                bat 'dotnet test'
            }
        }

        stage('Publish') {
            steps {
                bat 'dotnet publish -c Release -o publish'
            }
        }
    }
}
