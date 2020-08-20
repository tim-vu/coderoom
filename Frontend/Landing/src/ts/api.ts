export interface UserVm {
  connectionId: string;
  nickName: string;
  inGroupCall: boolean;
}

export interface RoomVm {
  id: string;
  users: UserVm[];
  text: string;
  typingUserConnectionId: string | null;
  language: number;
}

const BASE_URL = "https://api.codetwice.net/api";

export const createRoom: () => Promise<RoomVm> = () => {
  return fetch(BASE_URL + "/room", {
    method: "POST",
  }).then((r) => r.json());
};
