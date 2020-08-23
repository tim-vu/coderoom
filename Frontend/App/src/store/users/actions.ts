import { User } from "../../api/types";
import {
  JOIN_GROUP_CALL,
  JOINED_GROUP_CALL,
  JoinedGroupCall,
  JoinGroupCall,
  RECEIVED_ANSWER_DATA,
  RECEIVED_OFFER_DATA,
  ReceivedAnswerData,
  ReceivedOfferData,
  SEND_ANSWER_DATA,
  SEND_OFFER_DATA,
  SendAnswerData,
  SendOfferData,
  STREAM_CONNECTED,
  StreamConnected,
  USER_JOINED,
  USER_JOINED_GROUP_CALL,
  USER_LEFT,
  USER_LEFT_GROUP_CALL,
  UserJoined,
  UserJoinedGroupCall,
  UserLeft,
  UserLeftGroupCall,
} from "./types";

export function joinGroupCall(stream: MediaStream): JoinGroupCall {
  return {
    type: JOIN_GROUP_CALL,
    stream,
  };
}

export function joinedGroupCall(stream: MediaStream): JoinedGroupCall {
  return {
    type: JOINED_GROUP_CALL,
    stream,
  };
}

export function streamConnected(
  connectionId: string,
  stream: MediaStream
): StreamConnected {
  return {
    type: STREAM_CONNECTED,
    connectionId,
    stream,
  };
}

export function sendOfferData(
  connectionId: string,
  data: string
): SendOfferData {
  return {
    type: SEND_OFFER_DATA,
    connectionId,
    data,
  };
}

export function sendAnswerData(
  connectionId: string,
  data: string
): SendAnswerData {
  return {
    type: SEND_ANSWER_DATA,
    connectionId,
    data,
  };
}

export function receivedOfferData(
  connectionId: string,
  data: string
): ReceivedOfferData {
  return {
    type: RECEIVED_OFFER_DATA,
    connectionId,
    data,
  };
}

export function receivedAnswerData(
  connectionId: string,
  data: string
): ReceivedAnswerData {
  return {
    type: RECEIVED_ANSWER_DATA,
    connectionId,
    data,
  };
}

export function userJoinedGroupCall(connectionId: string): UserJoinedGroupCall {
  return {
    type: USER_JOINED_GROUP_CALL,
    connectionId,
  };
}

export function userLeftGroupCall(connectionId: string): UserLeftGroupCall {
  return {
    type: USER_LEFT_GROUP_CALL,
    connectionId,
  };
}

export function userJoined(user: User): UserJoined {
  return {
    type: USER_JOINED,
    user,
  };
}

export function userLeft(connectionId: string): UserLeft {
  return {
    type: USER_LEFT,
    connectionId,
  };
}
