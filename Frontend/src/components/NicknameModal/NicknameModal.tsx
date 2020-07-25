import React from "react";
import { Link } from "react-router-dom";

interface NicknameModalProps {
  visible: boolean;
  onNicknameChosen: (nickname: string) => void;
}

const USERNAME = /\w{3,20}/;

interface State {
  value: string;
  error: string | null;
}

const NicknameModal: React.FC<NicknameModalProps> = ({
  visible,
  onNicknameChosen,
}) => {
  const [state, setState] = React.useState<State>({
    value: "",
    error: null,
  });

  const handleOnChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setState({ value: event.target.value, error: null });
  };

  const handleOnBlur = () => {
    setState({
      ...state,
      error: USERNAME.test(state.value)
        ? null
        : "You must choose a valid nickname",
    });
  };

  const handleOKClicked = () => {
    if (!USERNAME.test(state.value)) return;

    onNicknameChosen(state.value);
  };

  if (!visible) return null;

  return (
    <div className={"fixed inset-0 z-10 bg-background bg-opacity-50"}>
      <div className="relative bg-background bg-opacity-100 w-128 mx-auto top-1/2 transform -translate-y-1/2">
        <div className="w-full h-full level-16dp p-4 rounded-md">
          <h3 className="font-medium text-white text-xl mb-3">
            Choose a nickname:
          </h3>
          <div>
            {state.error ? (
              <p className="text-red-500 mb-1">{state.error}</p>
            ) : null}
            <input
              value={state.value}
              onChange={handleOnChange}
              onBlur={handleOnBlur}
              type="text"
              className="focus:outline-none border-none rounded-md h-10 w-full mb-3 p-2 level-24dp text-white opacity-high"
            />
          </div>
          <div className="flex justify-end">
            <Link to="/">
              <button className="level-24dp w-20 mr-2 px-2 py-1 rounded-md">
                <p className="text-white opacity-high">Cancel</p>
              </button>
            </Link>
            <button
              className="level-24dp w-20 px-2 py-1 rounded-md"
              onClick={handleOKClicked}
            >
              <p className="text-white opacity-high">OK</p>
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default NicknameModal;
