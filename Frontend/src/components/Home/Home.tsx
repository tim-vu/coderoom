import React, { Fragment } from "react";

import Header from "../Header/Header";

const Home: React.FC = () => {
  return (
    <Fragment>
      <Header />
      <div className="mt-16 pt-6 w-full h-full">
        <p>Home page</p>
      </div>
    </Fragment>
  );
};

export default Home;
