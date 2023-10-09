## TCP Server and Client

Simple TCP server and winforms client example code. There's also a Unity game engine client example in the `unity_client` directory.

Server has 3 optional args. `<IP> <PORT> <SSL>`

_Ex: csharp_server 192.168.1.20 5600 true_

## How to Setup SSL Communication

To communicate via SSL Stream, clients install your certificate and server installs p12 file.

Install OpenSSL. https://slproweb.com/products/Win32OpenSSL.html

Open cmd prompt, type: 

```
set OPENSSL_CONF=%programfiles%\OpenSSL-Win64\bin\openssl.cfg
setx OPENSSL_CONF "%programfiles%\OpenSSL-Win64\bin\openssl.cfg"
```

Also, type: 

```
set RANDFILE=%programfiles%OpenSSL-Win64\bin\.rnd
setx RANDFILE "%programfiles%OpenSSL-Win64\bin\.rnd"
```

Generate your pem files: `openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365`

Convert pem to crt file: `openssl x509 -outform der -in cert.pem -out cert.crt`

Generate a .p12 file: `openssl pkcs12 -export -out keyStore.p12 -inkey key.pem -in cert.pem`

Generate a .cer file: `openssl x509 -inform PEM -in cert.pem -outform DER -out cert.cer`

Server: Double click on p12 to install. Client: Double click on cert.crt to install.

Place cert.cer next to csharp_server.exe

