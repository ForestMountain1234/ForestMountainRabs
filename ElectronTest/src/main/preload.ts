import { contextBridge, ipcRenderer } from 'electron';

//■renderer公開API
contextBridge.exposeInMainWorld('API', {
  //公開API名: (引数) => ipcRenderer.send('ipcMainのAPI名', 引数),
});
