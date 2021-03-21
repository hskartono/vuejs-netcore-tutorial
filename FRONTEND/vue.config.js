// vue.config.js
module.exports = {
    lintOnSave: false,
    devServer : {
        headers: { "Access-Control-Allow-Origin": "*" },
        /*proxy: {
            '^/api': {
                //target: "https://192.168.1.6/",
                target: "http://localhost:4500/",
                ws: true,
                changeOrigin: true
              },
            
        }*/
        /*open: process.platform === 'darwin',
        host: '0.0.0.0',
        port: 8080, // CHANGE YOUR PORT HERE!
        https: true,
        hotOnly: false,*/
    },
    publicPath: process.env.NODE_ENV === 'production'
    ? '/'
    : '/',
    configureWebpack: {
        output: {
            globalObject: 'this'
        },
        stats: {
            warningsFilter: w => w !== 'CriticalDependenciesWarning',
        },
    },
}
