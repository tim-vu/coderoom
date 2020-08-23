import {
  CLEAR_OUTPUT,
  ClearOutput,
  CODE_EXECUTION_COMPLETED,
  CODE_EXECUTION_STARTED,
  CodeExecutionCompleted,
  CodeExecutionStarted,
  START_CODE_EXECUTION,
  StartCodeExecution,
} from "./types";
import {Language} from "../../api/types";
import {CHANGE_LANGUAGE} from "../room/types";

export function clearOutput(): ClearOutput {
  return {
    type: CLEAR_OUTPUT,
  };
}

export function startCodeExecution(): StartCodeExecution {
  return {
    type: START_CODE_EXECUTION,
  };
}

export function codeExecutionStarted(
  message: string | undefined
): CodeExecutionStarted {
  return {
    type: CODE_EXECUTION_STARTED,
    message,
  };
}

export function codeExecutionCompleted(result: string): CodeExecutionCompleted {
  return {
    type: CODE_EXECUTION_COMPLETED,
    result,
  };
}

export function changeLanguage(language: Language, reset: boolean) {
  return {
    type: CHANGE_LANGUAGE,
    language,
    reset
  };
}
