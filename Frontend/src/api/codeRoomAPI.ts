import axios from "axios";
import { Room } from "./types";

const instance = axios.create({
  baseURL: "https://api.codetwice.net/api/",
});

export function createRoom() {
  return instance.post<Room>("/room");
}
