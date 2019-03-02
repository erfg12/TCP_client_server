const { app, BrowserWindow } = require("electron");

const Net = require("net");
const client = new Net.Socket();

let mainWindow;

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 800,
    height: 600,
    webPreferences: {
      nodeIntegration: true
    }
  });

  mainWindow.loadFile("index.html");

  mainWindow.setMenu(null);
  //mainWindow.openDevTools(); // DEBUG

  mainWindow.on("closed", function() {
    mainWindow = null;
  });
}

app.on("ready", createWindow);

app.on("window-all-closed", function() {
  if (process.platform !== "darwin") {
    app.quit();
  }
});

app.on("activate", function() {
  if (mainWindow === null) {
    createWindow();
  }
});

var ipc = require("electron").ipcMain;

ipc.on("sendAction", function(event, nick, msg) {
  client.write(nick + ":" + msg + "\0");
  event.sender.send("clearMsgs");
});

ipc.on("disconnect", function(event, data){
  client.end();
  client.destroy();
  event.sender.send("msgReply", "TCP connection has been closed.");
});

ipc.on("connectAction", function(event, host, port) {
  console.log("Attempting to connect to: " + host + ":" + port);
  event.sender.send("msgReply", "Attempting to connect...");

  client.connect(port, host, function() {
    console.log("TCP client connected to: " + host + ":" + port);
    event.sender.send("msgReply", "Connection was successful!");
    event.sender.send("toggleCBtn");
  });

  // receive data
  client.on("data", function(data) {
    var readData = data + "";

    if (readData[readData.length - 1] == "\0") {
      var msg = readData.split("\0"); // split data at null
      for (var i = 0, len = msg.length; i < len; i++) {
        if (msg[i].length > 0) {
          event.sender.send("msgReply", msg[i]);
        }
      }
    } else {
      readData += readData;
    }
  });

  client.on("close", function() {
    console.log("Client closed");
  });

  client.on("error", function(err) {
    console.error(err);
    event.sender.send("msgReply", "ERROR: " + JSON.stringify(err));
    event.sender.send("toggleCBtn");
  });
});
