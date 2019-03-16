import { reducer as formReducer } from "redux-form";
import { combineReducers } from "redux";
import { connectRouter } from 'connected-react-router'

import ping from './../../modules/account/reducers/pingReducer';
import groups from './../../modules/groups/reducers/groupsReducer';
import session from "./sessionReducer";
import config from "./configReducer";
import mainpage from "../../modules/mainpage/reducers";
import registerFormReducer from "../../modules/account/reducers/registerFormReducer";
import loginFormReducer from "../../modules/account/reducers/loginFormReducer";


export default (history) => combineReducers({
    router: connectRouter(history),
    form: formReducer.plugin({
        register: registerFormReducer,
        login: loginFormReducer
    }),
    ping,
    groups,
    session,
    config,
    mainpage
})
