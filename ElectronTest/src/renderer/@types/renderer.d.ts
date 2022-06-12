export interface IElectronAPI {
    GetDate: () => Promise<any>
    GetStock: () => Promise<any>
}

declare global {
    interface Window {
        API: IElectronAPI
    }
}