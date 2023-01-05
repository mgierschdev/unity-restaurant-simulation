const _ = require("lodash-4.17");
const { DataApi } = require("@unity-services/cloud-save-1.2");
const { SettingsApi } = require("@unity-services/remote-config-1.1");

const userVersion = "defaultuser.1.0.0";

module.exports = async ({ params, context, logger }) => {
    try {

        const {
            projectId,
            environmentId,
            playerId
        } = context;

        const remoteConfig = new SettingsApi(context);
        const cloudSave = new DataApi(context);
        var responseValue = 2;

        //Check if the cloud save exists 
        const resultUserData = await getCloudSaveUserData(cloudSave, projectId, playerId);

        if (resultUserData == "") {

            logger.debug("create new player");
            const resultRemoteConfig = await getRemoteConfigData(remoteConfig, projectId, playerId, environmentId);
            //logger.debug(resultRemoteConfig);

            for (var localKey in resultRemoteConfig) {
                // logger.debug(key +" "+resultRemoteConfig[key]);
                await cloudSave.setItem(projectId, playerId,
                    {
                        key: localKey,
                        value: resultRemoteConfig[localKey]
                    });
            }

            logger.debug(dataBatch);



            responseValue = 1;

            logger.debug(saveUserResult.data);
        } else {
            response = 3;
        }


        // response values from CloudCodeGetPlayerDataResponse()
        const result = {
            key: "Response",
            value: responseValue
        };

        return result;

    } catch (error) {

        logger.debug(error);

        const result = {
            key: "Error",
            value: "During execution"
        };
        return result;
    }
};

async function getCloudSaveUserData(cloudSave, projectId, playerId) {
    const getCloudSaveUserDataResponse = await cloudSave.getItems(
        projectId,
        playerId,
        [
            "NAME",
            "VERSION"
        ]
    );

    return getCloudSaveUserDataResponse.data.results;
}

async function getRemoteConfigData(remoteConfig, projectId, playerId, environmentId) {
    const getRemoteConfigDataSettingsResponse = await remoteConfig.assignSettings({
        projectId,
        environmentId,
        "userId": playerId,
        // associate the current timestamp with the user in Remote Config to affect which season Game Override we get
        "attributes": {
            "unity": {},
            "app": {},
            "user": {
                "timestampMinutes": _.now() //current server time
            },
        },
        "key": [userVersion]
    });

    if (getRemoteConfigDataSettingsResponse.data &&
        getRemoteConfigDataSettingsResponse.data.configs &&
        getRemoteConfigDataSettingsResponse.data.configs.settings) {
        return getRemoteConfigDataSettingsResponse.data.configs.settings[userVersion];
    }

    throw new CloudCodeError("could not load cloud config");
}

class CloudCodeError extends Error {
    constructor(message) {
        super(message);
        this.name = "CloudCodeCustomError";
        this.status = 1;
    }
}