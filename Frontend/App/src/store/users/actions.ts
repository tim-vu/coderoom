import {User} from "../../api/types";
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
  USER_JOINED,
  USER_JOINED_GROUP_CALL,
  USER_LEFT,
  USER_LEFT_GROUP_CALL,
  UserJoinedGroupCall,
} from "./types";
import {AppThunk} from "../index";
import {toast} from "react-toastify";

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
): AppThunk {

  return function(dispatch, getState){

    const nickname = getState().user.users.filter(u => u.connectionId === connectionId).map(u => u.nickName)[0]

    toast.info(`${nickname} has joined the call`);

    dispatch({
      type: STREAM_CONNECTED,
      connectionId,
      stream,
    });
  }
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

export function userLeftGroupCall(connectionId: string): AppThunk {
  return function(dispatch, getState){
    const nickname = getState().user.users.filter(u => u.connectionId === connectionId).map(u => u.nickName)[0]

    toast.info(`${nickname} has left the call`);

    dispatch({
      type: USER_LEFT_GROUP_CALL,
      connectionId,
    });
  }
}


export function userJoined(user: User): AppThunk {
  return function(dispatch)  {
    dispatch({
      type: USER_JOINED,
      user,
    });

    toast.info(`${user.nickName} has joined the room`);
  }
}


export function userLeft(connectionId: string): AppThunk<void> {

  return function(dispatch, getState){

    const nickname = getState().user.users.filter(u => u.connectionId === connectionId).map(u => u.nickName)[0];

    toast.info(`${nickname} has left the room`);

    dispatch({
      type: USER_LEFT,
      connectionId,
    });

  }
}
