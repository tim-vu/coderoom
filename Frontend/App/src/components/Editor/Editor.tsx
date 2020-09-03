import React from "react";
import AceEditor from "react-ace";

import { config } from "ace-builds";

import "ace-builds/src-noconflict/mode-java";
import "ace-builds/src-noconflict/mode-csharp";
import "ace-builds/src-noconflict/mode-python";
import "ace-builds/src-noconflict/mode-javascript";
import "ace-builds/src-noconflict/mode-typescript";
import "ace-builds/src-noconflict/theme-dracula";
import { Language } from "../../api/types";
import { updateText } from "../../store/room/actions";
import { AppState } from "../../store";
import { connect } from "react-redux";

config.set(
  "basePath",
  "https://cdn.jsdelivr.net/npm/ace-builds@1.4.8/src-noconflict/"
);
config.setModuleUrl(
  "ace/mode/javascript_worker",
  "https://cdn.jsdelivr.net/npm/ace-builds@1.4.8/src-noconflict/worker-javascript.js"
);

interface EditorProps {
  text: string;
  language: Language;
  locked: boolean;
  updateText: typeof updateText;
}

const Editor: React.FC<EditorProps> = ({
  text,
  locked,
  language,
  updateText,
}) => {
  return (
    <div className="w-full h-full">
      <AceEditor
        mode={language.aceMode}
        theme="dracula"
        fontSize={16}
        showPrintMargin={false}
        onChange={updateText}
        value={text}
        width="100%"
        height="100%"
        readOnly={locked}
        setOptions={{
          tabSize: 4,
        }}
      />
    </div>
  );
};

const mapStateToProps = (state: AppState) => ({
  text: state.room.text,
  language: state.execution.language,
  locked:
    state.room.typingUserConnectionId !== null &&
    state.room.typingUserConnectionId !== state.user.me?.connectionId,
});

export default connect(mapStateToProps, { updateText })(Editor);
