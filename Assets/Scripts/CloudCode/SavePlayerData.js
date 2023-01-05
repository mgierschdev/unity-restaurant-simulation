const _ = require("lodash-4.17");
const {
    DataApi
} = require("@unity-services/cloud-save-1.2");

module.exports = async ({ params, context, logger }) => {
    
    const {
        projectId,
        playerId
    } = context;

    const api = new DataApi(context);

    const setResult = await api.setItem(projectId, playerId, {
        key: "test",
        value: "testValue"
    });

    logger.debug(JSON.stringify(setResult.data));

    const result = {
        key: "SAVED",
        value: "TRUE"
    };

    return result;
};