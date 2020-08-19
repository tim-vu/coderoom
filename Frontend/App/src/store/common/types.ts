import { RoomVm } from "../../api/types";

export const JOINED_ROOM = "JOINED_ROOM";
export const LEFT_ROOM = "LEFT_ROOM";

export interface JoinedRoom {
  type: typeof JOINED_ROOM;
  room: RoomVm;
  connectionId: string;
}

export interface LeftRoom {
  type: typeof LEFT_ROOM;
}

export type CommonActions = JoinedRoom | LeftRoom;
