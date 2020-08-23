import React from "react";

interface ErrorProps {
  errorMessage: string;
}

const Error: React.FC<ErrorProps> = ({ errorMessage }) => {
  return (
    <div className="h-full flex flex-col justify-center items-center">
      <h2 className="text-white text-5xl font-semibold opacity-high">
        Oops, something went wrong
      </h2>
      <p className="text-white text-xl mb-6">{errorMessage}</p>
      <div className="flex flex-row justify-around items-center">
        <button
          className="level-08dp w-32 px-2 py-1 rounded-md m-1"
          onClick={() => window.location.reload()}
        >
          <p className="text-white opacity-high">Refresh</p>
        </button>
        <a href="/">
          <button className="level-08dp w-32 px-2 py-1 rounded-md m-1">
            <p className="text-white opacity-high">Home</p>
          </button>
        </a>
      </div>
    </div>
  );
};

export default Error;
