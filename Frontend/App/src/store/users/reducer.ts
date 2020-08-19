import { UserActions, UserState } from "./types";
import { ById, toById } from "../byid";
import { User } from "../../api/types";
import produce from "immer";

const initialState: UserState = {
  users: [],
  me: null,
  nickname: null,
};

export function userReducer(state = initialState, action: UserActions) {
  switch (action.type) {
    case "JOINED_ROOM":
      return produce(state, (draft) => {
        draft.users = action.room.users
          .filter((u) => u.connectionId !== action.connectionId)
          .map((u) => <User>u);
        draft.me = <User>(
          action.room.users.find((u) => u.connectionId === action.connectionId)
        );
        draft.nickname = draft.me.nickName;
      });
    case "JOINED_GROUP_CALL":
      return produce(state, (draft) => {
        // @ts-ignore
        draft.me.inGroupCall = true;
        // @ts-ignore
        draft.me.stream = action.stream;
      });
    case "STREAM_CONNECTED":
      return produce(state, (draft) => {
        draft.users
          .filter((u) => u.connectionId === action.connectionId)
          .forEach((u) => (u.stream = action.stream));
      });
    case "USER_JOINED_GROUP_CALL":
      return produce(state, (draft) => {
        draft.users
          .filter((u) => u.connectionId === action.connectionId)
          .forEach((u) => (u.inGroupCall = true));
      });
    case "USER_LEFT_GROUP_CALL":
      return produce(state, (draft) => {
        draft.users
          .filter((u) => u.connectionId === action.connectionId)
          .forEach((u) => {
            u.inGroupCall = false;
            u.stream = undefined;
          });
      });
    case "LEFT_ROOM":
      return initialState;
    case "USER_JOINED":
      return produce(state, (draft) => {
        draft.users.push(action.user);
      });
    case "USER_LEFT":
      return produce(state, (draft) => {
        draft.users = draft.users.filter(
          (u) => u.connectionId !== action.connectionId
        );
      });
    default:
      return state;
  }
}
