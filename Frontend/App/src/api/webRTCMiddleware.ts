import { Dispatch, Middleware, Store } from "redux";
import { Action, AppState } from "../store";
import Peer, { Instance } from "simple-peer";
import { User } from "./types";
import { ById } from "../store/byid";
import {
  sendAnswerData,
  sendOfferData,
  streamConnected,
  userLeftGroupCall,
} from "../store/users/actions";

let PeerConnections: ById<Instance> = {};

export const webRTCMiddleware: Middleware<Store<AppState, Action>> = (
  store
) => (next) => async (action: Action) => {
  const { dispatch, getState } = store;

  switch (action.type) {
    case "JOINED_GROUP_CALL":
      PeerConnections = createPeerConnections(
        Object.values(getState().user.users),
        action.stream,
        dispatch
      );

      return next(action);
    case "USER_LEFT":
      const peer: Instance | undefined = PeerConnections[action.connectionId];

      if (!peer) return next(action);

      peer.destroy();
      delete PeerConnections[action.connectionId];
      break;
    case "RECEIVED_OFFER_DATA":
      const stream = getState().user.me.stream as MediaStream;

      const signalPeer: Instance | undefined =
        PeerConnections[action.connectionId];

      if (signalPeer) {
        signalPeer.signal(JSON.parse(action.data));
        return;
      }

      PeerConnections[action.connectionId] = createPeerConnection(
        action.connectionId,
        stream,
        action.data,
        dispatch
      );

      return next(action);
    case "RECEIVED_ANSWER_DATA":
      const answerPeer: Instance | undefined =
        PeerConnections[action.connectionId];

      if (!answerPeer) {
        console.log("Answer data received from unknown peer");
        return;
      }

      console.log("Received answer data from: " + action.connectionId);
      answerPeer.signal(JSON.parse(action.data));
      break;
    default:
      return next(action);
  }
};

const createPeerConnections: (
  user: User[],
  stream: MediaStream,
  dispatch: Dispatch
) => ById<Instance> = (users, stream, dispatch) => {
  return users
    .filter((u) => u.inGroupCall)
    .reduce((acc: ById<Instance>, user: User) => {
      const peer = new Peer({
        initiator: true,
        stream: stream,
        config: WEBRTC_CONFIG,
      });

      peer.on("signal", async (data) => {
        console.log("Sending offer data to: " + user.connectionId);
        dispatch(sendOfferData(user.connectionId, JSON.stringify(data)));
      });

      peer.on("stream", (stream) => {
        dispatch(streamConnected(user.connectionId, stream));
      });

      peer.on("close", () => {
        delete PeerConnections[user.connectionId];
        dispatch(userLeftGroupCall(user.connectionId));
      });

      acc[user.connectionId] = peer;

      return acc;
    }, {});
};

const createPeerConnection = (
  connectionId: string,
  stream: MediaStream,
  data: string,
  dispatch: Dispatch
) => {
  const peer = new Peer({
    initiator: false,
    stream,
    config: WEBRTC_CONFIG,
  });

  peer.on("signal", async (data) => {
    dispatch(sendAnswerData(connectionId, JSON.stringify(data)));
  });

  peer.on("stream", (stream) => {
    dispatch(streamConnected(connectionId, stream));
  });

  peer.on("close", () => {
    delete PeerConnections[connectionId];
    dispatch(userLeftGroupCall(connectionId));
  });

  console.log("Received offer data from: " + connectionId);
  peer.signal(JSON.parse(data));

  return peer;
};

const WEBRTC_CONFIG = {
  iceServers: [
    {
      urls: ["stun:eu-turn7.xirsys.com"],
    },
    /*
    {
      username:
        "p4TvAvkV6Oq5r4X2msIJr9tkJqeK7s-WoikZwtXNHjZRaLERVDxcM5Mv5x5taQ3jAAAAAF8XE6x0aW12dQ==",
      credential: "d3400b12-cb6c-11ea-bafa-0242ac140004",
      urls: [
        "turn:eu-turn7.xirsys.com:80?transport=udp",
        "turn:eu-turn7.xirsys.com:3478?transport=udp",
        "turn:eu-turn7.xirsys.com:80?transport=tcp",
        "turn:eu-turn7.xirsys.com:3478?transport=tcp",
        "turns:eu-turn7.xirsys.com:443?transport=tcp",
        "turns:eu-turn7.xirsys.com:5349?transport=tcp",
      ],
    },
    */
  ],
};
