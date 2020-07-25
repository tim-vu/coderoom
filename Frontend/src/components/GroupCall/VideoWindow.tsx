import React, { CSSProperties, useEffect } from "react";

interface VideoWindowProps {
  stream: MediaStream;
  title: string;
  handle: string;
  style?: CSSProperties;
  className?: string;
  onMouseDown?: (event: React.MouseEvent) => void;
  onMouseUp?: (event: React.MouseEvent) => void;
  onTouchStart?: (event: React.TouchEvent) => void;
  onTouchEnd?: (event: React.TouchEvent) => void;
}

const VideoStyle: CSSProperties = {
  width: "320px",
  height: "180px",
};

const VideoWindow: React.FC<VideoWindowProps> = (props) => {
  const { stream, title, handle, style, ...remainingProps } = props;

  const videoRef = React.useRef<HTMLVideoElement>(null);

  useEffect(() => {
    if (!videoRef.current) return;
    videoRef.current.srcObject = stream;
  }, [stream]);

  return (
    <div
      {...remainingProps}
      className={
        "overflow-hidden rounded bg-background pointer-events-auto z-50"
      }
      style={{ ...style }}
    >
      <div
        className={
          handle +
          " level-06dp flex justify-center items-center p-1 cursor-move"
        }
      >
        <p className="text-white opacity-medium">{title}</p>
      </div>
      <div className="level-06dp p-1 pt-0">
        <video
          className="object-fill"
          ref={videoRef}
          autoPlay={true}
          style={VideoStyle}
        />
      </div>
    </div>
  );
};

export default VideoWindow;
