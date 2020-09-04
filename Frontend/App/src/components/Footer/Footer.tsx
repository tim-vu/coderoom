import React from "react";

import { faUserPlus } from "@fortawesome/free-solid-svg-icons/";
import { faVideo } from "@fortawesome/free-solid-svg-icons/";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { User } from "../../api/types";
import { AppState } from "../../store";
import { connect } from "react-redux";
import { UserState } from "../../store/users/types";
import { joinGroupCall } from "../../store/users/actions";
import { toast } from "react-toastify";

interface FooterProps {
  user: UserState;
  joinGroupCall: typeof joinGroupCall;
}

const renderUser = (user: User) => {
  return (
    <div className="flex items-center ml-3" key={user.connectionId}>
      <span className="w-3 h-3 rounded-full bg-green-500" />
      <p className="text-white opacity-medium ml-1">{user.nickName}</p>
    </div>
  );
};

const Footer: React.FC<FooterProps> = ({ user, joinGroupCall }) => {
  const handleJoinCallClicked = () => {
    navigator.mediaDevices
      .getUserMedia({ video: true, audio: true })
      .then(joinGroupCall)
      .catch((err) => {
        console.log(err);
      });
  };

  const handleInviteClicked = () => {
    navigator.clipboard.writeText(window.location.href).then(() => {
      toast.info("The link has been copied to your clipboard");
    });
  };

  const joinCallDisabled = user.me?.inGroupCall;

  return (
    <div className="level-04dp h-16 w-full flex justify-between items-center">
      <div className="ml-8 flex items-center">
        <button
          className="level-06dp w-24 py-1 px-2 rounded-md flex justify-around items-center text-white opacity-high"
          onClick={handleInviteClicked}
        >
          Invite
          <FontAwesomeIcon icon={faUserPlus} className="text-white" />
        </button>
        <button
          className="ml-3 level-06dp w-28 py-1 px-2 rounded-md flex justify-around items-center text-white opacity-high"
          disabled={joinCallDisabled}
          onClick={handleJoinCallClicked}
        >
          Join Call
          <FontAwesomeIcon icon={faVideo} className="text-white" />
        </button>
        <div className="ml-3 flex">
          {Object.values(user.users).map((u) => renderUser(u))}
        </div>
      </div>
      <div className="mr-6">
        <button className="bg-red-600 py-1 px-2 rounded-md text-white opacity-high">
          Close room
        </button>
      </div>
    </div>
  );
};

const mapStateToProps = (state: AppState) => ({
  user: state.user,
});

export default connect(mapStateToProps, { joinGroupCall })(Footer);
