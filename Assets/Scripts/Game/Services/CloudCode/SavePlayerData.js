const _ = require("lodash-4.17");
const { DataApi } = require("@unity-services/cloud-save-1.2");
const { SettingsApi } = require("@unity-services/remote-config-1.1");

const userVersion = "defaultuser.1.0.0";

module.exports = async ({ params, context, logger }) => {
    // try {

    const {
        projectId,
        environmentId,
        playerId
    } = context;

    // remote config 
    const remoteConfig = new SettingsApi(context);
    const resultRemoteConfig = await getRemoteConfigData(remoteConfig, projectId, playerId, environmentId);
    logger.debug(projectId + " " + " " + environmentId + " ");
    logger.debug(resultRemoteConfig.data);


    // // Cloud save
    // const api = new DataApi(context);
    // const setResult = await api.setItem(projectId, playerId, {
    //     key: "test",
    //     value:
    //     {
    //         key1: "test",
    //         key2: "test",
    //         key3: {
    //             key1: 5,
    //             key2: 10,
    //             key3: "test"
    //         }
    //     }
    // });
    // logger.debug(JSON.stringify(setResult.data));

    const result = {
        key: "Key response",
        value: "Value response"
    };

    return result;


    // } catch (error) {

    //     logger.debug(error);

    //     const result = {
    //         key: "Key error response",
    //         value: "Value error response"
    //     };
    //     return result;
    // }
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
                "timestampMinutes": _.now()//current server time
            },
        },
        "key": [userVersion]
    });


    return getRemoteConfigDataSettingsResponse;

    // if (getRemoteConfigDataSettingsResponse.data.configs &&
    //   getRemoteConfigDataSettingsResponse.data.configs.settings
    // ) {
    //   return getRemoteConfigDataSettingsResponse;
    // }

    // throw new CloudCodeError("could not load cloud config");
}

class CloudCodeError extends Error {
    constructor(message) {
        super(message);
        this.name = "CloudCodeCustomError";
        this.status = 1;
    }
}