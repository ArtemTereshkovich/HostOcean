import React from "react";
import PropTypes from "prop-types";

import { connect } from "react-redux";
import { Redirect, Route } from "react-router";

const AuthorizedControl = ({ component: Component, isAuthorized, ...rest }) => {
  return (
    <Route
      {...rest}
      render={props =>
        isAuthorized ?
          <Component {...props} />
          : <Redirect to="/accounts/login" />
      }
    />);
};

AuthorizedControl.propTypes = {
  children: PropTypes.any,
  isAuthorized: PropTypes.bool
};

const mapStateToProps = state => {
  const isExpired = new Date(state.session.expires) < new Date();
  return {
    isAuthorized: Boolean(state.user.id && state.session.accessToken && !isExpired)
  }
};

export default connect(mapStateToProps)(AuthorizedControl);