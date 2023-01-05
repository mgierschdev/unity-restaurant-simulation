const _ = require("lodash-4.17");
const {
    CloudSave
} = require("@unity-services/cloud-save-1.2");

module.exports = async ({ params, context, logger }) => {
    const {
        projectId,
        playerId
    } = context;

    const cloudSave = new CloudSave(context);

    const setResult = await cloudSave.setItem(projectId, playerId, {
        key: "TestSave",
        value: "TesteSaveValue"
    });
    
    logger.debug(JSON.stringify(setResult.data));

    const result = {
        key: "SAVED",
        value: "TRUE"
    };

    return result;
};