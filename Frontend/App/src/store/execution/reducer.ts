import { ExecutionActions, ExecutionState } from "./types";
import { getLanguageByKey, Language, Languages } from "../../api/types";
import produce from "immer";

const initialState: ExecutionState = {
  results: [],
  executing: false,
  language: Languages[0],
};

export function executionReducer(
  state = initialState,
  action: ExecutionActions
): ExecutionState {
  switch (action.type) {
    case "JOINED_ROOM":
      return produce(state, (draft) => {
        draft.language = getLanguageByKey(action.room.language) as Language;
      });
    case "LANGUAGE_CHANGED":
      return produce(state, (draft) => {
        draft.language = action.language;
      });
    case "CLEAR_OUTPUT":
      return produce(state, (draft) => {
        draft.results = [];
      });
    case "CODE_EXECUTION_STARTED":
      return produce(state, (draft) => {
        draft.executing = true;

        if (!action.message) return;

        draft.results.push(action.message);
      });
    case "CODE_EXECUTION_COMPLETED":
      return produce(state, (draft) => {
        draft.executing = false;
        draft.results.push(action.result);
      });
    case "LEFT_ROOM":
      return initialState;
    default:
      return state;
  }
}
