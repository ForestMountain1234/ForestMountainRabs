import path from 'path';
import { BrowserWindow, app, ipcMain, session } from 'electron';

const isDev = process.env.NODE_ENV === 'development';


//-----------------------------------------------------------------------------------------
//■開発用 ホットリロード設定
//-----------------------------------------------------------------------------------------
if (isDev) {
  require('electron-reload')(__dirname, {
    electron: path.resolve(__dirname,'../node_modules/.bin/electron')
  });
}

//-----------------------------------------------------------------------------------------
//■Windowの作成
//-----------------------------------------------------------------------------------------
const createWindow = () => {
  //インスタンス生成
  const mainWindow = new BrowserWindow({
    webPreferences: { preload: path.join(__dirname, 'preload.js')}
  });

  //開発用/本番用設定
  if (isDev) {
    mainWindow.webContents.openDevTools({ mode: 'detach' });//devツール表示
  }
  else {
    mainWindow.setMenuBarVisibility(false);//メニューバー非表示
  }

  mainWindow.loadFile('dist/index.html');//windowに表示するhtmlを読み込む
};


//-----------------------------------------------------------------------------------------
//■Windowの表示
//-----------------------------------------------------------------------------------------
app.whenReady().then(createWindow);
app.once('window-all-closed', () => app.quit());
