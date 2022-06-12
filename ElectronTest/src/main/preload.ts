import { contextBridge, ipcRenderer } from 'electron';

//■renderer公開API
contextBridge.exposeInMainWorld('API', {
  GetDate: async () => await ipcRenderer.invoke('GetDate'),
  GetStock: async () => await ipcRenderer.invoke('GetStock'),
});
