import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faRedo } from "@fortawesome/free-solid-svg-icons";
import { clearOutput } from "../../store/execution/actions";
import { connect } from "react-redux";

interface OutputControls {
  clearOutput: typeof clearOutput;
}

const OutputControls: React.FC<OutputControls> = ({ clearOutput }) => {
  return (
    <div className="level-04dp w-full h-12 flex justify-between items-center pl-3 pr-3">
      <div className="h-full flex">
        <div className="h-full border-b border-gray-600 flex justify-center items-center">
          <button className="h-8">
            <p className="text-white opacity-high">Run output</p>
          </button>
        </div>
      </div>
      <div>
        <button
          className="level-06dp w-24 px-2 py-1 rounded-md flex justify-around items-center"
          onClick={clearOutput}
        >
          <p className="text-white opacity-high">Reset</p>
          <FontAwesomeIcon icon={faRedo} className="text-white opacity-high" />
        </button>
      </div>
    </div>
  );
};

export default connect((state) => ({}), { clearOutput })(OutputControls);
