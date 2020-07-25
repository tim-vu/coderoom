import React from "react";
import { getLanguageByKey, Language, Languages } from "../../api/types";
import { faPlay } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { AppState } from "../../store";
import { changeLanguage } from "../../store/room/actions";
import { connect } from "react-redux";
import { startCodeExecution } from "../../store/execution/actions";
import { ExecutionState } from "../../store/execution/types";

const renderLanguageOption = (language: Language) => {
  return (
    <option
      className="border-none bg-background"
      value={language.key}
      key={language.key}
    >
      {language.name}
    </option>
  );
};

interface EditorControlsProps {
  execution: ExecutionState;
  changeLanguage: typeof changeLanguage;
  startCodeExecution: typeof startCodeExecution;
}

const EditorControls: React.FC<EditorControlsProps> = ({
  execution,
  startCodeExecution,
  changeLanguage,
}) => {
  const handleOnLanguageChange = (
    event: React.ChangeEvent<HTMLSelectElement>
  ) => {
    changeLanguage(getLanguageByKey(parseInt(event.target.value)) as Language);
  };

  const handleOnRunClicked = () => {
    startCodeExecution();
  };

  return (
    <div className="level-04dp w-full h-12 flex justify-between items-center pb-1">
      <div className="ml-5 inline-block relative">
        <select
          className="level-06dp appearance-none h-8 py-1 px-2 w-32 text-white  rounded-md"
          onChange={handleOnLanguageChange}
          value={execution.language.key}
        >
          {Languages.map((l) => renderLanguageOption(l))}
        </select>
        <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center px-2 text-gray-700">
          <svg className="text-white h-4 w-4" viewBox="0 0 20 20">
            <path
              className="fill-current opacity-high"
              d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z"
            />
          </svg>
        </div>
      </div>
      <div className="mr-5">
        <button
          className="level-06dp bg-green-500 h-8 w-24 rounded-md flex justify-around items-center"
          disabled={execution.executing}
          onClick={handleOnRunClicked}
        >
          <p className="text-white opacity-high">Run</p>
          <FontAwesomeIcon
            icon={faPlay}
            className="text-sm text-white opacity-high"
          />
        </button>
      </div>
    </div>
  );
};

const mapStateToProps = (state: AppState) => ({
  execution: state.execution,
});

export default connect(mapStateToProps, { changeLanguage, startCodeExecution })(
  EditorControls
);
