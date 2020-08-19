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
