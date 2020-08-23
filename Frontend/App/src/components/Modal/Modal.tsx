import React from "react";

interface ModalProps {
  show: boolean;
  message: string;
  onYesClicked: () => void;
  onNoClicked: () => void;
  onCancelClicked: () => void;
}

const Modal: React.FC<ModalProps> = ({
  show,
  message,
  onYesClicked,
  onNoClicked,
  onCancelClicked,
}) => {
  if (!show) return null;

  return (
    <div className="fixed inset-0 z-10 bg-background bg-opacity-50">
      <div className="relative z-50 bg-background bg-opacity-100 w-128 mx-auto top-1/2 transform -translate-y-1/2">
        <div className="w-full h-full level-16dp p-5 rounded-md">
          <p className="text-white text-xl opacity-medium mb-4">{message}</p>
          <div className="flex justify-between items-center">
            <button
              className="level-24dp w-24 text-white px-2 py-1 opacity-medium rounded-md"
              onClick={onCancelClicked}
            >
              Cancel
            </button>
            <div className="flex">
              <button
                className="level-24dp w-24 text-white px-2 py-1 opacity-medium rounded-md mr-2"
                onClick={onNoClicked}
              >
                No
              </button>
              <button
                className="level-24dp w-24 text-white px-2 py-1 opacity-medium rounded-md"
                onClick={onYesClicked}
              >
                Yes
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Modal;
