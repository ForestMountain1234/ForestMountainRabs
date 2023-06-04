export interface IElectronAPI {
}

declare global {
    interface Window {
        API: IElectronAPI
    }
}