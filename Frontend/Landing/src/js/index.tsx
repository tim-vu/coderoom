import { createRoom } from "./api";

const menuButton = document.getElementById("menu-button");
const tryButton = document.getElementById("try-button");
const menu = document.getElementById("menu");

menuButton.onclick = () => {
  const classList = menu.classList;

  if (classList.contains("menu-visible")) classList.remove("menu-visible");
  else classList.add("menu-visible");
};

tryButton.onclick = async () => {
  const room = await createRoom();

  window.location.href = "https://codetwice.net/app/" + room.id;
};
