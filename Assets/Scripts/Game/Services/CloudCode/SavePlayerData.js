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


        //Check if the cloud save exists 

        const resultRemoteConfig = await getRemoteConfigData(remoteConfig, projectId, playerId, environmentId);
        //logger.debug(resultRemoteConfig);

        const saveUserResult = await cloudSave.setItem(projectId, playerId, {
            key: "userData",
            value: resultRemoteConfig
        });
        //logger.debug(saveUserResult);

        const result = {
            key: "Key response",
            value: "Value response"
        };

        return result;

    } catch (error) {
        // TODO: add to analytics events
        //logger.debug(error);
        const result = {
            key: "Error",
            value: "During execution"
        };
        return result;
    }
};

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