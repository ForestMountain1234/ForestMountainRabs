export interface IElectronAPI {
    //preload.tsで公開したAPI名を記述
}

declare global {
    interface Window {
        API: IElectronAPI
    }
}