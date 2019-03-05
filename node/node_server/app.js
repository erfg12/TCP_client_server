var net = require('net');

var PORT = 13000;

var clients = [];

net.createServer(function(sock) {
    console.log('CONNECTED: ' + sock.remoteAddress +':'+ sock.remotePort);
    sock.name = "";
    clients.push(sock);
    
    sock.on('data', function(data) {
        //console.log('RAW DATA RCVD: ' + sock.remoteAddress + ': ' + data);
        var readData = data + '';
        
        if (readData[readData.length -1] == '\0') { // check if end of msg (if last char is null)
            var msg = readData.split('\0'); // split data at null
            for (var i = 0, len = msg.length; i < len; i++) {
                if (msg[i].length > 0) { // if not dead end
                    console.log('MSG RCVD: ' + msg[i]);
                    if (msg[i].indexOf('|') > -1) // command
                        parseCMD(msg[i]);
                    else // normal chat msg
                        broadcast(msg[i]);
                }
            }
        } else {
            readData += readData; // append to new data and wait for null char
        }
    });

    sock.on('end', function() {
        console.log('CLOSED: ' + sock.remoteAddress + ':' + sock.remotePort);
        clients.splice(clients.indexOf(sock), 1);
        broadcast(sock.name + " left.");
    });

    sock.on('error', (err) => {
        //console.log('TCP error', err);
        console.log('TCP_ERROR: ' + sock.remoteAddress + ':' + sock.remotePort);
        clients.splice(clients.indexOf(sock), 1);
        broadcast(sock.name + " left.");
    })

    function broadcast(message) {
        clients.forEach(function (client) {
            //console.log("SENDING:" + message);
            client.write(message + '\0');
        });
    }

    /// commands contain '|' characters. 0 = command, 1+ = arguments.
    function parseCMD(cmd){
        var nCmd = cmd.split('|');
        if (nCmd[0] == "con") { // member connected. Format: 0 = command, 1 = member name
            sock.name = nCmd[1]; // name the socket
            broadcast("Welcome " + nCmd[1] + "!");
        }
        else if (nCmd[0] == "lm") { // list members. Format: 0 = command
            var cNames = [];
            clients.forEach(function (client) {
                cNames.push(client.name);
            });
            sock.write(cNames.join(',') + '\0');
        } else if (nCmd[0] == "pm") { // private message. Format: 0 = command, 1 = receiving member, 2 = message
            clients.forEach(function (client) {
                if (client.name == nCmd[1]){
                    client.write("[PRIV]" + sock.name + ":" + nCmd[2]);
                }
            });
        }
    }
    
}).listen(PORT);

console.log('Server listening on port ' + PORT);