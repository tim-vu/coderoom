import { User } from "../../api/types";
import { JoinedRoom, LeftRoom } from "../common/types";

export const SET_NICKNAME = "SET_NICKNAME";

export const JOIN_GROUP_CALL = "JOIN_GROUP_CALL";
export const JOINED_GROUP_CALL = "JOINED_GROUP_CALL";

export const SEND_OFFER_DATA = "SEND_OFFER_DATA";
export const SEND_ANSWER_DATA = "SEND_ANSWER_DATA";
export const STREAM_CONNECTED = "STREAM_CONNECTED";
export const RECEIVED_OFFER_DATA = "RECEIVED_OFFER_DATA";
export const RECEIVED_ANSWER_DATA = "RECEIVED_ANSWER_DATA";

export const USER_JOINED_GROUP_CALL = "USER_JOINED_GROUP_CALL";
export const USER_LEFT_GROUP_CALL = "USER_LEFT_GROUP_CALL";

export const USER_JOINED = "USER_JOINED";
export const USER_LEFT = "USER_LEFT";

export interface UserState {
  users: User[];
  me: User | null;
  nickname: string | null;
}

export interface JoinGroupCall {
  type: typeof JOIN_GROUP_CALL;
  stream: MediaStream;
}

export interface JoinedGroupCall {
  type: typeof JOINED_GROUP_CALL;
  stream: MediaStream;
}

export interface SendOfferData {
  type: typeof SEND_OFFER_DATA;
  connectionId: string;
  data: string;
}

export interface SendAnswerData {
  type: typeof SEND_ANSWER_DATA;
  connectionId: string;
  data: string;
}

export interface StreamConnected {
  type: typeof STREAM_CONNECTED;
  connectionId: string;
  stream: MediaStream;
}

export interface ReceivedOfferData {
  type: typeof RECEIVED_OFFER_DATA;
  connectionId: string;
  data: string;
}

export interface ReceivedAnswerData {
  type: typeof RECEIVED_ANSWER_DATA;
  connectionId: string;
  data: string;
}

export interface UserJoinedGroupCall {
  type: typeof USER_JOINED_GROUP_CALL;
  connectionId: string;
}

export interface UserLeftGroupCall {
  type: typeof USER_LEFT_GROUP_CALL;
  connectionId: string;
}

export interface UserJoined {
  type: typeof USER_JOINED;
  user: User;
}

export interface UserLeft {
  type: typeof USER_LEFT;
  connectionId: string;
}

export type UserActions =
  | UserJoined
  | JoinGroupCall
  | JoinedGroupCall
  | ReceivedOfferData
  | ReceivedAnswerData
  | SendOfferData
  | SendAnswerData
  | UserLeftGroupCall
  | UserJoinedGroupCall
  | StreamConnected
  | UserLeft
  | JoinedRoom
  | LeftRoom;
