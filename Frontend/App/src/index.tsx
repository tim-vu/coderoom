import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import Store from "./store";
import App from "./components/App/App";

import "./styles/app.css";
import "./styles/spinner.css";
import "react-toastify/dist/ReactToastify.css";

const rootElem = document.getElementById("root");
ReactDOM.render(
  <Provider store={Store}>
    <App />
  </Provider>,
  rootElem
);

if (module.hot) {
  module.hot.accept("./components/App/App", () => {
    const NextApp = require("./components/App/App").default;
    ReactDOM.render(<NextApp />, rootElem);
  });
}
