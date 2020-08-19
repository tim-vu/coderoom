import {
  applyMiddleware,
  combineReducers,
  createStore,
  Middleware,
} from "redux";
import { roomReducer } from "./room/reducer";
import { composeWithDevTools } from "redux-devtools-extension";
import { RoomActions } from "./room/types";
import { signalRMiddleware } from "../api/signalRMiddleware";
import { userReducer } from "./users/reducer";
import { ExecutionActions } from "./execution/types";
import { executionReducer } from "./execution/reducer";
import { UserActions } from "./users/types";
import { CommonActions } from "./common/types";
import { webRTCMiddleware } from "../api/webRTCMiddleware";

const rootReducer = combineReducers({
  room: roomReducer,
  user: userReducer,
  execution: executionReducer,
});

function configureStore() {
  const middlewares: Middleware[] = [signalRMiddleware, webRTCMiddleware];
  const middlewareEnhancer = applyMiddleware(...middlewares);

  return createStore(rootReducer, composeWithDevTools(middlewareEnhancer));
}
const Store = configureStore();

export type Action =
  | CommonActions
  | RoomActions
  | ExecutionActions
  | UserActions;

export type AppState = ReturnType<typeof rootReducer>;
export default Store;
