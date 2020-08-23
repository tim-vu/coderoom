import React from "react";
import { connect } from "react-redux";
import { joinRoom } from "../../store/room/actions";

const USERNAME = /\w{3,20}/;

interface State {
  value: string;
  error: string | null;
}

interface GreetingProps {
  roomId: string;
  joinRoom: typeof joinRoom;
}

const Greeting: React.FC<GreetingProps> = ({ roomId, joinRoom }) => {
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

    joinRoom(roomId, state.value);
  };

  return (
    <div className="h-full flex flex-col justify-center items-center opacity-high">
      <h1 className="text-white text-5xl font-semibold text-center mb-5">
        Welcome to CodeTwice
      </h1>
      <p className="text-white text-xl font-medium opacity-medium mb-3">
        Choose a nickname to get started
      </p>
      <div className="flex flex-col items-center top-1/2 w-64 level-06dp p-4 rounded-md">
        {state.error ? (
          <p className="text-red-500 mb-1">{state.error}</p>
        ) : null}
        <input
          value={state.value}
          onChange={handleOnChange}
          onBlur={handleOnBlur}
          type="text"
          className="focus:outline-none border-none rounded-md h-10 w-full mb-3 p-2 level-08dp text-white opacity-high"
        />
        <button
          className="level-08dp w-full px-2 py-1 rounded-md"
          onClick={handleOKClicked}
        >
          <p className="text-white opacity-high">OK</p>
        </button>
      </div>
    </div>
  );
};

export default connect(null, { joinRoom })(Greeting);
