const { app, BrowserWindow } = require('electron');
const path = require('path');
const express = require('express');

let server;

function createWindow() {
  const win = new BrowserWindow({
    width: 1200,
    height: 800,
    webPreferences: {
      nodeIntegration: true,
      contextIsolation: false
    }
  });

  const exp = express();
  const distPath = path.join(__dirname, '../dist/ElectronClient/browser');
  exp.use(express.static(distPath));

  server = exp.listen(4201, () => {
    console.log('Angular served at http://localhost:4201');
    win.loadURL('http://localhost:4201');
  });
}

app.whenReady().then(createWindow);

app.on('window-all-closed', () => {
  if (server) server.close();
  if (process.platform !== 'darwin') app.quit();
});
