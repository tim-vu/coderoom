import React from "react";
import {
  useLocation,
  withRouter,
  RouteComponentProps,
  Link,
} from "react-router-dom";
import { createRoom } from "../../api/codeRoomAPI";

const Header: React.FC<RouteComponentProps> = ({ history }) => {
  const { pathname } = useLocation();

  const showButton = !pathname.includes("/room");

  const handleCreateRoomClicked = async () => {
    const response = await createRoom();
    history.push(`/${response.data.id}`);
  };

  return (
    <div className="absolute inset-x-0 h-16 top-0 border-b flex justify-between content-center">
      <div className="flex flex-col justify-center ml-6">
        <Link to="/">
          <h1 className="text-3xl text-gray-800 align-middle tracking-tighter">
            CodeRoom
          </h1>
        </Link>
      </div>
      <div
        className={
          "flex flex-col justify-center mr-6 " + (showButton ? "" : "hidden")
        }
      >
        <button
          className="p-5 border rounded h-6 flex items-center bg-teal-500 text-white"
          onClick={handleCreateRoomClicked}
        >
          Create Room
        </button>
      </div>
    </div>
  );
};

export default withRouter(Header);
