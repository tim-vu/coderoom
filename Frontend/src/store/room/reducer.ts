import { RoomActions, RoomState, Status } from "./types";
import produce from "immer";

const initialState: RoomState = {
  id: null,
  status: Status.CONNECTING,
  error: null,
  text: "",
  typingUserConnectionId: null,
};

export function roomReducer(
  state = initialState,
  action: RoomActions
): RoomState {
  switch (action.type) {
    case "JOINED_ROOM":
      return produce(state, (draft) => {
        draft.status = Status.JOINED;
        draft.id = action.room.id;
        draft.text = action.room.text;
        draft.typingUserConnectionId = action.room.typingUserConnectionId;
      });
    case "ROOM_EXISTS":
      return produce(state, (draft) => {
        draft.status = Status.EXISTS;
      });
    case "ROOM_ERROR":
      return produce(state, (draft) => {
        draft.error = action.error;
        draft.status = Status.ERROR;
      });
    case "TEXT_UPDATED":
      return produce(state, (draft) => {
        draft.text = action.text;
      });
    case "TYPING_USER_CHANGED":
      return produce(state, (draft) => {
        draft.typingUserConnectionId = action.connectionId;
      });
    case "LEFT_ROOM":
      return produce(state, (draft) => {
        draft.id = null;
        draft.status = Status.LEFT;
        draft.text = "";
        draft.typingUserConnectionId = null;
      });
    default:
      return state;
  }
}
