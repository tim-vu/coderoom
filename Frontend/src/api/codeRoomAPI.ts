import axios from "axios";
import { Room } from "./types";

const instance = axios.create({
  baseURL: "http://localhost:5000/api/",
});

export function createRoom() {
  return instance.post<Room>("/room");
}
