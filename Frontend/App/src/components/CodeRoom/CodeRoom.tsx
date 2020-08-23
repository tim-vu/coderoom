import React, { useLayoutEffect } from "react";
import Editor from "../Editor/Editor";
import SplitPane from "react-split-pane";
import { leaveRoom, updateText } from "../../store/room/actions";
import { AppState } from "../../store";
import { connect } from "react-redux";
import Footer from "../Footer/Footer";
import EditorControls from "../EditorControls/EditorControls";
import OutputControls from "../OutputControls/OutputControls";
import Output from "../Output/Output";
import GroupCall from "../GroupCall/GroupCall";
const MIN_SPLIT_SIZE = 400;
const MIN_CODEROOM_WIDTH = 800;

const CodeRoom: React.FC = () => {
  const [split, setSplit] = React.useState(
    Math.max(document.body.clientWidth / 2, MIN_SPLIT_SIZE)
  );

  const handleWindowResize = () => {
    setSplit(Math.max(document.body.clientWidth / 2, MIN_SPLIT_SIZE));
  };

  useLayoutEffect(() => {
    window.addEventListener("resize", handleWindowResize);

    return () => window.removeEventListener("resize", handleWindowResize);
  }, []);

  return (
    <div
      className="bg-background absolute inset-0 w-full coderoom"
      style={{ minWidth: MIN_CODEROOM_WIDTH }}
    >
      <div className="absolute inset-x-0 top-0 bottom-16">
        <SplitPane
          split="vertical"
          primary="first"
          size={split}
          minSize={MIN_SPLIT_SIZE}
          maxSize={-MIN_SPLIT_SIZE}
        >
          <div className="h-full">
            <EditorControls />
            <Editor />
          </div>
          <div className="h-full flex flex-col">
            <OutputControls />
            <div className="flex-grow">
              <Output />
            </div>
          </div>
        </SplitPane>
        <GroupCall />
      </div>
      <div className="absolute bottom-0 w-full">
        <Footer />
      </div>
    </div>
  );
};

const mapStateToProps = (state: AppState) => ({
  nickname: state.user.nickname,
});

export default connect(mapStateToProps, {
  updateText,
  leaveRoom,
})(CodeRoom);
