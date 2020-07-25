import { RoomActions, RoomState } from "./types";
import produce from "immer";

const initialState: RoomState = {
  id: null,
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
        draft.id = action.room.id;
        draft.text = action.room.text;
        draft.typingUserConnectionId = action.room.typingUserConnectionId;
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
        draft.text = "";
        draft.typingUserConnectionId = null;
      });
    default:
      return state;
  }
}
