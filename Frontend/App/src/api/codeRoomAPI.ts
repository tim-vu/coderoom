import axios from "axios";
import { RoomVm } from "./types";

export const BASE_URL = process.env.NODE_ENV === "production" ? "https://api.codetwice.net" : "http://localhost:5000"

const instance = axios.create({
  baseURL: BASE_URL + "/api",
});

export function createRoom() {
  return instance.post<RoomVm>("/room");
}
