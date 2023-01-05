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
        value:
        {
            key1: "test",
            key2: "test",
            key3: {
                key1: 5,
                key2: 10,
                key3: "test"
            }
        }
    });

    logger.debug(JSON.stringify(setResult.data));

    const result = {
        key: "SAVED",
        value: "TRUE"
    };

    return result;
};