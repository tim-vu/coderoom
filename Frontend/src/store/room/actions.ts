import {
  CHANGE_LANGUAGE,
  JOIN_ROOM,
  JoinRoom,
  LANGUAGE_CHANGED,
  LEAVE_ROOM,
  LeaveRoom,
  TEXT_UPDATED,
  TextUpdated,
  TYPING_USER_CHANGED,
  TypingUserChanged,
  UPDATE_TEXT,
  UpdateText,
} from "./types";
import { Language, Room } from "../../api/types";
import { JOINED_ROOM, JoinedRoom, LEFT_ROOM, LeftRoom } from "../common/types";

export function joinRoom(roomId: string, nickName: string): JoinRoom {
  return {
    type: JOIN_ROOM,
    roomId,
    nickName,
  };
}

export function leaveRoom(): LeaveRoom {
  return {
    type: LEAVE_ROOM,
  };
}

export function updateText(text: string): UpdateText {
  return {
    type: UPDATE_TEXT,
    text,
  };
}

export function textUpdated(text: string): TextUpdated {
  return {
    type: TEXT_UPDATED,
    text,
  };
}

export function joinedRoom(room: Room, connectionId: string): JoinedRoom {
  return {
    type: JOINED_ROOM,
    room,
    connectionId,
  };
}

export function leftRoom(): LeftRoom {
  return {
    type: LEFT_ROOM,
  };
}

export function changeLanguage(language: Language) {
  return {
    type: CHANGE_LANGUAGE,
    language,
  };
}

export function languageChanged(language: Language) {
  return {
    type: LANGUAGE_CHANGED,
    language,
  };
}

export function typingUserChanged(
  connectionId: string | null
): TypingUserChanged {
  return {
    type: TYPING_USER_CHANGED,
    connectionId: connectionId,
  };
}
