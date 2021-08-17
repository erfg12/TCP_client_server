CC = "gcc"

all: server client

server: src/server.o
	$(CC) src/server.c -o server

client: src/client.o
	$(CC) src/client.c -o client