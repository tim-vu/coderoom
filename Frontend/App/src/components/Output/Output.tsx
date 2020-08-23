import React from "react";
import { AppState } from "../../store";
import { connect } from "react-redux";

interface OutputProps {
  results: string[];
}

const formatOutput = (outputs: string[]) => {
  return outputs
    .map((o) => {
      return o.endsWith("\n") ? o.substring(0, o.length - 1) : o;
    })
    .join("\n");
};

const Output: React.FC<OutputProps> = ({ results }) => {
  return (
    <div className="w-full h-full m-3">
      <textarea
        spellCheck={false}
        autoCapitalize="off"
        autoComplete="off"
        className="w-full h-full resize-none bg-transparent focus:outline-none text-white opacity-75"
        defaultValue={formatOutput(results)}
        readOnly={true}
      />
    </div>
  );
};

const mapStateToProps = (state: AppState) => ({
  results: state.execution.results,
});

export default connect(mapStateToProps, {})(Output);
