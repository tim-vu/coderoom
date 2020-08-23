import { JoinedRoom, LeftRoom } from "../common/types";
import { Language } from "../../api/types";
import { CHANGE_LANGUAGE, LANGUAGE_CHANGED } from "../room/types";

export const CLEAR_OUTPUT = "CLEAR_OUTPUT";
export const START_CODE_EXECUTION = "START_CODE_EXECUTION";
export const CODE_EXECUTION_STARTED = "CODE_EXECUTION_STARTED";
export const CODE_EXECUTION_COMPLETED = "CODE_EXECUTION_COMPLETED";

export interface ExecutionState {
  results: string[];
  executing: boolean;
  language: Language;
}

export interface ClearOutput {
  type: typeof CLEAR_OUTPUT;
}

export interface StartCodeExecution {
  type: typeof START_CODE_EXECUTION;
}

export interface CodeExecutionStarted {
  type: typeof CODE_EXECUTION_STARTED;
  message: string | undefined;
}

export interface CodeExecutionCompleted {
  type: typeof CODE_EXECUTION_COMPLETED;
  result: string;
}

export interface ClearOutput {
  type: typeof CLEAR_OUTPUT;
}

export interface ChangeLanguage {
  type: typeof CHANGE_LANGUAGE;
  language: Language;
  reset: boolean;
}

export interface LanguageChanged {
  type: typeof LANGUAGE_CHANGED;
  language: Language;
}

export type ExecutionActions =
  | ClearOutput
  | StartCodeExecution
  | CodeExecutionStarted
  | CodeExecutionCompleted
  | ChangeLanguage
  | LanguageChanged
  | LeftRoom
  | JoinedRoom;
