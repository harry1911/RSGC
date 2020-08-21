const path = require("path");
const webpack = require("webpack");

const root = path.resolve(__dirname);
const dist = path.join(root, "wwwroot");
const environment = (process.env.NODE_ENV || "development").trim();
const plugins = environment == 'development'
    ? []
    : [
        new webpack.optimize.UglifyJsPlugin({
            beautify: false,
            mangle: {
                screw_ie8: true,
                keep_fnames: true
            },
            compress: {
                warnings: false,
            },
            output: {
                comments: false
            },
            sourceMap: false,
            parallel: true
        }),
    ];

module.exports = {
    entry: {
        app: path.join(root, "src", "app.ts"),
    },
    devtool: 'source-map',
    resolve: {
        extensions: ['.ts', '.js'],
        "modules": [
            "./node_modules"
        ],
        alias: {
            "src": path.resolve('./src')
        }
    },
    module: {
        rules: [
            {
                enforce: 'pre',
                test: /\.tsx?$/,
                loader: 'tslint-loader',
            },
            {
                test: /\.tsx?$/,
                loader: 'ts-loader',
                options: {
                    compilerOptions: {
                        removeComments: environment != 'development',
                        sourceMap: environment == 'development',
                    }
                }
            },
        ],
    },
    output: {
        path: dist,
        filename: "[name].js",
    },
    plugins: plugins,
};
