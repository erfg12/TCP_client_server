#include <stdio.h>
#ifdef _WIN32
#include <Winsock2.h>
#include <ws2tcpip.h>
#elif linux
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#endif
#include <string.h>

#ifndef HEADER_FILE
#define HEADER_FILE



#endif