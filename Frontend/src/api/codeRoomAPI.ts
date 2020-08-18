import axios from "axios";
import { RoomVm } from "./types";

export const BASE_URL = "https://api.codetwice.net";

const instance = axios.create({
  baseURL: BASE_URL + "/api",
});

export function createRoom() {
  return instance.post<RoomVm>("/room");
}
