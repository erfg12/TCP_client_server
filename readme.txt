To communicate via SSL Stream you need to install your certificates and keys as Trusted Root.

Install OpenSSL. https://slproweb.com/products/Win32OpenSSL.html

Copy openssl.cnf to C:\Program Files\Common Files\SSL/openssl.cnf

Generate your private key and cert files: openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365

Convert pem to crt file: openssl x509 -outform der -in cert.pem -out cert.crt

Generate a .p12 file, double click to install it: openssl pkcs12 -export -out keyStore.p12 -inkey key.pem -in cert.pem

Open certlm.exe, right click on Trusted Root Certification Authorities > All Tasks > Import. Select cert.crt