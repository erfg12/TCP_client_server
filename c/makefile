CC = "gcc"

all: server client

server: src/server.o
	$(CC) src/server.c -o server -lws2_32

client: src/client.o
	$(CC) src/client.c -o client -lws2_32