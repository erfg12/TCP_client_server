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
    serverAddr.sin_port = htons(7891);
    serverAddr.sin_addr.s_addr = inet_addr("127.0.0.1");

    memset(serverAddr.sin_zero, '\0', sizeof serverAddr.sin_zero);
    bind(welcomeSocket, (struct sockaddr *) &serverAddr, sizeof(serverAddr));

    if (listen(welcomeSocket,5) == INVALID_SOCKET) {
        wprintf(L"socket function failed with error: %ld\n", WSAGetLastError());
        WSACleanup();
        return 1;
    }
    else
        printf("listening\n");
    
    addr_size = sizeof serverStorage;
    newSocket = accept(welcomeSocket, (struct sockaddr *) &serverStorage, &addr_size);

    //Receive a message from client
	while( (read_size = recv(newSocket, buffer, 1024, 0)) > 0 )
	{
        //send(newSocket, buffer, strlen(buffer), 0);
        printf("Received: %s\n", buffer);
	}

#ifdef _WIN32
    WSACleanup();
#endif
    return 0;
}