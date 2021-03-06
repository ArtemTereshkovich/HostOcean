import { reducer as formReducer } from "redux-form";
import { combineReducers } from "redux";
import { connectRouter } from "connected-react-router";

import ping from "./../../modules/account/reducers/pingReducer";
import groups from "./../../modules/groups/reducers/groupsReducer";
import session from "./sessionReducer";
import config from "./configReducer";
import user from "./userReducer";
import statistic from "../../modules/statistics/reducers/statisticsReducer";
import statisticsFormReducer from './../../modules/statistics/reducers/statisticsFormReducer';
import signalr from "./signalrReducer";
import mainpage from "../../modules/mainpage/reducers";
import registerFormReducer from "../../modules/account/reducers/registerFormReducer";
import loginFormReducer from "../../modules/account/reducers/loginFormReducer";
import userInfoReducer from "../../modules/account/reducers/userInfoReducer";
import ui from "../../modules/ui/reducers";
import requests from "../../modules/requests/reducers";
import app from "../../modules/notifications/reducers";

export default history =>
    combineReducers({
        router: connectRouter(history),
        form: formReducer.plugin({
            register: registerFormReducer,
            login: loginFormReducer,
            userInfo: userInfoReducer,
            statistics: statisticsFormReducer,
        }),
        ping,
        groups,
        session,
        user,
        signalr,
        config,
        mainpage,
        app,
        ui,
        requests,
        statistic
    });
