import {
  applyMiddleware,
  combineReducers,
  createStore,
  Middleware, Store,
} from "redux";
import { roomReducer } from "./room/reducer";
import { composeWithDevTools } from "redux-devtools-extension";
import { RoomActions, RoomState} from "./room/types";
import { signalRMiddleware } from "../api/signalRMiddleware";
import { userReducer } from "./users/reducer";
import { ExecutionActions, ExecutionState} from "./execution/types";
import { executionReducer } from "./execution/reducer";
import { UserActions, UserState} from "./users/types";
import { CommonActions } from "./common/types";
import { webRTCMiddleware } from "../api/webRTCMiddleware";
import thunk, {ThunkAction, ThunkDispatch} from "redux-thunk";

export interface AppState {
  room: RoomState;
  user: UserState
  execution: ExecutionState
}

export type Action =
  | CommonActions
  | RoomActions
  | ExecutionActions
  | UserActions;

export type AppDispatch = ThunkDispatch<AppState, { }, Action>
export type AppStore = Store<AppState, Action> & { dispatch: AppDispatch }
export type AppThunk<ReturnType = void> = ThunkAction<ReturnType, AppState, unknown, Action>

const rootReducer = combineReducers({
  room: roomReducer,
  user: userReducer,
  execution: executionReducer,
});

function configureStore() {
  const middlewares: Middleware[] = [thunk, signalRMiddleware, webRTCMiddleware];
  const middlewareEnhancer = applyMiddleware(...middlewares);


  const store = createStore<AppState, Action, {}, {}>(rootReducer, composeWithDevTools(middlewareEnhancer));

  if (process.env.NODE_ENV !== 'production') {
    if (module.hot) {
      module.hot.accept('.', () => {
        store.replaceReducer(rootReducer);
      });
    }
  }

  return store;
}

export default configureStore();
