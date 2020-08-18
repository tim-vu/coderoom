import { JoinedRoom, LeftRoom } from "../common/types";

export const CHECK_ROOM_EXISTS = "CHECK_ROOM_EXISTS";
export const ROOM_EXISTS = "ROOM_EXISTS";

export const ROOM_ERROR = "ROOM_ERROR";
export const JOIN_ROOM = "JOIN_ROOM";
export const LEAVE_ROOM = "LEAVE_ROOM";
export const UPDATE_TEXT = "UPDATE_TEXT";
export const TEXT_UPDATED = "TEXT_UPDATED";
export const CHANGE_LANGUAGE = "CHANGE_LANGUAGE";
export const LANGUAGE_CHANGED = "LANGUAGE_CHANGED";

export const TYPING_USER_CHANGED = "TYPING_USER_CHANGED";

export enum Status {
  CONNECTING,
  ERROR,
  EXISTS,
  JOINED,
  LEFT,
}

export interface RoomState {
  id: string | null;
  status: Status;
  error: string | null;
  text: string;
  typingUserConnectionId: string | null;
}

export interface CheckRoomExists {
  type: typeof CHECK_ROOM_EXISTS;
  roomId: string;
}

export interface RoomExists {
  type: typeof ROOM_EXISTS;
}

export interface RoomError {
  type: typeof ROOM_ERROR;
  error: string;
}

export interface JoinRoom {
  type: typeof JOIN_ROOM;
  roomId: string;
  nickName: string;
}

export interface LeaveRoom {
  type: typeof LEAVE_ROOM;
}

export interface UpdateText {
  type: typeof UPDATE_TEXT;
  text: string;
}

export interface TextUpdated {
  type: typeof TEXT_UPDATED;
  text: string;
}

export interface TypingUserChanged {
  type: typeof TYPING_USER_CHANGED;
  connectionId: string | null;
}

export type RoomActions =
  | CheckRoomExists
  | RoomExists
  | RoomError
  | JoinRoom
  | JoinedRoom
  | LeaveRoom
  | LeftRoom
  | UpdateText
  | TextUpdated
  | TypingUserChanged;
