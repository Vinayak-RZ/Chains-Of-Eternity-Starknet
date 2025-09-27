import resolve from "@rollup/plugin-node-resolve"
import commonjs from "@rollup/plugin-commonjs"
import json from "@rollup/plugin-json"
import replace from "@rollup/plugin-replace"
import terser  from "@rollup/plugin-terser"

export default {
  input: "src/flowBridge.js",
  output: {
    file: "dist/flowBridge.bundle.js",
    format: "umd",           // single <script> tag, exposes window.FlowBridge
    name: "FlowBridge",
    sourcemap: true,
    inlineDynamicImports: true,
  },
  plugins: [
    // Some deps expect these env vars at build time
    replace({
      preventAssignment: true,
      "process.env.NODE_ENV": JSON.stringify("production"),
      "process.env.FCL_ENV": JSON.stringify("browser")
    }),
    resolve({ browser: true, preferBuiltins: false }),
    commonjs(),
    json(),
    terser()
  ]
}
