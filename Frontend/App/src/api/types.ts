import { Instance } from "simple-peer";

export interface UserVm {
  connectionId: string;
  nickName: string;
  inGroupCall: boolean;
}

export interface User {
  connectionId: string;
  nickName: string;
  inGroupCall: boolean;
  stream?: MediaStream;
  peer: Instance;
}

export interface Language {
  key: number;
  name: string;
  aceMode: string;
}

export interface RoomVm {
  id: string;
  users: UserVm[];
  text: string;
  typingUserConnectionId: string | null;
  language: number;
}

export const Languages: Language[] = [
  { key: 0, name: "Java", aceMode: "java" },
  { key: 1, name: "C#", aceMode: "csharp" },
  { key: 2, name: "Python3", aceMode: "python" },
  { key: 3, name: "JavaScript", aceMode: "javascript" },
];

export function getLanguageByKey(languageKey: number): Language | undefined {
  return Languages.find((l) => l.key === languageKey);
}
