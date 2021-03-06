import { handleActions, combineActions } from "redux-actions";

import * as actions from "../actions/queuesActions";
import * as eventsActions from "../actions/eventsActions";
import * as stateActions from "../../../state/actions/sessionActions";
import * as requestsActions from "../../requests/actions";

const initialState = {};

export default handleActions(
    {
        [stateActions.resetState]: (state, action) => {
            return { ...initialState };
        },
        [eventsActions.getEventsSuccess]: (state, action) => {
            const newState = { ...initialState };
            action.response.data.forEach(
                event => (newState[event.queueId] = event.queue)
            );
            return { ...newState };
        },

        [actions.takeQueueRequest]: (state, action) => {
            return setIsLoading(state, action);
        },
        [actions.takeQueueFailed]: (state, action) => {
            return resetIsLoading(state, action);
        },
        [actions.takeQueueSuccess]: (state, action) => {
            return resetIsLoading(state, action);
        },

        [actions.leaveQueueRequest]: (state, action) => {
            return setIsLoading(state, action);
        },
        [actions.leaveQueueFailed]: (state, action) => {
            return resetIsLoading(state, action);
        },
        [actions.leaveQueueSuccess]: (state, action) => {
            return resetIsLoading(state, action);
        },
        [actions.getQueueRequest]: (state, action) => {
            const { queueId } = action.payload;

            const newState = { ...state };
            newState[queueId] = { ...newState[queueId], isLoading: true };

            return newState;
        },
        [actions.getQueueSuccess]: (state, action) => {
            const newState = { ...state };
            const newQueue = action.response.data;

            newState[newQueue.id] = {
                ...state[newQueue.id],
                ...newQueue,
                isLoading: false
            };

            return { ...newState };
        },

        [actions.removeUserFromQueue]: (state, action) => {
            const { queueId, userId } = action.payload;

            const newState = { ...state };
            let newUserQueue = [...newState[queueId].userQueues];
            newUserQueue = newUserQueue.filter(uq => uq.userId !== userId);
            newState[queueId] = {
                ...newState[queueId],
                userQueues: newUserQueue
            };

            return newState;
        },
        [actions.addUserToQueue]: (state, action) => {
            const data = action.payload;
            const { queueId, userId } = data;

            const newState = { ...state };
            let newUserQueue = newState[queueId].userQueues.filter(
                uq => uq.userId !== userId
            );
            newUserQueue.push(data);

            newState[queueId] = {
                ...newState[queueId],
                userQueues: newUserQueue
            };

            return newState;
        },
        [actions.swapUsers]: (state, action) => {
            const data = action.payload;
            const { queueId, creatorUserId, receiverUserId } = data;

            if (!state[queueId]) {
                return { ...state };
            }

            let newUserQueue = [...state[queueId].userQueues];
            let creatorUserIndex = newUserQueue.findIndex(
                uq => uq.userId === creatorUserId
            );
            let receiverUserIndex = newUserQueue.findIndex(
                uq => uq.userId === receiverUserId
            );

            if (creatorUserIndex === -1 || receiverUserIndex === -1) {
                return { ...state };
            }

            const tempUser = { ...newUserQueue[creatorUserIndex] };
            newUserQueue[creatorUserIndex] = {
                ...newUserQueue[receiverUserIndex]
            };
            newUserQueue[receiverUserIndex] = tempUser;

            const newState = { ...state };

            newState[queueId] = {
                ...newState[queueId],
                userQueues: newUserQueue
            };

            return newState;
        },
        [combineActions(requestsActions.createSwapRequestRequest, requestsActions.updateRequestRequest)]: (state, action) => {
            const data = action.payload;
            const { queueId, receiverUserId, creatorUserId } = data;
            const searchedUserId = data.state === 2 ? creatorUserId : receiverUserId

            const newState = { ...state };
            let userIndex = newState[queueId].userQueues.findIndex(
                uq => uq.userId === searchedUserId
            );
            if (userIndex === -1) {
                return { ...state };
            }
            
            const newUserQueue = [...newState[queueId].userQueues];
            newUserQueue[userIndex] = {
                ...newUserQueue[userIndex],
                isLoading: true
            };

            newState[queueId] = {
                ...newState[queueId],
                userQueues: newUserQueue
            };

            return newState;
        },
        [combineActions(requestsActions.createSwapRequestSuccess, requestsActions.updateRequestSuccess)]: (state, action) => {
            const data = action.payload;
            const { queueId, receiverUserId, creatorUserId } = data;
            const searchedUserId = data.state === 2 ? creatorUserId : receiverUserId

            const newState = { ...state };
            let userIndex = newState[queueId].userQueues.findIndex(
                uq => uq.userId === searchedUserId
            );
            if (userIndex === -1) {
                return { ...state };
            }
            
            const newUserQueue = [...newState[queueId].userQueues];
            newUserQueue[userIndex] = {
                ...newUserQueue[userIndex],
                isLoading: false
            };

            newState[queueId] = {
                ...newState[queueId],
                userQueues: newUserQueue
            };

            return newState;
        }
    },
    initialState
);

function setIsLoading(state, action) {
    const { queueId } = action.payload;

    const newState = { ...state };
    newState[queueId] = { ...newState[queueId], isLoading: true };

    return newState;
}

function resetIsLoading(state, action) {
    const { queueId } = action.payload;

    const newState = { ...state };
    newState[queueId] = { ...newState[queueId], isLoading: false };

    return newState;
}
