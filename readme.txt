To communicate via SSL Stream, clients install your certificate and server to install p12 key/cert combo.

Install OpenSSL. https://slproweb.com/products/Win32OpenSSL.html

Copy openssl.cnf to C:\Program Files\Common Files\SSL/openssl.cnf

Generate your pem files: openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365

Convert pem to crt file: openssl x509 -outform der -in cert.pem -out cert.crt

Generate a .p12 file: openssl pkcs12 -export -out keyStore.p12 -inkey key.pem -in cert.pem

Server: Double click on p12 to install. Client: Double click on server.crt to install.

Place cert.crt next to csharp_server.exe
