#include "networking.h"

int main() {
    int welcomeSocket, newSocket, read_size;
    char buffer[1024];
    struct sockaddr_in serverAddr;
    struct sockaddr_storage serverStorage;
    socklen_t addr_size;

#ifdef _WIN32
    WSADATA wsaData;
    // Initialize Winsock
    int iResult = WSAStartup(MAKEWORD(2,2), &wsaData);
    if (iResult != 0) {
        printf("WSAStartup failed with error: %d\n", iResult);
        return 1;
    }
#endif

    welcomeSocket = socket(PF_INET, SOCK_STREAM, 0);

    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(13000);
    serverAddr.sin_addr.s_addr = inet_addr("127.0.0.1");

    memset(serverAddr.sin_zero, '\0', sizeof serverAddr.sin_zero);
    bind(welcomeSocket, (struct sockaddr *) &serverAddr, sizeof(serverAddr));

#ifdef _WIN32
    if (listen(welcomeSocket,5) == INVALID_SOCKET) {
        printf("socket function failed with error: %d\n", WSAGetLastError());
        WSACleanup();
        return 1;
    }
    else
        printf("listening\n");
#else
    if (listen(welcomeSocket,5) == SOL_SOCKET) {
        printf("socket function failed with error\n");
        return 1;
    }
    else
        printf("listening\n");
#endif
    
    addr_size = sizeof serverStorage;
    if ((newSocket = accept(welcomeSocket, (struct sockaddr *) &serverStorage, &addr_size)) > 0) {
        printf("Client connected!\n");
    }

    //Receive a message from client
	while( (read_size = recv(newSocket, buffer, 1024, 0)) > 0 )
	{
        char * CmdString = strtok(buffer, "\n");
        send(newSocket, CmdString, strlen(CmdString), 0); // send message back to client
        printf("Received:%s\n", CmdString);
	}

#ifdef _WIN32
    WSACleanup();
#endif
    return 0;
}