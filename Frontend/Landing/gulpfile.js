const gulp = require("gulp");
const concat = require("gulp-concat");
const connect = require("gulp-connect");
const postcss = require("gulp-postcss");
const cleanCSS = require("gulp-clean-css");
const injectPartials = require("gulp-inject-partials");
const browserify = require("browserify");
const source = require("vinyl-source-stream");
const tsify = require("tsify");
const fancy_log = require("fancy-log");
const uglify = require("gulp-uglify");
const sourcemaps = require("gulp-sourcemaps");
const buffer = require("vinyl-buffer");
const gulpif = require("gulp-if");

const configuration = {
  sourceMaps: process.env.NODE_ENV !== "production",

  paths: {
    src: {
      html: "./src/*.html",
      partials: "./src/partials/*.html",
      css: "./src/styles/tailwind.css",
      ts: "./src/styles/*.ts",
    },

    output: "./build",
  },

  localServer: {
    port: 5000,
    url: "http://localhost:5000",
  },
};

const html = () => {
  return gulp
    .src(configuration.paths.src.html)
    .pipe(
      injectPartials({
        start: '@partial("{{path}}',
        end: '")',
        removeTags: true,
      })
    )
    .pipe(gulp.dest(configuration.paths.output))
    .pipe(connect.reload());
};

const css = () => {
  return gulp
    .src(configuration.paths.src.css)
    .pipe(postcss([require("tailwindcss"), require("autoprefixer")]))
    .pipe(concat("app.css"))
    .pipe(cleanCSS())
    .pipe(gulp.dest(configuration.paths.output + "/assets"))
    .pipe(connect.reload());
};

const ts = () => {
  return browserify({
    basedir: ".",
    debug: true,
    entries: ["src/ts/index.ts"],
    cache: {},
    packageCache: {},
  })
    .plugin(tsify)
    .bundle()
    .on("error", fancy_log)
    .pipe(source("bundle.js"))
    .pipe(buffer())
    .pipe(gulpif(configuration.sourceMaps, sourcemaps.init({ loadMaps: true })))
    .pipe(uglify())
    .pipe(gulpif(configuration.sourceMaps, sourcemaps.write("./")))
    .pipe(gulp.dest(configuration.paths.output + "/assets"));
};

const server = () => {
  return connect.server({
    root: "build",
    port: configuration.localServer.port,
    livereload: true,
  });
};

const watch = () => {
  gulp.watch(configuration.paths.src.html, html);
  gulp.watch(configuration.paths.src.partials, html);
  gulp.watch(configuration.paths.src.css, css);
  gulp.watch(configuration.paths.src.ts, ts);
};

exports.css = css;
exports.ts = ts;
exports.dev = gulp.parallel(html, css, ts, server, watch);
exports.build = gulp.parallel(html, css, ts);
