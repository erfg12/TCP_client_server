#include "networking.h"

int main() {
    int clientSocket;
    char message[1024] , server_reply[1024];
    struct sockaddr_in serverAddr;
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

    clientSocket = socket(PF_INET, SOCK_STREAM, 0);

    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(13000);
    serverAddr.sin_addr.s_addr = inet_addr("127.0.0.1");

    memset(serverAddr.sin_zero, '\0', sizeof serverAddr.sin_zero);

    addr_size = sizeof serverAddr;
    connect(clientSocket, (struct sockaddr *) &serverAddr, addr_size);
    printf("Connected to server!\n");

    while(1)
	{
		printf("Message: ");
		gets(message);
		
		if (send(clientSocket, message, strlen(message), 0) < 0)
		{
			puts("Send failed");
			return 1;
		}
		
		if (recv(clientSocket, server_reply, 1024, 0) < 0)
		{
			puts("recv failed");
			break;
		}
		
        printf("Received: %s\n", server_reply);
	}

#ifdef _WIN32
    WSACleanup();
#endif
    return 0;
}