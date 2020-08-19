import React from "react";
import Draggable from "react-draggable";
import VideoWindow from "./VideoWindow";
import { User } from "../../api/types";
import { AppState } from "../../store";
import { connect } from "react-redux";

interface GroupCallProps {
  users: User[];
  me: User | null;
}

const renderUserVideo = (user: User) => {
  const handle = "_" + user.connectionId;

  return (
    <Draggable key={user.connectionId} handle={"." + handle} bounds="parent">
      <VideoWindow
        handle={handle}
        title={user.nickName}
        stream={user.stream as MediaStream}
      />
    </Draggable>
  );
};

const GroupCall: React.FC<GroupCallProps> = ({ users, me }) => {
  const allUsers = me ? [...users, me] : users;

  return (
    <div className="bg-transparent inset-0 w-full h-full pointer-events-none flex justify-end items-end p-6">
      {allUsers.filter((u) => u.stream !== undefined).map(renderUserVideo)}
    </div>
  );
};

const mapStateToProps = (state: AppState) => ({
  users: state.user.users,
  me: state.user.me,
});

export default connect(mapStateToProps, {})(GroupCall);
