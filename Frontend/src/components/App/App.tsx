import React from "react";
import { BrowserRouter, Route, Switch } from "react-router-dom";

import "../../styles/app.css";

import CodeRoom from "../CodeRoom/CodeRoom";
import Home from "../Home/Home";
import { leaveRoom } from "../../store/room/actions";
import { connect } from "react-redux";

interface AppProps {
  leaveRoom: typeof leaveRoom;
}

const App: React.FC<AppProps> = ({ leaveRoom }) => {
  return (
    <BrowserRouter>
      <Switch>
        <Route
          path="/:roomId(\d+)"
          exact={true}
          component={CodeRoom}
          onLeave={() => leaveRoom()}
        />
        <Route path="/" component={Home} />
      </Switch>
    </BrowserRouter>
  );
};

export default connect(() => ({}), { leaveRoom })(App);
