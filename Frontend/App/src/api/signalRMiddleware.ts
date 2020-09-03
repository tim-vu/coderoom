import {Action, AppDispatch, AppState, AppStore} from "../store";
import * as signalR from "@microsoft/signalr";
import { HttpTransportType, HubConnectionState } from "@microsoft/signalr";
import {
  joinedRoom,
  languageChanged,
  leftRoom,
  textUpdated,
  typingUserChanged,
  roomError,
  roomExists, joinRoom,
} from "../store/room/actions";
import { Middleware} from "redux";
import { getLanguageByKey, Language, RoomVm, User } from "./types";
import {
  joinedGroupCall,
  receivedAnswerData,
  receivedOfferData,
  userJoined,
  userJoinedGroupCall,
  userLeft,
} from "../store/users/actions";
import {
  codeExecutionCompleted,
  codeExecutionStarted,
} from "../store/execution/actions";
import { BASE_URL } from "./codeRoomAPI";
import {toast} from "react-toastify";

const connection = new signalR.HubConnectionBuilder()
  .withAutomaticReconnect()
  .withUrl(BASE_URL + "/hubs/room", {
    transport: HttpTransportType.WebSockets,
  })
  .build();

const INTERESTING_ACTION_TYPES = [
  "CHECK_ROOM_EXISTS",
  "JOIN_ROOM",
  "JOIN_GROUP_CALL",
  "SEND_OFFER_DATA",
  "SEND_ANSWER_DATA",
  "LEAVE_ROOM",
  "UPDATE_TEXT",
  "CHANGE_LANGUAGE",
  "START_CODE_EXECUTION",
];

export const signalRMiddleware: Middleware<AppStore> = (
  store
) => (next) => async (action: Action) => {
  const { dispatch, getState } = store;

  if (!INTERESTING_ACTION_TYPES.includes(action.type)) {
    return next(action);
  }

  if (action.type === "CHECK_ROOM_EXISTS") {
    try {
      await initializeSignalR(dispatch, getState);
    } catch (e) {
      dispatch(roomError("Unable to establish a connection with the server"));
      console.log(e);
      return;
    }

    const exists = await connection.invoke<boolean>(
      "DoesRoomExist",
      action.roomId
    );

    if (!exists) {
      dispatch(roomError("The room you tried to join does not exist"));
      return;
    }

    dispatch(roomExists());
    return;
  }

  if (connection.state !== HubConnectionState.Connected) {
    return;
  }

  switch (action.type) {
    case "JOIN_ROOM":
      const room = await connection.invoke<RoomVm>(
        "JoinRoom",
        action.roomId,
        action.nickName
      );

      dispatch(joinedRoom(room, connection.connectionId as string));
      break;
    case "JOIN_GROUP_CALL":
      await connection.send("JoinGroupCall");
      dispatch(joinedGroupCall(action.stream));
      break;
    case "SEND_OFFER_DATA":
      await connection.invoke(
        "SendOfferData",
        action.connectionId,
        action.data
      );
      break;
    case "SEND_ANSWER_DATA":
      await connection.send("SendAnswerData", action.connectionId, action.data);
      break;
    case "LEAVE_ROOM":
      await connection.send("LeaveRoom");
      dispatch(leftRoom());
      break;
    case "UPDATE_TEXT":
      await connection.send("UpdateText", action.text);
      dispatch(textUpdated(action.text));
      break;
    case "CHANGE_LANGUAGE":
      await connection.send("ChangeLanguage", action.language.key, action.reset);
      dispatch(languageChanged(action.language));
      break;
    case "START_CODE_EXECUTION":
      await connection.send("StartCodeExecution");
      dispatch(codeExecutionStarted(undefined));
      break;
  }
};

const initializeSignalR = (dispatch: AppDispatch, getState : () => AppState) => {
  connection.on("OnUserJoined", (user: User) => {

    dispatch(userJoined(user));
  });

  connection.on("OnUserLeft", (connectionId: string) => {
    dispatch(userLeft(connectionId));
  });

  connection.on("OnTextChanged", (text: string) => {
    dispatch(textUpdated(text));
  });

  connection.on("OnTypingUserChanged", (connectionId: string | null) => {
    dispatch(typingUserChanged(connectionId));
  });

  connection.on("OnLanguageChanged", (language: number) => {
    dispatch(languageChanged(getLanguageByKey(language) as Language));
  });

  connection.on("OnCodeExecutionStarted", (message: string) => {
    dispatch(codeExecutionStarted(message));
  });

  connection.on("OnCodeExecutionCompleted", (output: string) => {
    dispatch(codeExecutionCompleted(output));
  });

  connection.on("OnOfferDataReceived", (connectionId: string, data: string) => {
    dispatch(receivedOfferData(connectionId, data));
  });

  connection.on(
    "OnAnswerDataReceived",
    (connectionId: string, data: string) => {
      dispatch(receivedAnswerData(connectionId, data));
    }
  );

  connection.on("OnUserJoinedGroupCall", (connectionId: string) => {
    dispatch(userJoinedGroupCall(connectionId));
  });

  connection.onclose((error) => {

    dispatch(roomError("The connection to the server closed unexpectedly"));

    if (error) {
      console.log(error);
    }
  });

  connection.onreconnecting(() => {
    toast.error("The connection to the server has been lost, trying to reconnect", {
      autoClose: false,
      toastId: 'CONNECTION_LOST'
    })
  });

  connection.onreconnected(() => {

    toast.dismiss('CONNECTION_LOST');
    toast.info('The connection to the server has been restored, rejoining the room');

    const roomId = getState().room.id as string;
    const nickName = getState().user.nickname as string;
    
    dispatch(joinRoom(roomId, nickName))
  });

  return connection.start();
};
