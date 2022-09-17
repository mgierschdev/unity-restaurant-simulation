pipeline {
    agent { 
        node {
            label "master"
            customWorkspace "/Users/macbookpro/work/Repositories/Jenkins/"
        }
    }
    environment {
        UNITY_PATH = "/Applications/Unity/Hub/Editor/2022.1.16f1/Unity.app/Contents/MacOS/Unity"
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
                sh "${UNITY_PATH} -quit -batchmode -projectPath . -executeMethod  BuildScript.PerformIOSBuild -stackTraceLogType Full --silent-crashes -logFile 'Unity/jenkinsAndroid.log'"
            }
        }
        stage('Build Android') {
            steps { 
                echo 'Bulding....'
                echo "Running ${BUILD_ID} on ${JENKINS_URL}"
                sh "${UNITY_PATH} -quit -batchmode -projectPath . -executeMethod BuildScript.PerformAndroidBuild -stackTraceLogType Full --silent-crashes -logFile 'Unity/jenkinsIOS.log'"
                        
                //  bat "\"${UNITY_PATH}\" -batchmode -projectPath . -executeMethod BuildScript.PerformBuild -logFile build.log"
                // /Applications/Unity/Hub/Editor/2021.3.5f1/Unity.app/Contents/MacOS/Unity -projectPath '/Users/macbookpro/work/Repositories/TestUnityProject' -executeMethod BuildScript.PerformBuild -logFile 'build.log' 
            }
        }
    }                       
}