import { Action, AppState } from "../store";
import * as signalR from "@microsoft/signalr";
import { HttpTransportType, HubConnectionState } from "@microsoft/signalr";
import {
  joinedRoom,
  languageChanged,
  leftRoom,
  textUpdated,
  typingUserChanged,
} from "../store/room/actions";
import { Middleware, MiddlewareAPI, Store } from "redux";
import { getLanguageByKey, Language, Room, User } from "./types";
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

const connection = new signalR.HubConnectionBuilder()
  .configureLogging(signalR.LogLevel.Debug)
  .withAutomaticReconnect()
  .withUrl("http://localhost:5000/hubs/room", {
    transport: HttpTransportType.WebSockets,
  })
  .build();

export const signalRMiddleware: Middleware<Store<AppState, Action>> = (
  store
) => (next) => async (action: Action) => {
  const { dispatch } = store;

  switch (action.type) {
    case "JOIN_ROOM":
      if (connection.state !== HubConnectionState.Connected) {
        try {
          await initializeSignalR(store);
        } catch (e) {
          console.error(e);
          return;
        }
      }

      const room = await connection.invoke<Room>(
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
      await connection.send("ChangeLanguage", action.language.key);
      dispatch(languageChanged(action.language));
      break;
    case "START_CODE_EXECUTION":
      await connection.send("StartCodeExecution");
      dispatch(codeExecutionStarted(undefined));
      break;
    default:
      return next(action);
  }
};

const initializeSignalR: (store: MiddlewareAPI) => void = ({
  getState,
  dispatch,
}) => {
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

  return connection.start();
};
