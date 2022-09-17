pipeline {
    agent { 
        node {
            label "master"
            customWorkspace "/Users/macbookpro/work/Repositories/Jenkins/"
        }
    }
    environment {
        UNITY_PATH = "/Applications/Unity/Hub/Editor/2022.1.16f1/Unity.app/Contents/MacOS/Unity"
        PROJECT_PATH = "/Users/macbookpro/Work/Repositories/UnityProject/"
        BUILD_ID = "${env.BUILD_ID}"
        JENKINS_URL = "${env.JENKINS_URL}"
    }
    
    stages {
        stage('Test EditTests') {
            steps {
                echo 'EditTests..'
            }
        } 

       stage('Test PlayModeTests') {
            steps {
                echo 'PlayModeTests..'
            }
        }
        stage('Build IOS') {
            steps { 

                echo 'Bulding....'
                echo "Running ${BUILD_ID} on ${JENKINS_URL}"
                sh "${UNITY_PATH} -quit -batchmode -projectPath ${PROJECT_PATH} -executeMethod BuildScript.PerformIOSBuild -stackTraceLogType Full -logfile"
            }
        }
        stage('Build Android') {
            steps { 
                echo 'Bulding....'
                echo "Running ${BUILD_ID} on ${JENKINS_URL}"
                sh "${UNITY_PATH} -quit -batchmode -projectPath ${PROJECT_PATH} -executeMethod BuildScript.PerformAndroidBuild -stackTraceLogType Full -logfile"
                // /Applications/Unity/Hub/Editor/2022.1.16f1/Unity.app/Contents/MacOS/Unity -quit -batchmode -projectPath . -executeMethod BuildScript.PerformAndroidBuild -stackTraceLogType Full -logfile ~/Unity/Unity.log
            }
        }
    }                       
}