import React, { Fragment, useEffect } from "react";
import { BrowserRouter, Route, Switch } from "react-router-dom";

import { connect } from "react-redux";
import { RoomState, Status } from "../../store/room/types";
import { AppState } from "../../store";
import { checkRoomExists, leaveRoom } from "../../store/room/actions";
import Greeting from "../Greeting/Greeting";
import CodeRoom from "../CodeRoom/CodeRoom";
import Error from "../Error/Error";

interface AppProps {
  status: Status;
  checkRoomExists: typeof checkRoomExists;
  leaveRoom: typeof leaveRoom;
}

const extractRoomId = () => {
  const path = window.location.pathname;
  const index = path.lastIndexOf("/");

  return index === -1 ? "" : path.substring(index + 1);
};

const ROOM_ID = extractRoomId();

const App: React.FC<AppProps> = ({ status, checkRoomExists, leaveRoom }) => {
  useEffect(() => {
    checkRoomExists(ROOM_ID);

    return () => {
      leaveRoom();
    };
  }, []);

  return (
    <Fragment>
      {status === Status.EXISTS && <Greeting roomId={ROOM_ID} />}
      {status === Status.JOINED && <CodeRoom />}
      {status === Status.ERROR && (
        <Error errorMessage="The room you tried to join does not exist" />
      )}
    </Fragment>
  );
};

const mapStateToProps = (state: AppState) => ({
  status: state.room.status,
});

export default connect(mapStateToProps, { checkRoomExists, leaveRoom })(App);
